using LearnSuperSocket.AppSession;
using SuperSocket.SocketBase.Command;
using SuperSocket.SocketBase.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LearnSuperSocket.Command
{
    public class CreateUser : CommandBase<TelentSession, StringRequestInfo>
    {

        public override void ExecuteCommand(TelentSession session, StringRequestInfo requestInfo)
        {
            if (requestInfo.Parameters.Length != 3)
            {
                session.Send("cmd error!");
            }
            string userName = requestInfo.Body;
            string userPassward1 = requestInfo.Parameters[1];
            string userPassward2 = requestInfo.Parameters[2];

            if (userPassward1 != userPassward2)
            {
                session.Send("The two passwords are different");
            }
        }
    }
}
