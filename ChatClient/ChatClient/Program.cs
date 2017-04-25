using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net.Sockets;
using System.Net;

namespace ChatClient
{
    public partial class Program
    {
        static System.Net.Sockets.TcpClient clientSocket = new System.Net.Sockets.TcpClient();
        static NetworkStream serverStream = default(NetworkStream);
        string readData = null;
        public static Thread ctThread = null;
        public static bool stillAlive = true;
        public static string thisID = null;
        public static bool connectedToServer = false;
        public static bool madeConnection = false;

        static void Main(string[] args)
        {

            Console.CancelKeyPress += new ConsoleCancelEventHandler(myHandler);
            Program t = new Program();
            t.button2_Click();

            while (stillAlive)
            {
                t.button1_Click();
            }
        }

        private void button1_Click()
        {
            String s = Console.ReadLine();
            byte[] outStream = System.Text.Encoding.ASCII.GetBytes(s + "$");
            if (!(outStream.Length == 1))
            {
                try
                {
                    serverStream.Write(outStream, 0, outStream.Length);
                    serverStream.Flush();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
        }

        private void button2_Click()
        {
            readData = "Conected to Chat Server ...";
            msg();
            //String s = Console.ReadLine();
            clientSocket.Connect("127.0.0.1", 12149);
            serverStream = clientSocket.GetStream();

            //byte[] outStream = System.Text.Encoding.ASCII.GetBytes(s + "$");
            //serverStream.Write(outStream, 0, outStream.Length);
            //serverStream.Flush();

            ctThread = new Thread(getMessage);
            ctThread.Start();
            ctThread.IsBackground = true;
            connectedToServer = true;
        }

        private void getMessage()
        {
            int read = 1;
            while (read > 0 && connectedToServer && clientSocket.Connected == true)
            {
                try
                {
                    serverStream = clientSocket.GetStream();

                    int buffSize = 0;
                    byte[] inStream = new byte[10025];
                    buffSize = clientSocket.ReceiveBufferSize;
                    read = serverStream.Read(inStream, 0, 10025);
                    if (read <= 0)
                    {
                        connectedToServer = false;
                        break;
                    }
                    string returndata = System.Text.Encoding.ASCII.GetString(inStream);
                    returndata = returndata.Substring(0, returndata.IndexOf("\0"));
                    readData = "" + returndata;
                    msg();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
        }

        private void msg()
        {
            //if (this.InvokeRequired)
            //    this.Invoke(new MethodInvoker(msg));
            //else
            Console.WriteLine(readData);
            //textBox1.Text = textBox1.Text + Environment.NewLine + " >> " + readData;
        }

        protected static void myHandler(object sender, ConsoleCancelEventArgs args)
        {
            if (ctThread.IsAlive)
            {
                ctThread.Abort();
            }
            clientSocket.Close();
            // Set the Cancel property to true to prevent the process from terminating.
            //Console.WriteLine("Setting the Cancel property to true...");

            //args.Cancel = true;
            stillAlive = false;

            // Announce the new value of the Cancel property.
            //Console.WriteLine("  Cancel property: {0}", args.Cancel);
            //Console.WriteLine("The read operation will resume...\n");
        }

    }
}