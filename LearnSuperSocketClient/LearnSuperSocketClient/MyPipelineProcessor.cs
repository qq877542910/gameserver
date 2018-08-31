using SuperSocket.ProtoBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LearnSuperSocketClient
{
    class MyPipelineProcessor : IPipelineProcessor
    {
        private BufferList bfList = new BufferList();

        BufferList IPipelineProcessor.Cache
        {
            get
            {
                return bfList;
            }
        }

        ProcessResult IPipelineProcessor.Process(ArraySegment<byte> segment)
        {
            ProcessResult pr = new ProcessResult();
            pr.Packages.Add(new MyPackageInfo() { bytes = segment.ToArray() });

            return pr;
        }

        void IPipelineProcessor.Reset()
        {
        }
    }
}
