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
        this.head = Head;
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
        BinaryWriter headSw = new BinaryWriter(headMs);

        headSw.Write(data.head);

        MemoryStream bodyMs = new MemoryStream();
        BinaryWriter bodySw = new BinaryWriter(bodyMs);
        bodySw.Write(data.hash);
        bodySw.Write(data.type);
        bodySw.Write(data.state);
        bodySw.Write(data.session);

        bodySw.Write(data.cn);
        bodySw.Write(data.module);
        bodySw.Write(data.cmd);

        if (data.attach != null)
        {
            byte[] attachBytes = ObjectToBytes(data.attach);

            bodySw.Write(attachBytes.Length);
            bodySw.Write(attachBytes);
        }
        else
        {
            bodySw.Write(0);
        }

        if (data.clientAttach != null)
        {
            byte[] clientAttachBytes = ObjectToBytes(data.clientAttach);

            bodySw.Write(clientAttachBytes.Length);
            bodySw.Write(clientAttachBytes);
        }
        else
        {
            bodySw.Write((Int32)0);
        }

        bodySw.Flush();

        headSw.Write((int)bodyMs.Length);
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
        int head =br.ReadInt32();

        if (head != Head)
        {
            return false;
        }

        int packageLength =br.ReadInt32();

        //剩余数据小于packageLength
        if (MemStream.Length - MemStream.Position - 8 < packageLength)
        {
            return false;
        }

        data = new SocketData();
        data.head = head;
        data.packageLength = packageLength;
        data.hash = br.ReadInt32();
        data.type =br.ReadInt32();
        data.state = br.ReadInt32();
        data.session = br.ReadInt32();

        data.cn =br.ReadInt64();
        data.module = br.ReadInt32();
        data.cmd = br.ReadInt32();
        int attachLength = br.ReadInt32();

        if (attachLength > 0)
        {
            data.attach = BytesToObject(br.ReadBytes(attachLength));
        }
        int clientAttachLength = br.ReadInt32();
        if (clientAttachLength > 0)
        {
            data.clientAttach = BytesToObject(br.ReadBytes(clientAttachLength));
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