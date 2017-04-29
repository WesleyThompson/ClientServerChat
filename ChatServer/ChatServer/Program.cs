using System;
using System.Net.Sockets;
using System.Net;
using System.Collections.Generic;

namespace ChatServer
{
    class Program
    {
        static int port = 12149;
        static string ipAddress = "127.0.0.1";

        static bool isRunning = true;

        public volatile static Dictionary<string, TcpClient> activeUsers = new Dictionary<string, TcpClient>();
        public static Dictionary<string, string> users = new Dictionary<string, string>()
            {
                { "Tom", "Tom11"},
                { "David", "David22"},
                { "Beth", "Beth33"},
                { "John", "John44"}
            };

        public static int maxClients = 3;

        static void Main(string[] args)
        {
            TcpListener serverSocket = new TcpListener( IPAddress.Parse(ipAddress), port);
            TcpClient clientSocket = default(TcpClient);

            serverSocket.Start();
            Console.WriteLine("Server Started");

            int threadCounter = 0;
            while (isRunning)
            {
                threadCounter += 1;
                clientSocket = serverSocket.AcceptTcpClient();
                Console.WriteLine("Client No: " + threadCounter + " started!");
                ClientHandler client = new ClientHandler();
                client.startClient(clientSocket, Convert.ToString(threadCounter));
            }

            clientSocket.Close();
            serverSocket.Stop();
            Console.WriteLine(" >> " + "exit");
            Console.ReadLine();
        }

        public static void SendToUser(string message, string username, bool flag, TcpClient tcpClient, bool isPrivate)
        {
            TcpClient socket;
            socket = tcpClient;
            NetworkStream netStream = socket.GetStream();
            Byte[] bytes = null;

            if (flag == true)
            {
                if (isPrivate == true)
                {
                    bytes = System.Text.Encoding.ASCII.GetBytes(username + " (private): " + message);
                }
                else
                {
                    bytes = System.Text.Encoding.ASCII.GetBytes(username + ": " + message);
                }
            }
            else
            {
                bytes = System.Text.Encoding.ASCII.GetBytes(message);
            }

            netStream.Write(bytes, 0, bytes.Length);
            netStream.Flush();
        }

        public static void Broadcast(string message, string username, bool flag)
        {
            foreach(KeyValuePair<string, TcpClient> user in activeUsers)
            {
                if(user.Key != username)
                {
                    SendToUser(message, username, flag, user.Value, false);
                }
            }
        }

    }
}
