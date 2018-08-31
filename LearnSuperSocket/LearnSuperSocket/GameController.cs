using LearnSuperSocket.AppSession;
using SuperSocket.SocketBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class GameController
{
    private static GameController instance;

    public static GameController Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new GameController();
            }
            return instance;
        }
    }

    private List<TelentSession> sessionList = new List<TelentSession>();

    public void AddSession(TelentSession session)
    {
        if (!sessionList.Contains(session))
        {
            sessionList.Add(session);

            SendToAll("new session in : " + session.SessionID);
        }
    }

    public void CloseSession(TelentSession session)
    {
        if (sessionList.Contains(session))
        {
            sessionList.Remove(session);

            SendToAll("session out : " + session.SessionID);
        }
    }

    public void CloseAll()
    {
        lock (sessionList)
        {
            for (int i = sessionList.Count - 1; i >= 0; i--)
            {
                CloseSession(sessionList[i]);
            }
        }
    }

    public void SendToAll(string msg)
    {
        foreach (var val in sessionList)
        {
            val.Send(msg);
        }
    }

    public void SendToSession(TelentSession session, string msg)
    {
        session.Send(msg);
    }
}
