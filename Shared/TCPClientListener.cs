using Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace JWTDemo.Shared
{
    public class TCPClientListener
    {
        private readonly TcpListener _server;

        public TCPClientListener(string ip, int port)
        {
            var localAddr = IPAddress.Parse(ip);
            _server = new TcpListener(localAddr, port);
        }

        public async Task Listen(CancellationToken cancellationToken, Func<string, string> messageReceivedHandler)
        {
            if (messageReceivedHandler is null)
            {
                throw new ArgumentNullException(nameof(messageReceivedHandler));
            }

            try
            {
                _server.Start();

                var data = string.Empty;
                var bytes = new Byte[256];

                // Enter the listening loop.
                while (!cancellationToken.IsCancellationRequested)
                {
                    Console.Write("Waiting for a connection... ");
                    var client = await _server.AcceptTcpClientAsync(cancellationToken).ConfigureAwait(false);
                    Console.WriteLine("Connected!");

                    var stream = client.GetStream();

                    int i;

                    // Loop to receive all the data sent by the client.
                    while ((i = await stream.ReadAsync(bytes, cancellationToken).ConfigureAwait(false)) != 0)
                    {
                        // Translate data bytes to a ASCII string.
                        data = System.Text.Encoding.ASCII.GetString(bytes, 0, i);
                        Console.WriteLine("Received: {0}", data);

                        var response = messageReceivedHandler(data);
                        var msg = System.Text.Encoding.ASCII.GetBytes(response);

                        // Send back a response.
                        stream.Write(msg, 0, msg.Length);
                        Console.WriteLine("Sent: {0}", data);
                    }

                    // Shutdown and end connection
                    client.Close();
                }
            }
            finally
            {
                _server.Stop();
            }
        }
    }
}
