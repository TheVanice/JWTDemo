using JWTDemo.Shared;
using Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;

/// <summary>
/// The Server should: Issue a token for clients when credentials are ok
/// </summary>
namespace JWTDemo.Server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var server = new TCPClientListener(IPAddress.Loopback.ToString(), Constants.SERVER_PORT);
            var cts = new CancellationTokenSource();

            _ = Task.Run(async () =>
              {
                  await server.Listen(cts.Token, (message) =>
                  {
                      try
                      {
                          var request = IssueTokenRequest.FromString(message);
                          if (VerifyRequest(request))
                          {
                              var handler = new JwtSecurityTokenHandler();
                              var token = IssueToken(request.Service, request.UserName == Constants.Admin);
                              return handler.WriteToken(token);
                          }
                          else
                              return "-unauthorized-";
                      }
                      catch (Exception e)
                      {
                          Console.WriteLine(e);
                          return "-internal error-";
                      }
                  });
              });

            Console.ReadLine();
            cts.Cancel();
        }

        private static bool VerifyRequest(IssueTokenRequest request)
        {
            if (request.UserName == Constants.Admin && request.Pass == Constants.AdminPass)
            {
                return true;
            }
            else if (request.UserName == Constants.User && request.Pass == Constants.UserPass)
            {
                return true;
            }

            return false;
        }

        private static JwtSecurityToken IssueToken(string service, bool writePermission)
        {
            Claim[] claims;

            if (writePermission)
            {
                claims = new Claim[]
                {
                    new Claim(Constants.ReadPermission, ""),
                    new Claim(Constants.WritePermission, "")
                };
            }
            else
            {
                claims = new Claim[]
                {
                    new Claim(Constants.ReadPermission, ""),
                };
            }

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Constants.TokenSignKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            return new JwtSecurityToken(
                issuer: Constants.AuthService,
                audience: service,
                expires: DateTime.Now.AddSeconds(10),
                //signingCredentials: credentials,
                signingCredentials: null,
                claims: claims);
        }
    }
}