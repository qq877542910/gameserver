using SuperSocket.ClientEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SuperSocket.ProtoBase;

namespace LearnSuperSocketClient
{
    class MyClient : EasyClient
    {
        protected override void HandlePackage(IPackageInfo package)
        {
            base.HandlePackage(package);
        }
    }
}
