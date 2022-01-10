using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Microsoft.Extensions.Configuration;
using MTCG.DAL;
using MTCG.Http;

namespace MTCG
{
    internal class Program
    {
        static void Main(string[] args)
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
                var socket = listener.AcceptTcpClient();
                new Thread(HandleClient).Start(socket);
            }
        }

        private static void HandleClient(Object obj)
        {
            var socket = (TcpClient) obj;
            HttpHandler handler = new HttpHandler(socket);
            handler.Handle();
        }
    }
}