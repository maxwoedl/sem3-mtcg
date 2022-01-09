using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using MTCG.DAL;
using MTCG.Http;
using HttpStatusCode = MTCG.Http.HttpStatusCode;

namespace MTCG
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            // var config = new ConfigurationBuilder()
            //     .SetBasePath(Directory.GetCurrentDirectory())
            //     .AddJsonFile("appsettings.json", false, true)
            //     .Build();
            
            TcpListener listener = new TcpListener(IPAddress.Loopback, 10001);
            listener.Start(5);

            Console.CancelKeyPress += (sender, e) => Environment.Exit(0);

            var dbContext = DatabaseContext.GetInstance();
            dbContext.Init();
            
            while (true)
            {
                var socket = await listener.AcceptTcpClientAsync();
                HttpHandler handler = new HttpHandler(socket);
                handler.Handle();
            }
        }
    }
}