using SuperSocket.SocketBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LearnSuperSocket.AppSession
{
    public class GameSession : AppSession<GameSession>
    {
        protected override void OnSessionStarted()
        {
            base.OnSessionStarted();

            this.Send("you are in gameServer");
        }
    }
}
