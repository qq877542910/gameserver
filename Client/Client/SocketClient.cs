using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using System;
using System.Text;
using System.IO;

public class SocketClient 
{
    TcpClient client;

    public string host = "127.0.0.1";
    public int port = 64420;
    public bool IsConnect = false;

    NetworkStream outStream;
    NetworkStream getStream;

    private const int Max_SIZE = 1024;
    private byte[] buff = new byte[Max_SIZE];

    private long cn = 0;

    public long CN
    {
        get
        {
            return cn++;
        }
    }

    public void StartConnect()
    {
        client = new TcpClient(AddressFamily.InterNetwork);
        client.SendTimeout = 1000;
        client.ReceiveTimeout = 1000;
        client.NoDelay = true;

        if (client.Connected)
        {
            Console.WriteLine("client alreay connected");
        }
        try
        {
            Console.WriteLine("client start connect : ip : " + host + " , port : " + port);
            client.BeginConnect(host, port, new AsyncCallback(Connect), client);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            this.Close();
        }

    }

    private void Connect(IAsyncResult ar)
    {
        if (!client.Connected)
        {
            Console.WriteLine("connect have error");
            this.Close();
            return;
        }
        Console.WriteLine("client connect server! " + client.Client.LocalEndPoint);

        outStream = client.GetStream();
        client.GetStream().BeginRead(buff, 0, buff.Length,new AsyncCallback(OnRead) ,null);

       // SocketData data = new SocketData();
        //data.SetData(CN, 0, 1, "client said : hello ", null);
        //WriteMsg(SocketData.SerializeData(data));
    }

    private void OnRead(IAsyncResult ar)
    {
        int byteRead = 0;
        try
        {
            lock (client.GetStream())
            {
                byteRead = client.GetStream().EndRead(ar);
            }

            if (byteRead < 1)
            {
                Console.WriteLine("connect is lost");
                this.Close();
                return;
            }

            SocketData data;

            if (SocketData.DeserializeData(buff, byteRead, out data))
            {
                Console.WriteLine("read : " + data.cn +"_" + data.module + "_" + data.cmd + " : " + data.attach.ToString());
            }

            lock (client.GetStream())
            {
                Array.Clear(buff, 0, buff.Length);
                client.GetStream().BeginRead(buff, 0, buff.Length, new AsyncCallback(OnRead), null);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);

            this.Close();
        }
    }

    public void WriteMsg(byte[] msg)
    {
        MemoryStream ms = null;
        using (ms = new MemoryStream())
        {
            if (client != null && client.Connected)
            {
                ms.Position = 0;
                outStream.BeginWrite(msg, 0, msg.Length, new AsyncCallback(OnWrite), null);
            }
            else
            {
                Console.WriteLine("client.connected ------->> false");
            }
        }
    }

    private void OnWrite(IAsyncResult ar)
    {
        Socket socket = ar.AsyncState as Socket;
        int length = socket.EndSend(ar);

        Console.WriteLine("send over length : " + length);
    }

    public void Close()
    {
        if (client != null && client.Connected)
        {
            Console.WriteLine("socket close");
            client.Close();
        }
        client = null;

    }

    ~SocketClient()
    {
        Close();
    }
}
