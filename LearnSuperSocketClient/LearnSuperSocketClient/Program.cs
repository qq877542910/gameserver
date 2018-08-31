using SuperSocket.ClientEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
namespace LearnSuperSocketClient
{
    class Program
    {
        static async Task<bool> Connect(EasyClient client)
        {
            return await client.ConnectAsync(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 2017));
        }

        static void Main(string[] args)
        {
            var client = new MyClient();

            client.Initialize(new MyReceiveFilter(), (request) =>
             {
                 Console.WriteLine(request.Key);
             });

            var connect = Connect(client);

            if (connect.Result)
            {
                client.Send(Encoding.ASCII.GetBytes("LOGIN kerry"));
            }

            Console.ReadLine();
        }


    }
}
