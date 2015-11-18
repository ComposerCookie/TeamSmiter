using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lidgren.Network;
using SFML.Graphics;
using SFML.Window;
using System.Threading;

namespace TeamSmiter
{
    public class Client
    {
        // Attribute

        NetClient _client;
        NetPeerConfiguration _config;
        bool _successfullyStarted;
        string _password;
        VertexArray vertex;
        uint count1;
        uint count2;
        int count;
        int curMap;
        int temp1;
        int temp2;
        int temp3;
        byte[] whiteboard;
        GodInstance god;
        List<GodInstance> listOfGod;

        public Client(string address, int port)
        {
            vertex = new VertexArray(PrimitiveType.TrianglesStrip);
            Init(address, port);
        }

        // Starting up Methods

        public void Init(string address, int port)
        {
            // Setting up setting for connections.
            _config = new NetPeerConfiguration("TeamSmiterxV01");
            _config.UseMessageRecycling = false;
            _config.AutoFlushSendQueue = false;
            
            
           // Create the Client
            _client = new NetClient(_config);
            _client.RegisterReceivedCallback(new SendOrPostCallback(HandleIncomingData));
            _client.Shutdown("Bye");
        }

        public void Connect(string address, int port, string pass)
        {
            // Store the password for when the server ask
            _password = pass;

            // Start the client
            _client.Start();

            // Connect with a come in message
            NetOutgoingMessage hail = _client.CreateMessage("Scotty, Beam me up");
            _client.Connect(address, port, hail);
        }

        public void Shutdown(string reason)
        {
            // Shut down the server
            _client.Disconnect(reason);
            // s_client.Shutdown("Requested by user");
        }

        // Update Logs

        private void WriteText(string text)
        {
            MainViewer.Instance.WriteLog(text, LogType.ClientLogin);
            MainViewer.Instance.WriteLog(text, LogType.Main);
        }

        // Handle Server Data

        public void HandleIncomingData(object peer)
        {
            NetIncomingMessage im;
            while ((im = _client.ReadMessage()) != null)
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
                        if (status == NetConnectionStatus.Connected)
                        {
                        }

                        if (status == NetConnectionStatus.Disconnected)
                        {
                            MainViewer.Instance.EnableLoginCommand();
                            MainViewer.Instance.DisableConnected();
                        }

                        string reason = im.ReadString();
                        WriteText(status.ToString() + ": " + reason);

                        break;
                    case NetIncomingMessageType.Data:
                        //Check if receive asking for information from Server:
                        int serverData = im.ReadVariableInt32();
                        switch ((ServerSendType)serverData)
                        {
                            case ServerSendType.SendReceivedConnection:
                                WriteText("Server had received your connection requestion");
                                break;
                            case ServerSendType.SendRequestPassword:
                                WriteText("Server is now requesting the password...");
                                SendLoginInformation();
                                break;
                            case ServerSendType.SendApprovedConnection:
                                WriteText("You are now connected");
                                MainViewer.Instance.EnableConnected();
                                break;
                            case ServerSendType.SendNewConnection:
                                WriteText(im.ReadString() + " has connected");
                                string[] updateConnection = new string[im.ReadVariableInt32()];
                                for (int i = 0; i < updateConnection.Length; i++)
                                {
                                    updateConnection[i] = im.ReadString();
                                }
                                MainViewer.Instance.ClientUpdateOnlineLog(updateConnection);
                                break;
                            case ServerSendType.SendBroadcastChatMessage:
                                WriteText(im.ReadString());
                                break;
                            case ServerSendType.SendWhiteboardDrawnSomething:
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
                                        Console.WriteLine("client: index: " + (i - 3) + " x: " + x + " y: " + y);
                                        r = im.ReadByte(); g = im.ReadByte(); b = im.ReadByte(); a = im.ReadByte();
                                        vertex.Append(new Vertex(new Vector2f(x, y), new Color(r, g, b, a)));
                                        Console.WriteLine("" + r + " " + g + " " + b + " " + b);
                                        //Console.WriteLine("client: " + r + " " + g + " " + b);
                                    }
                                    else
                                    {
                                        x = im.ReadVariableInt32();
                                        y = im.ReadVariableInt32();
                                        Console.WriteLine("client: index: " + (i - 3) + " x: " + x + " y: " + y);
                                        r = im.ReadByte(); g = im.ReadByte(); b = im.ReadByte(); a = im.ReadByte();
                                        //vertex.Append(new Vertex(new Vector2f(x, y), new Color(r, g, b, a)));
                                        Console.WriteLine("" + r + " " + g + " " + b + " " + b);
                                        //Console.WriteLine("client: " + r + " " + g + " " + b);
                                    }
                                }
                                //im.ReadVariableInt32();
                                MainViewer.Instance.ReceiveDrawnSomething(vertex, tooltemp);
                                break;
                            case ServerSendType.SendLoadMap:
                                curMap = im.ReadVariableInt32();
                                MainViewer.Instance.LoadNewMap(curMap);
                                break;
                            case ServerSendType.SendTeamUpdated:
                                count = im.ReadVariableInt32();
                                listOfGod = new List<GodInstance>();
                                for (int i = 0; i < count; i++)
                                {
                                    temp1 = im.ReadVariableInt32();
                                    temp2 = im.ReadVariableInt32();
                                    temp3 = im.ReadVariableInt32();
                                    god = new GodInstance(temp2, temp3, temp1);
                                    god.Visible = im.ReadBoolean();

                                    listOfGod.Add(god);
                                }
                                MainViewer.Instance.BoardTeam1 = listOfGod;

