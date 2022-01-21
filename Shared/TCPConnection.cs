using System.Net.Sockets;
using System;

namespace Shared
{
    public class TCPClientConnection : IDisposable
    {
        private readonly TcpClient _tcpClient;
        private readonly NetworkStream _stream;

        public TCPClientConnection(string hostName, int port)
        {
            _tcpClient = new(hostName, port);
            _stream = _tcpClient.GetStream();
        }

        public async Task<string?> Send(string message, CancellationToken cancellationToken)
        {
            try
            {
                //Send data
                Console.WriteLine($"SEND {DateTime.Now}: '{message}' ");
                var data = System.Text.Encoding.ASCII.GetBytes(message);
                await _stream.WriteAsync(data, cancellationToken).ConfigureAwait(false);

                //process response
                data = new Byte[256];
                var bytes = await _stream.ReadAsync(data, cancellationToken).ConfigureAwait(false);
                var responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes);
                Console.WriteLine($"RESP {DateTime.Now}: '{responseData}' ");
                return responseData;
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: {0}", e);
                return null;
            }
        }

        void IDisposable.Dispose()
        {
            _stream.Close();
            _tcpClient.Close();
        }
    }
}