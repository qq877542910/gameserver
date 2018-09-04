
using LearnSuperSocket.AppSession;
using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Command;
using SuperSocket.SocketBase.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LearnSuperSocket.Command
{
    public class Help : CommandBase<TelentSession, StringRequestInfo>
    {
        private List<string> cmdStrList = new List<string>()
        {
            "USER   login ,logout , create user",
            "CMD    cmd...121",
        };

        public override void ExecuteCommand(TelentSession session, StringRequestInfo requestInfo)
        {
            session.Send(GetStrsByList(cmdStrList));
        }

        private string GetStrsByList(List<string> list)
        {
            string str = "\r\n Command :\r\n\r\n";

            for (int i = 0; i < list.Count; i++)
            {
                str +=" " + list[i] + "\r\n";
            }
            return str;
        }
    }
}
