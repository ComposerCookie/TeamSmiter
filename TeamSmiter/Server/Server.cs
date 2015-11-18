using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lidgren.Network;
using System.Windows.Forms;
using SFML.Graphics;
using SFML.Window;

namespace TeamSmiter
{
    public class Server
    {
        // Attribute
        NetPeerConfiguration _config;
        NetServer _server;
        SessionManager sessionManager;
        VertexArray vertex;

        public Server(int port, string pass, string admin)
        {
            Init(port, pass, admin);
            sessionManager = new SessionManager();
            vertex = new VertexArray(PrimitiveType.TrianglesStrip);
        }

        public bool SuccessFullyStarted { get; set; }
        public string Password { get; set; }
        public string AdminName { get; set; }

        // Core Functions

        public void Init(int port, string pass, string admin)
        {
            _config = new NetPeerConfiguration("TeamSmiterxV01");
            _config.MaximumConnections = 25;
            _config.Port = port;
            _server = new NetServer(_config);
            Password = pass;
            AdminName = admin;
            
        }

        public void StartServer(int port, string pass, string admin)
        {
            _config.Port = port;
            Password = pass;
            AdminName = admin;
            sessionManager.SessionList[0].Username = AdminName;
            _server.Start();
            SuccessFullyStarted = true;
            MainViewer.Instance.IsServer = true;
            MainViewer.Instance.EnableConnected();
            WriteText("Server had started...");
            UpdateConnectionsList();
        }

        public void Shutdown(string msg)
        {
            _server.Shutdown("Server Shutting Down (" + msg + ")");
        }

        // Update Connections List every time receive new connection

        private void UpdateConnectionsList()
        {
            MainViewer.Instance.ServerUpdateOnlineList(sessionManager.SessionList);
        }

        // Write Logs

        public void WriteText(string text)
        {
            MainViewer.Instance.WriteLog(text, LogType.ServerLogin);
            MainViewer.Instance.WriteLog(text, LogType.Main);
        }

        // Handle Data

