using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NetworkSocket.Fast;
using System.Configuration;
using LuceneServer.Filters;
using LuceneServer.Services;

namespace LuceneServer
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "LuceneServer";

            var server = new LnServer();
            server.GlobalFilter.Add(new ExceptionFilter());
            server.BindService<LnService>().BindService<SystemService>();
            server.StartListen(int.Parse(ConfigurationManager.AppSettings["Port"]));

            Console.WriteLine("服务监听中：端口" + server.LocalEndPoint.Port);
            while (true)
            {
                Console.ReadLine();
            }
        }
    }
}
