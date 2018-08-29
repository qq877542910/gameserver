using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer
{
    class Program
    {
        static void Main(string[] args)
        {
            ServerSocket server = new ServerSocket();
            server.Start();

            //SqliteHelper sqlHelper = new SqliteHelper();
            //sqlHelper.Action();

            Console.ReadLine();
        }
    }
}
