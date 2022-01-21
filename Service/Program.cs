using JWTDemo.Shared;
using Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;

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
                await server.Listen(cts.Token, (message) =>
                {
                    if (message == Constants.Token)
                        return "OK - here is the data";
                    else
                        return "ERROR - invalid token";
                });
            });

            Console.ReadLine();
            cts.Cancel();
        }
    }
}