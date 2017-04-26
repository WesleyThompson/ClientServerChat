using System;
using System.Net.Sockets;
using System.Net;

namespace ChatServer
{
    class Program
    {
        static int port = 12149;
        static string ipAddress = "127.0.0.1";

        static bool isRunning = true;

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
    }
}
