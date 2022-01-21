using JWTDemo.Shared;
using Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

/// <summary>
/// The Server should: Issue a token for clients when cridentials are ok
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
                      if (message == Constants.ValidTokenRequest)
                          return Constants.Token;
                      else
                          return "ERROR";
                  });
              });

            Console.ReadLine();
            cts.Cancel();
        }
    }
}