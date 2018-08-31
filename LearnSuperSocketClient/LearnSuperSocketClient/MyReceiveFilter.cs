using SuperSocket.ProtoBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LearnSuperSocketClient
{
    class MyReceiveFilter : TerminatorReceiveFilter<StringPackageInfo>
    {
        public MyReceiveFilter(): base(Encoding.ASCII.GetBytes("\r\n")) { }

        public override StringPackageInfo ResolvePackage(IBufferStream bufferStream)
        {
            return new StringPackageInfo(bufferStream.ReadString((int)bufferStream.Length, Encoding.ASCII), BasicStringParser.DefaultInstance);
        }
    }
}