        public void HandleIncomingData()
        {
            NetIncomingMessage im;
            string temp = "";
            while ((im = _server.ReadMessage()) != null)
            {
                // handle incoming message
                switch (im.MessageType)
                {
                    case NetIncomingMessageType.DebugMessage:
                    case NetIncomingMessageType.ErrorMessage:
                    case NetIncomingMessageType.WarningMessage:
                    case NetIncomingMessageType.VerboseDebugMessage:
                        string text = im.ReadString();
                        WriteText(text);
                        break;
                    case NetIncomingMessageType.StatusChanged:
                        NetConnectionStatus status = (NetConnectionStatus)im.ReadByte();
                        string reason = im.ReadString();
                        //WriteText(NetUtility.ToHexString(im.SenderConnection.RemoteUniqueIdentifier) + " " + status + ": " + reason);

                        if (status == NetConnectionStatus.Connected)
                        {
                            WriteText("Received Connection from: " + im.SenderConnection.RemoteEndPoint.Address.ToString());
                            SendConnectionReceived(im.SenderConnection);
                            WriteText("Checking for proper password...");
                            SendRequestLoginInformation(im.SenderConnection);
                        }
                        
                        UpdateConnectionsList();
                        break;
                    case NetIncomingMessageType.Data:
                        int dataType = im.ReadVariableInt32();
                        switch ((ClientSendType)dataType)
                        {
                            case ClientSendType.ClientSendLoginInformation:
                                if (!im.ReadString().Equals(Password))
                                {
                                    im.SenderConnection.Disconnect("Wrong password");
                                    WriteText("Disconnected connection from: " + im.SenderConnection.RemoteEndPoint.Address.ToString() + " for wrong password");
                                    return;
                                }
                                temp = im.ReadString();
                                if (!(sessionManager.SessionList.Find(ConnectedSession => ConnectedSession.Username == temp) == null))
                                {
                                    im.SenderConnection.Disconnect("Username already taken");
                                    WriteText("Disconnected connection from: " + im.SenderConnection.RemoteEndPoint.Address.ToString() + " for already exist username");
                                    return;
                                }
                                SendApprovedConnection(im.SenderConnection);
                                WriteText("Connection from: " + im.SenderConnection.RemoteEndPoint.Address.ToString() + "(Nick: " + temp + ") had been approved");
                                sessionManager.addConnection(temp, im.SenderConnection);
                                UpdateConnectionsList();
                                SendNewConnection(im.SenderConnection);
                                MainViewer.Instance.UpdateWhiteboardData();
                                //SendWhiteboardDrawnBoard(im.SenderConnection, MainViewer.Instance.getWhiteboardDataX(), MainViewer.Instance.getWhiteboardDataY(), MainViewer.Instance.getWhiteboardDataPixel());
                                SendWhiteboardDrawnBoard(im.SenderConnection, MainViewer.Instance.CurrentMap, MainViewer.Instance.getWhiteboard());
                                break;
                            case ClientSendType.ClientSendChatMessage:
                                temp = im.ReadString();
                                WriteText(sessionManager.SessionList.Find(ConnectedSession => ConnectedSession.Connection == im.SenderConnection).Username + ": " + temp);
                                SendBroadcastChatMessage(sessionManager.SessionList.Find(ConnectedSession => ConnectedSession.Connection == im.SenderConnection).Username + ": " + temp);
                                break;
                            case ClientSendType.ClientSendWhiteboardDrawnSomething:
                                vertex.Clear();
                                /*byte tooltemp = im.ReadByte();
                                for (int i = 0; i < im.ReadVariableUInt32(); i++)
                                {
                                    vertex.Append(new Vertex(new Vector2f(im.ReadFloat(), im.ReadFloat()), new Color(im.ReadByte(), im.ReadByte(), im.ReadByte(), im.ReadByte())));
                                }
                                MainViewer.Instance.ReceiveDrawnSomething(vertex, tooltemp);*/
                                byte tooltemp = im.ReadByte();
                                uint test1 = im.ReadVariableUInt32();
                                int x; int y; byte r; byte g; byte b; byte a;
                                for (int i = 0; i < test1; i++)
                                {
                                    
                                    //x = im.ReadFloat(); y = im.ReadFloat();
                                    if (i > 2)
                                    {
                                        x = im.ReadVariableInt32();
                                        y = im.ReadVariableInt32();
                                        r = im.ReadByte(); g = im.ReadByte(); b = im.ReadByte(); a = im.ReadByte();
                                        vertex.Append(new Vertex(new Vector2f(x, y), new Color(r, g, b, a)));
                                    }
                                    else
                                    {
                                        x = im.ReadVariableInt32();
                                        y = im.ReadVariableInt32();
                                        r = im.ReadByte(); g = im.ReadByte(); b = im.ReadByte(); a = im.ReadByte();
                                    }
                                }
                                MainViewer.Instance.ReceiveDrawnSomething(vertex, tooltemp);
                                SendWhiteboarDrawnSomething(vertex, tooltemp);
                                break;
                        }


                    /*    
                    // incoming chat message from a client
                        string chat = im.ReadString();
                        WriteText("Broadcasting '" + chat + "'");
                        // broadcast this to all connections, except sender
                        List<NetConnection> all = _server.Connections; // get copy
                        all.Remove(im.SenderConnection);
                        if (all.Count > 0)
                        {
                            NetOutgoingMessage om = _server.CreateMessage();
                            om.Write(NetUtility.ToHexString(im.SenderConnection.RemoteUniqueIdentifier) + " said: " + chat);
                            _server.SendMessage(om, all, NetDeliveryMethod.ReliableOrdered, 0);
                        }
                     * */
                        break;
                    default:
                        WriteText("Unhandled type: " + im.MessageType + " " + im.LengthBytes + " bytes " + im.DeliveryMethod + "|" + im.SequenceChannel);
                        break;
                }
                _server.Recycle(im);
            }
        }

        // Send Information

