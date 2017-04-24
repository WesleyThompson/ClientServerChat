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
                Console.WriteLine("Waiting on connections");
                server.Start();

                counter = 0;

                while(true)
                {
                    try
                    {
                        counter++;
                        client = server.AcceptTcpClient();
                        Console.WriteLine("Client #" + counter + " entered the chat!");
                        //ClientHandler clientHandle = new ClientHandler();
                        //clientHandle.StartClient(client, counter.ToString());

                        NetworkStream stream = client.GetStream();

                        Byte[] bytes = new Byte[1024];
                        String data = null;
                        int i;

                        // Loop to receive all the data sent by the client.
                        while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
                        {
                            // Translate data bytes to a ASCII string.
                            data = System.Text.Encoding.ASCII.GetString(bytes, 0, i);
                            Console.WriteLine("Received: {0}", data);

                            // Process the data sent by the client.
                            data = data.ToUpper();

                            byte[] msg = System.Text.Encoding.ASCII.GetBytes(data);

                            // Send back a response.
                            stream.Write(msg, 0, msg.Length);
                            Console.WriteLine("Sent: {0}", data);
                        }

                        //client.Close();
                    }
                    catch(Exception e)
                    {
                        Console.WriteLine(e);
                    }
                }
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
