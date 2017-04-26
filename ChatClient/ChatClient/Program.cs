using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net.Sockets;
using System.Net;

namespace ChatClient
{
    public class Program
    {
        static TcpClient clientSocket = new TcpClient();
        static NetworkStream serverStream = default(NetworkStream);

        static Thread clientThread = null;
        static bool isRunning = true;
        static string thisID = null;
        static bool connectedToServer = false;

        static Int32 byteLimit = 1024;
        static int port = 12149;
        static string ipAddress = "127.0.0.1";

        static void Main(string[] args)
        {
            Connect();

            while(isRunning)
            {
                String input = Console.ReadLine();
                if(input.Equals("exit"))
                {
                    isRunning = false;
                    clientThread.Abort();
                    break;
                }

                Byte[] output = Encoding.ASCII.GetBytes(input + "$");

                try
                {
                    serverStream.Write(output, 0, output.Length);
                    serverStream.Flush();
                }
                catch(Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }

        private static void Connect()
        {
            try
            {
                Console.Write("Connecting to Wes Chat ... ");
                clientSocket.Connect(ipAddress, port);

                //Start a background thread that outputs text to the console while we are typing etc
                clientThread = new Thread(GetMessage);
                clientThread.Start();
                clientThread.IsBackground = true;

                connectedToServer = true;
                Console.WriteLine("Connected");
            }
            catch(Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private static void GetMessage()
        {
            while(connectedToServer && clientSocket.Connected)
            {
                try
                {
                    serverStream = clientSocket.GetStream();

                    byte[] buffer = new byte[byteLimit];

                    int readStatus = serverStream.Read(buffer, 0, byteLimit);
                    if (readStatus <= 0)
                    {
                        connectedToServer = false;
                        break;
                    }

                    string returnData = System.Text.Encoding.ASCII.GetString(buffer);
                    returnData = returnData.Substring(0, returnData.IndexOf("\0"));

                    Console.WriteLine(returnData);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }
    }
}