        public void SendConnectionReceived(NetConnection p)
        {
            NetOutgoingMessage om = _server.CreateMessage();
            om.WriteVariableInt32((int)ServerSendType.SendReceivedConnection);
            _server.SendMessage(om, p, NetDeliveryMethod.ReliableOrdered, 0);
        }

        public void SendRequestLoginInformation(NetConnection p)
        {
            NetOutgoingMessage om = _server.CreateMessage();
            om.WriteVariableInt32((int)ServerSendType.SendRequestPassword);
            _server.SendMessage(om, p, NetDeliveryMethod.ReliableOrdered, 0);
        }

        public void SendApprovedConnection(NetConnection p)
        {
            NetOutgoingMessage om = _server.CreateMessage();
            om.WriteVariableInt32((int)ServerSendType.SendApprovedConnection);
            _server.SendMessage(om, p, NetDeliveryMethod.ReliableOrdered, 0);
        }

        public void SendNewConnection(NetConnection p)
        {
            NetOutgoingMessage om = _server.CreateMessage();
            om.WriteVariableInt32((int)ServerSendType.SendNewConnection);
            om.Write(sessionManager.SessionList.Find(ConnectedSession => ConnectedSession.Connection == p).Username);
            om.WriteVariableInt32(sessionManager.SessionList.Count);
            for (int i = 0; i < sessionManager.SessionList.Count; i++)
            {
                om.Write(sessionManager.SessionList[i].Username);
            }
            //_server.SendToAll(om, NetDeliveryMethod.ReliableOrdered);
            _server.SendMessage(om, _server.Connections, NetDeliveryMethod.ReliableOrdered, 0);
        }

        public void SendBroadcastChatMessage(string text)
        {
            if (_server.ConnectionsCount < 1)
                return;
            NetOutgoingMessage om = _server.CreateMessage();
            om.WriteVariableInt32((int)ServerSendType.SendBroadcastChatMessage);
            om.Write(text);
            //_server.SendToAll(om, NetDeliveryMethod.ReliableOrdered);
            _server.SendMessage(om, _server.Connections, NetDeliveryMethod.ReliableOrdered, 0);
        }

        public void SendLoadMap(int map)
        {
            if (_server.ConnectionsCount < 1)
                return;
            NetOutgoingMessage om = _server.CreateMessage();
            om.WriteVariableInt32((int)ServerSendType.SendLoadMap);
            om.WriteVariableInt32(map);
            //_server.SendToAll(om, NetDeliveryMethod.ReliableOrdered);
            _server.SendMessage(om, _server.Connections, NetDeliveryMethod.ReliableOrdered, 0);
        }

        public void SendGodMove(int team, int id, int x, int y)
        {
            if (_server.ConnectionsCount < 1)
                return;
            NetOutgoingMessage om = _server.CreateMessage();
            om.WriteVariableInt32((int)ServerSendType.SendGodMoved);
            om.WriteVariableInt32(team);
            om.WriteVariableInt32(id);
            om.WriteVariableInt32(x);
            om.WriteVariableInt32(y);
           
            //_server.SendToAll(om, NetDeliveryMethod.ReliableOrdered);
            _server.SendMessage(om, _server.Connections, NetDeliveryMethod.ReliableOrdered, 0);
        }

        public void SendUpdateTeam(List<GodInstance> a, List<GodInstance> b)
        {
            if (_server.ConnectionsCount < 1)
                return;
            NetOutgoingMessage om = _server.CreateMessage();
            om.WriteVariableInt32((int)ServerSendType.SendTeamUpdated);
            om.WriteVariableInt32(a.Count);
            for (int i = 0; i < a.Count; i++)
            {
                om.WriteVariableInt32(a[i].ID);
                om.WriteVariableInt32(a[i].X);
                om.WriteVariableInt32(a[i].Y);
                om.Write(a[i].Visible);
            }

            om.WriteVariableInt32(b.Count);
            for (int i = 0; i < b.Count; i++)
            {
                om.WriteVariableInt32(b[i].ID);
                om.WriteVariableInt32(b[i].X);
                om.WriteVariableInt32(b[i].Y);
                om.Write(b[i].Visible);
            }
            //_server.SendToAll(om, NetDeliveryMethod.ReliableOrdered);
            _server.SendMessage(om, _server.Connections, NetDeliveryMethod.ReliableOrdered, 0);
        }

