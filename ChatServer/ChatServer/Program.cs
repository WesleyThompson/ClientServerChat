using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;

namespace ChatServer
{
    class Program
    {
        static void Main(string[] args)
        {
            TcpListener server = null;
            TcpClient client = null;

            int counter = 0;

            try
            {
                //Student ID = 12382149
                Int32 listeningPort = 12149;
                IPAddress local = IPAddress.Parse("127.0.0.1");

                server = new TcpListener(local, listeningPort);
                client = default(TcpClient);

                Console.WriteLine("Starting up chat server");
                server.Start();

                counter = 0;

                while(true)
                {
                    counter++;
                    client = server.AcceptTcpClient();
                    Console.WriteLine("Client #" + counter + " entered the chat!");
                    ClientHandler clientHandle = new ClientHandler();
                    clientHandle.StartClient(client, counter.ToString());
                }

                client.Close();
                server.Stop();
                Console.WriteLine("Ending chat server");
            }
            catch(SocketException e)
            {
                Console.WriteLine("Socket Exception: ", e);
            }
            finally
            {
                server.Stop();
                Console.WriteLine("Closing down chat server");
                //Console.ReadLine();
            }
        }
    }
}
