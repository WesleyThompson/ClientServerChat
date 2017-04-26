using System;
using System.Text;
using System.Net.Sockets;
using System.Threading;
using System.Collections.Generic;

namespace ChatServer
{
    public class ClientHandler
    {
        TcpClient clientSocket;
        string clNo;
        public void startClient(TcpClient inClientSocket, string clineNo)
        {
            this.clientSocket = inClientSocket;
            this.clNo = clineNo;
            Thread ctThread = new Thread(doChat);
            ctThread.Start();
        }
        private void doChat()
        {
            int requestCount = 0;
            byte[] bytesFrom = new byte[1024];
            string dataFromClient = null;
            Byte[] sendBytes = new byte[1024];
            string serverResponse = null;
            string rCount = null;
            requestCount = 0;

            bool loggedIn = false;
            String username = null;
            String password = null;

            Dictionary<string, string> users = new Dictionary<string, string>()
            {
                { "Tom" , "Tom11"}
            };

            while (true)
            {
                try
                {
                    requestCount = requestCount + 1;
                    NetworkStream networkStream = clientSocket.GetStream();
                    networkStream.Read(bytesFrom, 0, 1024);
                    dataFromClient = System.Text.Encoding.ASCII.GetString(bytesFrom);
                    dataFromClient = dataFromClient.Substring(0, dataFromClient.IndexOf("$"));
                    Console.WriteLine(" >> " + "From client-" + clNo + dataFromClient);

                    rCount = Convert.ToString(requestCount);

                    switch(dataFromClient)
                    {
                        default:
                            if(dataFromClient.StartsWith("login "))
                            {
                                string loginStuff = RemoveString(dataFromClient, "login ");
                                ExtractLogin(ref username, ref password, loginStuff);

                                if (username == null || password == null)
                                {
                                    serverResponse = "Login missing username or password.";
                                }
                                else
                                {
                                    
                                    serverResponse = "Login confirmed.";
                                }
                            }
                            else
                            {
                                serverResponse = "Could not understand the command: " + dataFromClient + ".";
                            }
                            break;
                    }

                    
                    sendBytes = Encoding.ASCII.GetBytes(serverResponse);
                    networkStream.Write(sendBytes, 0, sendBytes.Length);
                    networkStream.Flush();
                    Console.WriteLine(" >> " + serverResponse);
                }
                catch (InvalidOperationException ioe)
                {
                    break;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(" >> " + ex.ToString());
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

            if (inputs.Length< 2 || inputs.Length> 2)
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

    }
}