        //public void SendWhiteboardDrawnBoard(NetConnection p, List<int> x, List<int> y, List<byte> pixel)
        public void SendWhiteboardDrawnBoard(NetConnection p, int curmap, byte[] pix)
        {
            NetOutgoingMessage om = _server.CreateMessage();
            om.WriteVariableInt32((int)ServerSendType.SendWhiteboardSendBoard);
            om.WriteVariableInt32(curmap);
            om.WriteVariableInt32(pix.Length);
            om.Write(pix);
            /*om.WriteVariableInt32(x.Count);
            for (int i = 0; i < x.Count; i++)
            {
                om.WriteVariableInt32(x[i]);
                om.WriteVariableInt32(y[i]);
            }
            for (int i = 0; i < pixel.Count; i++)
                om.Write(pixel[i]);

             * */
            _server.SendMessage(om, p, NetDeliveryMethod.ReliableOrdered, 0);
            WriteText("Sending whiteboard information to " + sessionManager.SessionList.Find(ConnectedSession => ConnectedSession.Connection == p).Username + "...");
            
        }

        public void SendWhiteboarDrawnSomething(VertexArray v, byte tool) // This right here my friend.
        {
            if (_server.Connections.Count <= 0)
                return;

            NetOutgoingMessage om = _server.CreateMessage();
            om.WriteVariableInt32((int)ServerSendType.SendWhiteboardDrawnSomething);
            om.Write(tool);
            om.Write(v.VertexCount + 3);
			for (int i = 0; i < v.VertexCount + 3; i++)
			{
                if (i > 2)
                {
                    int testA = (int)v[(uint)i - 3].Position.X;
                    int testB = (int)v[(uint)i - 3].Position.Y;
                    Console.WriteLine("server: index: " + (i - 3) + " x: " + testA + " y: " + testB);
                    om.WriteVariableInt32(testA);
                    om.WriteVariableInt32(testB);
                    om.Write(v[(uint)i - 3].Color.R);
                    om.Write(v[(uint)i - 3].Color.G);
                    om.Write(v[(uint)i - 3].Color.B);
                    om.Write(v[(uint)i - 3].Color.A);
                }
                else
                {
                    int testA = (int)v[(uint)i].Position.X;
                    int testB = (int)v[(uint)i].Position.Y;
                    Console.WriteLine("server: index: " + i + " x: " + testA + " y: " + testB);
                    om.WriteVariableInt32(testA);
                    om.WriteVariableInt32(testB);
                    om.Write(v[(uint)i].Color.R);
                    om.Write(v[(uint)i].Color.G);
                    om.Write(v[(uint)i].Color.B);
                    om.Write(v[(uint)i].Color.A);
                }
                
			}

            _server.SendMessage(om, _server.Connections, NetDeliveryMethod.ReliableOrdered, 0);
        }

        public void SendWhiteboardGivePen(ConnectedSession p)
        {
            NetOutgoingMessage om = _server.CreateMessage();
            om.WriteVariableInt32((int)ServerSendType.SendWhiteboardGivePen);
            _server.SendMessage(om, p.Connection, NetDeliveryMethod.ReliableOrdered, 0);
            WriteText("Granted " + p.Username + " pen privilege");
        }

        public void SendWhiteboardTakePen(ConnectedSession p)
        {
            NetOutgoingMessage om = _server.CreateMessage();
            om.WriteVariableInt32((int)ServerSendType.SendWhiteboardTakePen);
            _server.SendMessage(om, p.Connection, NetDeliveryMethod.ReliableOrdered, 0);
            WriteText("Revoked " + p.Username + "'s pen privilege");
        }

    }
}
