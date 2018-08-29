using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;

namespace GameServer
{
    class ServerSocket
    {
        private class StateObject
        {
            private const int BUFF_SIZE = 1024;
            public Socket socket;
            public byte[] buff = new byte[BUFF_SIZE];
        }

        public enum ConnectType
        {
            Connect,
            DisConnect,
            Close,
        }

        private Socket m_Socket;

        public string host = "127.0.0.1";
        public int port = 64420;

        public System.Action<ConnectType , Socket> onConnectChangedAction;
        public System.Action<SocketData> onReceiveAction;

        private int acceptAmount = 0;

        public int buffLength
        {
            get;
            private set;
        }

        private static readonly object lockObj = new object();

        private long cn = -1;
        public long CN
        {
            get
            {
                return cn--;
            }
        }

        public void Start()
        {
            buffLength = 0;

            m_Socket = new Socket(AddressFamily.InterNetwork , SocketType.Stream, ProtocolType.Tcp);
            m_Socket.Bind(new IPEndPoint(IPAddress.Parse(host), port));
            m_Socket.Listen(0);
            m_Socket.BeginAccept(new AsyncCallback(Accept), m_Socket);

            Console.WriteLine("server start listen : ip :" + host + " , port : " + port);
        }

        private void Accept(IAsyncResult ar)
        {
            Socket server = ar.AsyncState as Socket;
            Socket client = server.EndAccept(ar);

            if (onConnectChangedAction != null)
            {
                onConnectChangedAction(ConnectType.Connect, client);
            }
            Console.WriteLine("client accept : " + client.RemoteEndPoint);

            SocketData send = new SocketData();
            send.SetData(CN, 0, -1, "server said : hello" , null);

            SendMsg(client,SocketData.SerializeData(send));

            StateObject state = new StateObject() { socket = client };

            server.BeginAccept(new AsyncCallback(Accept), server);

            client.BeginReceive(state.buff, 0, state.buff.Length, SocketFlags.None, new AsyncCallback(OnRead), state);
        }

        private void OnRead(IAsyncResult ar)
        {
            StateObject state = ar.AsyncState as StateObject;
            Socket client = state.socket;

            int byteRead = client.EndReceive(ar);

            //包尺寸有问题，断线处理
            if (byteRead < 1)
            {
                if (onConnectChangedAction != null)
                {
                    onConnectChangedAction(ConnectType.DisConnect, client);
                }
                Console.WriteLine("client disconned!" + client.RemoteEndPoint);
                return;
            }

            lock (lockObj)
            {
                SocketData data;
                if (SocketData.DeserializeData(state.buff, byteRead, out data))
                {
                    Console.WriteLine("read : "+ data.cn +"_" + data.module + "_" + data.cmd + " : " + data.attach.ToString());
                    if (onReceiveAction != null)
                    {
                        onReceiveAction(data);
                    }
                }
                Array.Clear(state.buff, 0, state.buff.Length);
            }
            client.BeginReceive(state.buff, 0, state.buff.Length, SocketFlags.None, new AsyncCallback(OnRead), state);
        }

        public void SendMsg(Socket handle, byte[] msg)
        {
            handle.BeginSend(msg, 0, msg.Length, SocketFlags.None, new AsyncCallback(Send), handle);
        }

        private void Send(IAsyncResult ar)
        {
            Socket socket = ar.AsyncState as Socket;
            socket.EndSend(ar);
        }

        public void Close()
        {
            if (this.m_Socket != null)
            {
                Console.WriteLine("server close");
                m_Socket.Close();
            }
        }
    }
}
