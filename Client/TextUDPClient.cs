using System;
using System.Text;
using System.Threading;
using System.Net;
using System.Net.Sockets;

namespace Client {
    public class TextUDPClient {
        private bool connected = false;
        private IPAddress ipAddress;
        private int port;
        private IPEndPoint endPoint;
        private UdpClient udpClient;
        public delegate void ProcessText(string str);
        private ProcessText onReceive;
        private Thread receivingThread;

        public TextUDPClient(string ip, int port) {
            this.ipAddress = IPAddress.Parse(ip);
            this.port = port;
        }

        public void SendString(string str) {
            if (connected) {
                byte[] data = Encoding.UTF8.GetBytes(str);

                udpClient.Send(data, data.Length, endPoint);
            } else throw new Exception("Not connected");
        }

        private void ReceiveData() {
            while (true) {
                IPEndPoint receiveEndPoint = new IPEndPoint(IPAddress.Any, 0);
                byte[] data = udpClient.Receive(ref receiveEndPoint);
                string str = Encoding.UTF8.GetString(data);

                onReceive(str);
            }
        }

        public void Connect(ProcessText onReceive) {
            if (!connected) {
                this.onReceive = onReceive;

                endPoint = new IPEndPoint(ipAddress, port);
                udpClient = new UdpClient(port);

                receivingThread = new Thread(new ThreadStart(ReceiveData));
                receivingThread.IsBackground = true;
                receivingThread.Start();

                connected = true;
            } else throw new Exception("Already connected");
        }

        public void Disconnect() {
            if (connected) {
                receivingThread.Abort();
                udpClient.Close();

                connected = false;
            } else throw new Exception("Not connected");
        }
    }
}
