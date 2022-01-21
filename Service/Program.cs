using JWTDemo.Shared;
using Microsoft.IdentityModel.Tokens;
using Shared;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Claims;
using System.Text;

/// <summary>
/// The Service should: Require a valid token to return data
/// </summary>
namespace JWTDemo.Service
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var server = new TCPClientListener(IPAddress.Loopback.ToString(), Constants.SERVICE_PORT);
            var cts = new CancellationTokenSource();

            _ = Task.Run(async () =>
            {
                string data = "OK - here is the data";
                await server.Listen(cts.Token, (message) =>
                {
                    try
                    {
                        var request = DataRequest.FromString(message);
                        var identity = VerifyToken(request.Token);
                        if (request.Message == "Read" && VerifyPermissions(identity, Constants.ReadPermission))
                        {
                            return data;
                        }
                        else if (request.Message == "Write" && VerifyPermissions(identity, Constants.WritePermission))
                        {
                            data = data + " - " + DateTime.Now.ToString();
                            return data;
                        }
                        else
                        {
                            return Constants.INVALIDTOKEN;
                        }
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

        private static bool VerifyPermissions(ClaimsIdentity identity, string permission)
        {
            return identity.HasClaim(permission, "");
        }

        private static ClaimsIdentity VerifyToken(string authToken)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var validationParameters = new TokenValidationParameters()
            {
                ValidateLifetime = true,
                ValidateAudience = true,
                ValidateIssuer = true,
                ValidIssuer = Constants.AuthService,
                ValidAudience = Constants.ServiceName,
                //IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Constants.TokenSignKey))
                RequireSignedTokens = false,
            };

            var principal = tokenHandler.ValidateToken(authToken, validationParameters, out SecurityToken validatedToken);
            return new ClaimsIdentity(principal.Identity, principal.Claims);
        }
    }
}