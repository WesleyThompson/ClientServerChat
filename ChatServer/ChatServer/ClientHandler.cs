using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Threading;

namespace ChatServer
{
    class ClientHandler
    {
        TcpClient socket;
        string clientName;

        public void StartClient(TcpClient inSocket, string clientName)
        {
            this.socket = inSocket;
            this.clientName = clientName;

            //Threaded
            Thread clientThread = new Thread(StartChat);
            clientThread.Start();
        }


        private void StartChat()
        {
            //Restricting to 1024 cuz idk I wanna
            Byte[] buffer = new Byte[1024];
            string data;

            while(true)
            {
                try
                {
                    NetworkStream netStream = socket.GetStream();
                    netStream.Read(buffer, 0, buffer.Length);

                    data = System.Text.Encoding.ASCII.GetString(buffer);
                    data = data.Substring(0); //left a piece off

                    Console.WriteLine(clientName + ": " + data);

                    netStream.Flush();
                }
                catch(Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }
    }
}
