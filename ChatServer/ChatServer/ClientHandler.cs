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
            string rCount = null;
            requestCount = 0;

            bool runningClient = true;
            bool loggedIn = false;
            String username = null;
            String password = null;

            while (runningClient)
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
                        case "who":
                            foreach(string person in Program.activeUsers.Keys)
                            {
                                Program.SendToUser(person, username, false, clientSocket, true);
                            }
                            break;

                        case "logout":
                            if(loggedIn)
                            {
                                runningClient = false;
                                //Change to unicast
                                Program.Broadcast("User " + username + " logged out.", username, true);
                                Program.activeUsers.Remove(username);
                            }
                            else
                            {
                                Program.SendToUser("Cannot logout if not logged in.", username, false, clientSocket, true);
                            }
                            break;

                        default:
                            if(dataFromClient.StartsWith("login "))
                            {
                                string loginStuff = RemoveString(dataFromClient, "login ");
                                username = null;
                                password = null;
                                ExtractLogin(ref username, ref password, loginStuff);

                                Console.WriteLine(username + " " + password);

                                if (username == null || password == null)
                                {
                                    Program.SendToUser("Login Failed.", username, false, clientSocket, true);
                                }
                                else
                                {
                                    string pass;
                                    if(Program.users.TryGetValue(username, out pass))
                                    {
                                        if (pass.Equals(password))
                                        {
                                            if (Program.activeUsers.Count == Program.maxClients)
                                            {
                                                Program.SendToUser("Max number of users in the chat, try again later.", username, false, clientSocket, true);
                                            }
                                            else
                                            {
                                                Program.Broadcast(username + " has joined.", username, true);
                                                Program.SendToUser("Login Accepted.", username, false, clientSocket, true);
                                                Program.activeUsers.Add(username, clientSocket);
                                                loggedIn = true;
                                            }
                                        }
                                        else
                                        {
                                            Program.SendToUser("Login Failed! ", username, false, clientSocket, true);
                                        }
                                    }
                                    else
                                    {
                                        Program.SendToUser("Login Failed.", username, false, clientSocket, true);
                                    }
                                    
                                }
                            }
                            else if(dataFromClient.StartsWith("send all "))
                            {
                                if (loggedIn)
                                {
                                    string message = RemoveString(dataFromClient, "send all ");
                                    Program.Broadcast(message, username, true);
                                }
                                else
                                {
                                    Program.SendToUser("You must be logged in to send messages.", username, false, clientSocket, true);
                                }
                            }
                            else if(dataFromClient.StartsWith("send "))
                            {
                                if(loggedIn)
                                {
                                    string message = RemoveString(dataFromClient, "send ");
                                    string[] pieces = message.Split(' ');

                                    bool found = false;
                                    foreach(string user in Program.activeUsers.Keys)
                                    {
                                        if(pieces[0].Equals(user))
                                        {
                                            found = true;
                                            message = RemoveString(message, user + " ");
                                            Program.SendToUser(message, user, true, Program.activeUsers[user], true);
                                        }
                                    }

                                    if(!found)
                                    {
                                        Program.SendToUser("User you tried to send to is not in the chat", username, false, clientSocket, true);
                                    }
                                }
                                else
                                {
                                    Program.SendToUser("You must be logged in to send messages.", username, false, clientSocket, true);
                                }
                            }
                            else
                            {
                                Program.SendToUser("Could not understand the command: " + dataFromClient + ".", username, false, clientSocket, true);
                            }
                            break;
                    }
                }
                catch (InvalidOperationException ioe)
                {
                    Console.WriteLine(ioe);
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
