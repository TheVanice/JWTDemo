using Shared;
using System;
using System.Collections.Generic;
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
                while (true)
                {
                    var jwt = await serverConnection.Send(Constants.ValidTokenRequest, cts.Token);
                    if (jwt != null)
                    {
                        var result = await serviceConnection.Send(jwt, cts.Token).ConfigureAwait(false);
                        Console.WriteLine(result);
                    }
                    await Task.Delay(1000).ConfigureAwait(false);
                }
            }, cts.Token);

            Console.ReadLine();
            cts.Cancel();
        }
    }
}