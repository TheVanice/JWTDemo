using Microsoft.IdentityModel.Tokens;
using Shared;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;

/// <summary>
/// The Client should: Request a token from the server and use it to request data from the service
/// </summary>
namespace JWTDemo.Client
{
    public class Program
    {
        public static void Main(string[] args)
        {
            using var serviceConnection = new TCPClientConnection(IPAddress.Loopback.ToString(), Constants.SERVICE_PORT);
            using var serverConnection = new TCPClientConnection(IPAddress.Loopback.ToString(), Constants.SERVER_PORT);

            var cts = new CancellationTokenSource();

            _ = Task.Run(async () =>
            {
                JwtSecurityToken? token = null;
                int i = 0;
                while (true)
                {
                    try
                    {
                        if (token == null)
                        {
                            string user = i % 2 == 0 ? Constants.Admin : Constants.User;
                            string pass = i % 2 == 0 ? Constants.AdminPass : Constants.UserPass;

                            Console.WriteLine("Requesting new token ...");
                            token = await RequestAuthToken(user, pass, serverConnection, cts);
                            i++;
                        }

                        var handler = new JwtSecurityTokenHandler();
                        var result = await serviceConnection.Send(new DataRequest
                        {
                            Message = i % 2 == 0 ? "Write" : "Read",
                            Token = handler.WriteToken(token),
                        }.ToString(), cts.Token).ConfigureAwait(false);

                        if (result == Constants.INVALIDTOKEN)
                        {
                            token = null;
                            Console.WriteLine("Token invalid ...");
                        }
                        else
                        {
                            Console.WriteLine(result);
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                    await Task.Delay(1000).ConfigureAwait(false);
                }
            }, cts.Token);

            Console.ReadLine();
            cts.Cancel();
        }

        private static async Task<JwtSecurityToken> RequestAuthToken(string user, string pass, TCPClientConnection serverConnection, CancellationTokenSource cts)
        {
            var jwtEncoded = await serverConnection.Send(new IssueTokenRequest
            {
                UserName = user,
                Pass = pass,
                Service = Constants.ServiceName,
            }.ToString(), cts.Token);
            return new JwtSecurityToken(jwtEncoded);
        }
    }
}