using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;

namespace ChatClient
{
    class Program
    {
        static void Main(string[] args)
        {
            bool runFlag = true;
            string input;

            Int32 port = 12149;
            string local = "127.0.0.1";

            TcpClient clientSocket;
            NetworkStream serverStream;

            try
            {
                clientSocket = new TcpClient(local, port);

                serverStream = clientSocket.GetStream();
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }

            Console.WriteLine("Welcome to WesChat!");
            Console.WriteLine("Type help for available chat commands.");

            while (runFlag)
            {
                input = Console.ReadLine();
                input = input.ToLower();

                switch (input)
                {
                    case "login":
                        break;

                    case "send all":
                        break;

                    case "who":
                        break;

                    case "logout":
                        break;

                    case "help":
                        Console.WriteLine("Commands available:\nlogin\nsend all\nwho\nlogout\nhelp\nexit");
                        break;

                    case "exit":
                        runFlag = false;
                        break;

                    default:
                        Console.WriteLine("Could not understand the command: " + input);
                        break;
                }
            }
        }
    }
}
