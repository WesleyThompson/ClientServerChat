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
        static TcpClient clientSocket;
        static String userId = null;
        static String password = null;

        static void Main(string[] args)
        {
            bool runFlag = true;
            string input;

            Int32 port = 12149;
            string local = "127.0.0.1";

            ConnectToServer(port, local);

            Console.WriteLine("Welcome to WesChat!");
            Console.WriteLine("Type help for available chat commands.");

            while (runFlag)
            {
                input = Console.ReadLine();
                input = input.ToLower();

                switch (input)
                {
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
                        if(input.StartsWith("send all "))
                        {
                            string message = RemoveString(input, "send all ");
                            SendMessage(message);
                        }

                        else if(input.StartsWith("login "))
                        {
                            string loginStuff = RemoveString(input, "login ");
                            
                            

                            ExtractLogin(ref userId, ref password, loginStuff);

                            //TODO remove this
                            Console.WriteLine("UserId: " + userId);
                            Console.WriteLine("Password: " + password);

                            if(userId == null || password == null)
                            {
                                break;
                            }

                        }

                        else
                        {
                            Console.WriteLine("Could not understand the command: " + input);
                        }
                        break;
                }
            }
        }

        private static string RemoveString(string input, string removal)
        {
            int index = input.IndexOf(removal);
            string message = (index < 0)
                ? input
                : input.Remove(index, removal.Length);

            return message;
        }

        private static void ExtractLogin(ref String userId, ref String password, string loginStuff)
        {
            //Error checking
            string[] stringSeparators = new string[] { " " };
            string[] inputs = loginStuff.Split(stringSeparators, StringSplitOptions.None);

            if (inputs.Length < 2 || inputs.Length > 2)
            {
                Console.WriteLine("Incorrect number of login arguments. Try again.");
            }

            foreach (string input in inputs)
            {
                if (String.IsNullOrEmpty(input) || String.IsNullOrWhiteSpace(input))
                {
                    Console.WriteLine("Empty or missing login arguments.");
                    break;
                }

                if (userId == null)
                {
                    userId = input;
                }
                else if (password == null)
                {
                    password = input;
                }
            }
        }

        private static void ConnectToServer(Int32 port, string server)
        {
            try
            {
                clientSocket = new TcpClient();
                clientSocket.Connect(server, port);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            
        }

        private static void SendMessage(string message)
        {
            if(userId == null || password == null)
            {
                Console.WriteLine("Please login before sending a message.");
                return;
            }

            try
            {
                NetworkStream serverStream = clientSocket.GetStream();
                Byte[] data = System.Text.Encoding.ASCII.GetBytes(message);
                serverStream.Write(data, 0, data.Length);
                Console.WriteLine(userId + ": " + message);

                data = new byte[1024];
                string responseData = String.Empty;
                Int32 bytes = serverStream.Read(data, 0, data.Length);
                responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes);
                Console.WriteLine("Server: " + responseData);

                serverStream.Close();
            }
            catch(Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}
