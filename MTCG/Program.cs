using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using MTCG.Http;
using HttpStatusCode = MTCG.Http.HttpStatusCode;

namespace MTCG
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            TcpListener listener = new TcpListener(IPAddress.Loopback, 8000);
            listener.Start(5);

            Console.CancelKeyPress += (sender, e) => Environment.Exit(0);

            while (true)
            {
                try
                {
                    var socket = await listener.AcceptTcpClientAsync();
                    using var writer = new StreamWriter(socket.GetStream()) { AutoFlush = true };

                    using var reader = new StreamReader(socket.GetStream());
                    
                    HttpClient client = new HttpClient(reader);
                    client.Handle();

                    HttpResponse res = new HttpResponse
                    {
                        StatusCode = HttpStatusCode.Success,
                        Body = "Hello World"
                    };

                    await writer.WriteLineAsync(res.ToString());
                }
                catch (Exception exc)
                {
                    Console.WriteLine("error occurred: " + exc.Message);
                }
            }
        }
    }
}