                                count = im.ReadVariableInt32();
                                listOfGod = new List<GodInstance>();
                                for (int i = 0; i < count; i++)
                                {
                                    temp1 = im.ReadVariableInt32();
                                    temp2 = im.ReadVariableInt32();
                                    temp3 = im.ReadVariableInt32();
                                    god = new GodInstance(temp2, temp3, temp1);
                                    god.Visible = im.ReadBoolean();

                                    listOfGod.Add(god);
                                }
                                MainViewer.Instance.BoardTeam2 = listOfGod;
                                MainViewer.Instance.UpdateTeamList();
                                break;

                            case ServerSendType.SendGodMoved:
                                temp1 = im.ReadVariableInt32();
                                temp2 = im.ReadVariableInt32();
                                if (temp1 == 1)
                                {
                                    MainViewer.Instance.BoardTeam1[temp2].X = im.ReadVariableInt32();
                                    MainViewer.Instance.BoardTeam1[temp2].Y = im.ReadVariableInt32();
                                }
                                else
                                {
                                    MainViewer.Instance.BoardTeam2[temp2].X = im.ReadVariableInt32();
                                    MainViewer.Instance.BoardTeam2[temp2].Y = im.ReadVariableInt32();
                                }
                                break;
                            case ServerSendType.SendWhiteboardSendBoard:
                                curMap = im.ReadVariableInt32();
                                count = im.ReadVariableInt32();
                                whiteboard = im.ReadBytes(count);
                                /*whiteboardx.Clear();
                                whiteboardy.Clear();
                                whiteboardpixel.Clear();
                                count1 = (uint)im.ReadVariableInt32();
                                for (int i = 0; i < count1; i++)
                                {
                                    whiteboardx.Add(im.ReadVariableInt32());
                                    whiteboardy.Add(im.ReadVariableInt32());   
                                }
                                for (int i = 0; i < count1 * 4; i++)
                                {
                                    whiteboardpixel.Add(im.ReadByte());
                                }
                                MainViewer.Instance.ReceiveDrawnWhiteboard(whiteboardx, whiteboardy, whiteboardpixel);
                                 */
                                MainViewer.Instance.ReceiveDrawnWhiteboard(curMap, whiteboard);
                                break;
                            case ServerSendType.SendWhiteboardGivePen:
                                WriteText("Whiteboard drawing privilege had been granted");
                                MainViewer.Instance.EnableWhiteboard();
                                break;
                            case ServerSendType.SendWhiteboardTakePen:
                                WriteText("Whiteboard drawing privilege had been revoked");
                                MainViewer.Instance.DisableWhiteboard();
                                break;
                        }
                        
                        //string chat = im.ReadString();
                        //WriteText(chat);
                        break;
                    default:
                        WriteText("Unhandled type: " + im.MessageType + " " + im.LengthBytes + " bytes");
                        break;
                }
            }
        }

        // Send Data Methods
        public void Send(string text)
        {
            NetOutgoingMessage om = _client.CreateMessage(text);
            _client.SendMessage(om, NetDeliveryMethod.ReliableOrdered);
            WriteText("Sending '" + text + "'");
            _client.FlushSendQueue();
        }

        public void SendLoginInformation()
        {
            NetOutgoingMessage om = _client.CreateMessage();
            om.WriteVariableInt32((int)ClientSendType.ClientSendLoginInformation);
            om.Write(_password);
            om.Write(MainViewer.Instance.ClientUsername);
            WriteText("Sending password information...");
            _client.SendMessage(om, NetDeliveryMethod.ReliableOrdered);
            _client.FlushSendQueue();
        }

        public void SendBroadcastChat(string text)
        {
            NetOutgoingMessage om = _client.CreateMessage();
            om.WriteVariableInt32((int)ClientSendType.ClientSendChatMessage);
            om.Write(text);
            _client.SendMessage(om, NetDeliveryMethod.ReliableOrdered);
            _client.FlushSendQueue();
        }

        public void SendWhiteboarDrawnSomething(VertexArray v, byte tool) // This right here my friend.
        {
            NetOutgoingMessage om = _client.CreateMessage();
            om.WriteVariableInt32((int)ClientSendType.ClientSendWhiteboardDrawnSomething);
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
            _client.SendMessage(om, NetDeliveryMethod.ReliableOrdered, 0);
            _client.FlushSendQueue();
        }
    }
}
