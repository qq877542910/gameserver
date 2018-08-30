using SuperSocket.SocketBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SuperSocket.SocketBase.Protocol;

namespace AppSession
{
    class TelentSession : AppSession<TelentSession>
    {
        protected override void OnSessionStarted()
        {
            base.OnSessionStarted();

            GameController.Instance.AddSession(this);
        }

        protected override void HandleUnknownRequest(StringRequestInfo requestInfo)
        {
            base.HandleUnknownRequest(requestInfo);
        }

        protected override void HandleException(Exception e)
        {
            GameController.Instance.CloseSession(this);
            base.HandleException(e);
        }

        protected override void OnSessionClosed(CloseReason reason)
        {
            GameController.Instance.CloseSession(this);
            base.OnSessionClosed(reason);
        }
    }
}
