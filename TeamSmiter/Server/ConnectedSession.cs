using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lidgren.Network;

namespace TeamSmiter
{
    public class ConnectedSession
    {
        public ConnectedSession(string name, NetConnection p)
        {
            Username = name;
            Connection = p;
            Authority = 0;
        }

        public string Username { get; set; }
        public NetConnection Connection { get; set; }
        public byte Authority { get; set; }

        public ConnectedSession Container { get; set; }
    }
}
