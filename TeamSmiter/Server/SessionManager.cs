using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lidgren.Network;

namespace TeamSmiter
{
    public class SessionManager
    {
        public SessionManager()
        {
            SessionList = new List<ConnectedSession>();
            SessionList.Add(new ConnectedSession(MainViewer.Instance.AdminName, null));
        }

        public List<ConnectedSession> SessionList { get; set; }

        public void addConnection(string name, NetConnection p)
        {
            SessionList.Add(new ConnectedSession(name, p));
        }

        public void removeConnection(NetConnection p)
        {
            SessionList.Remove(SessionList.Find(ConnectedSession => ConnectedSession.Connection == p));
        }

    }
}
