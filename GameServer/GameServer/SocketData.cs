using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
class SocketData
{
    private static readonly int Head = -1;
    private static readonly int HeadLength = 4;
    private static readonly int PackageMinLength = 8;
    private static MemoryStream MemStream = new MemoryStream();

    public int head;
    public int packageLength;
    public int hash;
    public int type;
    public int state;
    public int session;

    public long cn;
    public int module;
    public int cmd;
    public object attach;
    public object clientAttach;

    public void SetData(long cn, int module, int cmd, object attach, object clientAttach)
    {
        this.cn = cn;
        this.module = module;
        this.cmd = cmd;
        this.attach = attach;
        this.clientAttach = clientAttach;
    }

    public static byte[] SerializeData(SocketData data)
    {
        byte[] buff;

        MemoryStream headMs = new MemoryStream();
        StreamWriter headSw = new StreamWriter(headMs);

        headSw.Write(Converter.GetBigEndian(data.head));

        MemoryStream bodyMs = new MemoryStream();
        StreamWriter bodySw = new StreamWriter(bodyMs);
        bodySw.Write(Converter.GetBigEndian(data.hash));
        bodySw.Write(Converter.GetBigEndian(data.type));
        bodySw.Write(Converter.GetBigEndian(data.state));
        bodySw.Write(Converter.GetBigEndian(data.session));

        bodySw.Write(Converter.GetBigEndian(data.cn));
        bodySw.Write(Converter.GetBigEndian(data.module));
        bodySw.Write(Converter.GetBigEndian(data.cmd));

        if (data.attach != null)
        {
            byte[] attachBytes = ObjectToBytes(data.attach);

            bodySw.Write(Converter.GetBigEndian(attachBytes.Length));
            bodySw.Write(attachBytes);
        }
        else
        {
            bodySw.Write(Converter.GetBigEndian((Int32)0));
        }

        if (data.clientAttach != null)
        {
            byte[] clientAttachBytes = ObjectToBytes(data.clientAttach);

            bodySw.Write(Converter.GetBigEndian(clientAttachBytes.Length));
            bodySw.Write(clientAttachBytes);
        }
        else
        {
            bodySw.Write(Converter.GetBigEndian((Int32)0));
        }

        bodySw.Flush();

        headSw.Write(Converter.GetBigEndian(bodyMs.Length));
        headSw.Write(bodyMs.GetBuffer());

        bodySw.Close();
        bodyMs.Close();

        headSw.Flush();
        buff = headMs.GetBuffer();

        headSw.Close();
        headMs.Close();

        return buff;
    }

    public static bool DeserializeData(byte[] buff, int length, out SocketData data)
    {

        data = null;
        MemStream.Seek(0, SeekOrigin.End);
        MemStream.Write(buff, 0, buff.Length);
        MemStream.Seek(0, SeekOrigin.Begin);

        if (MemStream.Length < HeadLength || MemStream.Length < PackageMinLength)
        {
            return false;
        }

        BinaryReader br = new BinaryReader(MemStream);
        int head =Converter.GetBigEndian( br.ReadInt32());

        if (head != Head)
        {
            return false;
        }

        int packageLength = Converter.GetBigEndian(br.ReadInt32());

        //剩余数据小于packageLength
        if (MemStream.Length - MemStream.Position - 8 < packageLength)
        {
            return false;
        }

        data = new SocketData();
        data.head = head;
        data.packageLength = packageLength;
        data.hash = Converter.GetBigEndian(br.ReadInt32());
        data.type = Converter.GetBigEndian(br.ReadInt32());
        data.state = Converter.GetBigEndian(br.ReadInt32());
        data.session = Converter.GetBigEndian(br.ReadInt32());

        data.cn = Converter.GetBigEndian(br.ReadInt64());
        data.module = Converter.GetBigEndian(br.ReadInt32());
        data.cmd = Converter.GetBigEndian(br.ReadInt32());
        int attachLength = Converter.GetBigEndian(br.ReadInt32());

        if (attachLength > 0)
        {
            data.attach = br.ReadBytes(attachLength);
        }
        int clientAttachLength = Converter.GetBigEndian(br.ReadInt32());
        if (clientAttachLength > 0)
        {
            data.clientAttach = br.ReadBytes(clientAttachLength);
        }

        //移除已经读取的数据,用剩余数据重新填充ms
        byte[] leftover = br.ReadBytes((int)(MemStream.Length - MemStream.Position));
        MemStream.SetLength(0);
        MemStream.Write(leftover, 0, leftover.Length);

        return true;
    }

    static int GetHash(SocketData data)
    {
        return 0;
    }

    static byte[] ObjectToBytes(object obj)
    {
        byte[] buff;

        using (MemoryStream ms = new MemoryStream())
        {
            IFormatter iformatter = new BinaryFormatter();
            iformatter.Serialize(ms, obj);
            buff = ms.GetBuffer();
        }
        return buff;
    }

    static object BytesToObject(byte[] buff)
    {
        object obj;
        using (MemoryStream ms = new MemoryStream(buff))
        {
            IFormatter iformatter = new BinaryFormatter();
            obj = iformatter.Deserialize(ms);
        }
        return obj;
    }
}