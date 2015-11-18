using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SFML.Graphics;
using SFML.Window;
using Lidgren.Network;

/// <summary>
/// Main Form of the program, the whole program revolve around this form
/// Used for all general purpose of visual showing
/// </summary>

namespace TeamSmiter
{
    public partial class MainViewer : Form
    {
        #region Private Variables
        
        static MainViewer _instance; // Singleton class
        TeamGodEdit tge;
        Server _server;
        Client _client;
        SmiteMap smiteMap; // Main drawing device for the whiteboard
        int notification; // To be used for notifiying when new chat message (or logs) received
        static DataPipeline _data; // Contains graphics datas, images, etc
        DialogResult keepGod; // For asking user

        #endregion

        #region Constructor

        public MainViewer()
        {
            InitializeComponent();

            // Initializing variables
            _data = new DataPipeline();
            smiteMap = new SmiteMap();
            teamTemp = new List<int>();

            // Create the smite map. We couldn't do it via the Toolbox because VS2012 couldn't detect
            // SFML library even though it was referenced, so we gotta do it the hard way
            smiteMap.Size = new System.Drawing.Size(963, 891);
            smiteMap.Location = new Point(7, 22);
            grpMap.Controls.Add(smiteMap);
            
            // By default we would have 0 notification
            notification = 0;
            
            // By default current map isn't set so
            CurrentMap = -1;

            // Repointing to itself, make this a singleton class
            _instance = this;
            
            // Setting up both default value for client and server
            _client = new Client(txtHostIP.Text, int.Parse(txtClientPort.Text));
            _server = new Server(int.Parse(txtServerPort.Text), txtServerPass.Text, txtAdminName.Text);
            
            // Defining 4 initial value for colors - all 4 are blacks
            PickedColor = new SFML.Graphics.Color[4];
            for (int i = 0; i < PickedColor.Length; i++)
                PickedColor[i] = new SFML.Graphics.Color(SFML.Graphics.Color.Black);

            // By default the instance is not a server
            IsServer = false;

            // For drawings
            CurrentSelectedColor = 0;
            PencilSize = 5;
            CurrentTool = (int)ToolType.Pencil;

            // Initialize the team
            BoardTeam1 = new List<GodInstance>();
            BoardTeam2 = new List<GodInstance>();
        }

        #endregion

        #region Property

        public static MainViewer Instance { get { return _instance; } }
        public DataPipeline Data { get { return _data; } }
        public bool IsServer { get; set; }
        public bool LoggedIn { get; set; }
        public string AdminName { get; set; }
        public string ClientUsername { get; set; }
        public SFML.Graphics.Color[] PickedColor { get; set; }
        public byte CurrentSelectedColor { get; set; }
        public int PencilSize { get; set; }
        public int CurrentTool { get; set; }
        public int CurrentMap { get; set; }
        public List<GodInstance> BoardTeam1 { get; set; }
        public List<GodInstance> BoardTeam2 { get; set; }
        public List<int> teamTemp { get; set; }

        #endregion

        #region Client-Server Connections
        
        /// <summary>
        /// Start Lidgren Server
        /// </summary>
        private void StartServer()
        {
            AdminName = txtAdminName.Text;
            _server.StartServer(int.Parse(txtServerPort.Text), txtServerPass.Text, txtAdminName.Text);
            DisableLoginCommand();
        }

        /// <summary>
        /// This method is called by the main thread of the program
        /// which is in Program.cs, handled by Lidgren
        /// basically this method is called everytime server receive
        /// data from clients
        /// </summary>
        public void ServerHandleData()
        {
            _server.HandleIncomingData();
        }
        
        /// <summary>
        /// Start Lidgren Client
        /// </summary>
        private void StartClient()
        {
            ClientUsername = txtNickname.Text;
            _client.Connect(txtHostIP.Text, int.Parse(txtClientPort.Text), txtClientPass.Text);
            DisableLoginCommand();
            IsServer = false;

        }

        #endregion

        #region Data Transfering Methods

        #region Whiteboard Specific
        
        /// <summary>
        /// This method get the pixels list of the whiteboard image, which is
        /// usually a pixel array of width * length * 4 (4 cuz RGBA)
        /// Called when new client connection connected and we send them
        /// a complete copy
        /// Used on the client side
        /// </summary>
        public byte[] getWhiteboard()
        {
            return smiteMap.Whiteboard;
        }

        /// <summary>
        /// Prerequiste to getWhiteboard method, this method request the Smite
        /// Map to generate the pixels array to be used in getWhiteboard method
        /// Used by the server
        /// </summary>
        public void UpdateWhiteboardData()
        {
            smiteMap.GetWhiteboard();
        }

        /// <summary>
        /// This method is called when a new vertex is drawed on either the
        /// client or server. It will send the data to the server so they
        /// can send data back
        /// </summary>
        /// <param name="v">The vertex array contain the lines of the drawing</param>
        /// <param name="tool">byte containing tool type, can be referenced to ToolType enum</param>
        public void SendNewDrawnSomething(VertexArray v, byte tool)
        {
            if (IsServer)
                _server.SendWhiteboarDrawnSomething(v, tool);
            else
                _client.SendWhiteboarDrawnSomething(v, tool);
        }

        /// <summary>
        /// This method is called when the client receive a complete copy of the server whiteboard
        /// Used by client side
        /// </summary>
        /// <param name="p">Array of bytes of the whiteboard, its size is width * height * 4 (4 because RGBA)</param>
        public void ReceiveDrawnWhiteboard(int curMap, byte[] p)
        {
            smiteMap.DrawReceivedWhiteboard(curMap, p);
        }

        /// <summary>
        /// This method is called when either the client or server receive a line being drawn
        /// Used by both server and client
        /// </summary>
        /// <param name="v">The vertex array contain the lines of the drawing</param>
        /// <param name="tool">byte containing tool type, can be referenced to ToolType enum</param>
        public void ReceiveDrawnSomething(VertexArray v, byte tool)
        {
            smiteMap.DrawReceivedVertex(v, tool);
        }

        #endregion

        #endregion

        #region Lists Updating Methods

        /// <summary>
        /// To update the list contain the maps and map images
        /// for the user to pick which map to use (for server only though)
        /// </summary>
        public void UpdateDrawMapsList()
        {
            lstDrawMaps.DataSource = null;
            lstDrawMaps.DataSource = Data.MapsList;
            lstDrawMaps.DisplayMember = "IdentifiedName";
        }

        /// <summary>
        /// To update list of already created turrets instance
        /// Player can pick and modify existing turrets
        /// Or create new instance of turrets
        /// </summary>
        public void UpdateDrawTurretList(int map)
        {
            lstDrawTurrets.DataSource = null;
            lstDrawTurrets.DataSource = Data.MapsList[map].TurretList;
            lstDrawTurrets.DisplayMember = "OriginalName";
        }

        /// <summary>
        /// To update list of already created champions instance
        /// Player can pick and modify existing champions
        /// Or create new instance of champions
        /// </summary>
        public void UpdateDrawChampionList()
        {
            
        }

        /// <summary>
        /// To update the form list with a list of connections
        /// </summary>
        /// <param name="list">Connection list</param>
        public void ServerUpdateOnlineList(List<ConnectedSession> list)
        {
            lstOnline.DataSource = null;
            lstOnline.DataSource = list;
            lstOnline.DisplayMember = "Username";
        }

        /// <summary>
        /// This method is called by the Client Handle Data when receive
        /// a new connection data sent by the server
        /// </summary>
        /// <param name="list">String list generated and sent by the server</param>
        public void ClientUpdateOnlineLog(string[] list)
        {
            lstOnline.DataSource = null;
            lstOnline.DataSource = list;
            //lstOnline.DisplayMember = "Username";
        }

        /// <summary>
        /// This method set the team blah blah
        /// </summary>
        public void UpdateTeamList()
        {
            lstTeam1.DataSource = null;
            lstTeam1.DataSource = BoardTeam1;
            lstTeam1.DisplayMember = "OriginalName";

            lstTeam2.DataSource = null;
            lstTeam2.DataSource = BoardTeam2;
            lstTeam2.DisplayMember = "OriginalName";
        }

        #endregion

        #region Form-specific Enable/Disbale for consistency purpose

        #region Regarding Connection
        /// <summary>
        /// Disable a few of the form's feature, mostly the connection part
        /// such as Conencting Button, host buttons, and read-only text boxes
        /// Situation that call this methods are:
        ///     - An attempt to connect the server is underway
        ///     - User chose to host server
        /// </summary>
        public void DisableLoginCommand()
        {
            txtClientPass.Enabled = false;
            txtClientPort.Enabled = false;
            txtHostIP.Enabled = false;
            txtNickname.Enabled = false;
            txtServerName.Enabled = false;
            txtServerPass.Enabled = false;
            txtServerPort.Enabled = false;
            txtAdminName.Enabled = false;

            btnHost.Enabled = false;
            btnLogin.Enabled = false;

            btnLogin.Text = "Connecting...";
            btnHost.Text = "Starting up...";
        }

        /// <summary>
        /// Enable a few of the form's feature, mostly connection part
        /// Do exactly opposite of what DisableLoginCommand do
        /// Situation that call this methods are:
        ///     - The client was disconnected for any reason
        /// </summary>
        /// <param name="param1">A simple integer</param>
        /// <param name="param2">A more sophisticated string</param>
        public void EnableLoginCommand()
        {
            txtClientPass.Enabled = true;
            txtClientPort.Enabled = true;
            txtHostIP.Enabled = true;
            txtNickname.Enabled = true;
            txtServerName.Enabled = true;
            txtServerPass.Enabled = true;
            txtServerPort.Enabled = true;
            txtAdminName.Enabled = true;

            btnHost.Enabled = true;
            btnLogin.Enabled = true;

            btnLogin.Text = "Retry?";
            btnHost.Text = "Retry?";
        }

        /// <summary>
        /// This method is called when a connection was made, for both client and server
        /// It set up the whiteboard and logged in specific stuffs
        /// </summary>
        public void EnableConnected()
        {
            startTab.Visible = false;
            mainTab.Visible = true;
            UpdateDrawMapsList();

            if (!IsServer)
            {
                DisableWhiteboard();
                btnGivePen.Enabled = false;
                btnTakePen.Enabled = false;
                btnDisconnect.Enabled = false;
            }
        }

        /// <summary>
        /// This method is called when server is shutted down or client is
        /// disconnected. It do opposite of what EnableConnected() do
        /// </summary>
        public void DisableConnected()
        {
            startTab.Visible = true;
            mainTab.Visible = false;
        }

        #endregion

        #region Regarding Whiteboard

        /// <summary>
        /// This method just disable whiteboard tools such as colors,
        /// tools, tools properties
        /// </summary>
        public void DisableWhiteboard()
        {
            grpMap.Enabled = false;
            grpColor.Enabled = false;
            grpProperty.Enabled = false;
            grpTool.Enabled = false;
        }

        /// <summary>
        /// This method enable whiteboards tools, opposite of
        /// EnableWhiteboard() method
        /// </summary>
        public void EnableWhiteboard()
        {
            grpMap.Enabled = true;
            grpColor.Enabled = true;
            grpProperty.Enabled = true;
            grpTool.Enabled = true;
        }

        public void EnableGameStuff()
        {
            btnGodPickEdit.Enabled = true;
            btnShowDrawTurrets.Enabled = true;
        }

        #endregion

        #endregion

        public void LoadNewMap(int map)
        {
            CurrentMap = map;
            smiteMap.changeMap(map);
        }

        /// <summary>
        /// This method will clear the current picked God list and refresh it to "battle" ready (prepare it for load)
        /// </summary>
        public void ClearCurrentGod()
        {
            BoardTeam1.Clear();
            BoardTeam2.Clear();

            for (int i = 0; i < MainViewer.Instance.Data.MapsList[MainViewer.Instance.CurrentMap].Team1Max; i++)
            {
                BoardTeam1.Add(new GodInstance(i * 55 + 50, 50, -1));
            }

            for (int i = 0; i < MainViewer.Instance.Data.MapsList[MainViewer.Instance.CurrentMap].Team2Max; i++)
            {
                BoardTeam2.Add(new GodInstance(i * 55 + 50, 105, -1));
            }
        }

        public void SendGodMoved(int team, int id, int x, int y)
        {
            _server.SendGodMove(team, id, x, y);
        }

        #region Forms-control specifics methods

        private void btnHost_Click(object sender, EventArgs e)
        {
            StartServer();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            if (txtNickname.Text.Equals(""))
            {
                MessageBox.Show("Please enter a nickname");
                return;
            }
            StartClient();
        }

        private void btnSendChat_Click(object sender, EventArgs e)
        {
            if (!txtChat.Text.Equals(""))
            {
                if (!IsServer)
                {
                    _client.SendBroadcastChat(txtChat.Text);
                }
                else
                {
                    _server.SendBroadcastChatMessage(txtAdminName.Text + ": " + txtChat.Text);
                    NativeModule.AppendText(rtbMainLog, txtAdminName.Text + ": " + txtChat.Text);
                }
                txtChat.Text = "";
            }
        }

        private void btnPlusZoom_Click(object sender, EventArgs e)
        {
            txtMapZoom.Text = "" + (float.Parse(txtMapZoom.Text) + 10);
            MapZoom();
        }

        private void btnMinusZoom_Click(object sender, EventArgs e)
        {
            txtMapZoom.Text = "" + (float.Parse(txtMapZoom.Text) - 10);
            MapZoom();
        }

        private void btnClrSelect1_Click(object sender, EventArgs e)
        {
            DialogResult result = clrPicker.ShowDialog();
            // See if user pressed ok.
            if (result == DialogResult.OK)
            {
                // Set form background to the selected color.
                PickedColor[0] = new SFML.Graphics.Color(clrPicker.Color.R, clrPicker.Color.G, clrPicker.Color.B, (byte)(trkOpaque.Value * 2.55f));
                btnClrSelect1.BackColor = clrPicker.Color;
            }

            CurrentSelectedColor = 0;
        }

        private void btnClrSelect2_Click(object sender, EventArgs e)
        {
            DialogResult result = clrPicker.ShowDialog();
            // See if user pressed ok.
            if (result == DialogResult.OK)
            {
                // Set form background to the selected color.
                PickedColor[1] = new SFML.Graphics.Color(clrPicker.Color.R, clrPicker.Color.G, clrPicker.Color.B, (byte)(trkOpaque.Value * 2.55f));
                btnClrSelect2.BackColor = clrPicker.Color;
            }

            CurrentSelectedColor = 1;
        }

        private void btnClrSelect3_Click(object sender, EventArgs e)
        {
            DialogResult result = clrPicker.ShowDialog();
            // See if user pressed ok.
            if (result == DialogResult.OK)
            {
                // Set form background to the selected color.
                PickedColor[2] = new SFML.Graphics.Color(clrPicker.Color.R, clrPicker.Color.G, clrPicker.Color.B, (byte)(trkOpaque.Value * 2.55f));
                btnClrSelect3.BackColor = clrPicker.Color;
            }

            CurrentSelectedColor = 2;
        }

        private void btnClrSelect4_Click(object sender, EventArgs e)
        {
            DialogResult result = clrPicker.ShowDialog();
            // See if user pressed ok.
            if (result == DialogResult.OK)
            {
                // Set form background to the selected color.
                PickedColor[3] = new SFML.Graphics.Color(clrPicker.Color.R, clrPicker.Color.G, clrPicker.Color.B, (byte)(trkOpaque.Value * 2.55f));
                btnClrSelect4.BackColor = clrPicker.Color;
            }

            CurrentSelectedColor = 3;
        }

        private void btnPencilTool_Click(object sender, EventArgs e)
        {
            CurrentTool = (int)ToolType.Pencil;
        }

        private void btnEraserTool_Click(object sender, EventArgs e)
        {
            CurrentTool = (int)ToolType.Eraser;
        }

        private void tabChat_Enter(object sender, EventArgs e)
        {
            notification = 0;
            tabChat.Text = "Chatroom";
        }

        private void ctxMnuClients_Opening(object sender, CancelEventArgs e)
        {

        }

        private void btnGivePen_Click(object sender, EventArgs e)
        {
            if (IsServer)
            {
                if (lstOnline.SelectedIndex != 0)
                    _server.SendWhiteboardGivePen((ConnectedSession)lstOnline.SelectedItem);
            }
        }

        private void btnTakePen_Click(object sender, EventArgs e)
        {
            if (IsServer)
            {
                if (lstOnline.SelectedIndex != 0)
                    _server.SendWhiteboardTakePen((ConnectedSession)lstOnline.SelectedItem);
            }
        }

        private void trkSize_Scroll(object sender, EventArgs e)
        {
            smiteMap.ChangeToolSize(trkSize.Value);
            lblToolSize.Text = "Size: " + trkSize.Value + "px";

        }

        private void ctxMnuClients_MouseCaptureChanged(object sender, EventArgs e)
        {
            ctxMnuClients.Visible = false;
        }

        private void lstOnline_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                lstOnline.SelectedIndex = lstOnline.IndexFromPoint(e.X, e.Y);

                if (lstOnline.SelectedIndex < 0)
                    return;

                ctxMnuClients.Left = e.X;
                ctxMnuClients.Top = e.Y;
                //if (e.X + ctxMnuClients.Size.Width < MainViewer.Instance.Size.Width)
                //{
                //    ctxMnuClients.Left = MainViewer.Instance.Size.Width - ctxMnuClients.Size.Width;
                //}
                //if (e.Y + ctxMnuClients.Size.Height < MainViewer.Instance.Size.Height)
                //{
                //    ctxMnuClients.Top = MainViewer.Instance.Size.Height - ctxMnuClients.Size.Height;
                //}
                ctxMnuClients.Show(ctxMnuClients.Left, ctxMnuClients.Top);
                //ctxMnuClients.Visible = true;
            }
        }

        private void trkOpaque_Scroll(object sender, EventArgs e)
        {
            for (int i = 0; i < 4; i++)
            {
                PickedColor[i].A = (byte)(trkOpaque.Value * 255 / 100);
            }

            lblToolOpaque.Text = "Opaque: " + trkOpaque.Value + "%";
        }

        private void btnPickDrawMap_Click(object sender, EventArgs e)
        {
            smiteMap.changeMap(lstDrawMaps.SelectedIndex);
            // Reset Gods
            if (CurrentMap > -1)
            {
                CurrentMap = lstDrawMaps.SelectedIndex;
                keepGod = MessageBox.Show("Do you want to keep your Gods list?", "Keep list?", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                if (keepGod == DialogResult.No)
                {
                    ClearCurrentGod();
                }
                else
                {
                    for (int i = BoardTeam1.Count; i > MainViewer.Instance.Data.MapsList[MainViewer.Instance.CurrentMap].Team1Max; i--)
                    {
                        BoardTeam1.RemoveAt(i - 1);
                    }

                    for (int i = BoardTeam2.Count; i > MainViewer.Instance.Data.MapsList[MainViewer.Instance.CurrentMap].Team2Max; i--)
                    {
                        BoardTeam2.RemoveAt(i - 1);
                    }
                }
            }
            else
            {
                EnableGameStuff();
                CurrentMap = lstDrawMaps.SelectedIndex;
                ClearCurrentGod();
            }
            UpdateTeamList();
            smiteMap.TeamSizeChanged();

            _server.SendLoadMap(CurrentMap);
        }

        private void btnShowDrawTurrets_Click(object sender, EventArgs e)
        {
            if (CurrentMap >= 0 && Data.MapsList[CurrentMap].TurretList.Count > 0)
                Data.MapsList[CurrentMap].TurretList[lstDrawTurrets.SelectedIndex].Visible = !Data.MapsList[CurrentMap].TurretList[lstDrawTurrets.SelectedIndex].Visible;
        }

        #endregion

        #region Misc Methods

        /// <summary>
        /// This method tell the smite map to zoom
        /// </summary>
        public void MapZoom()
        {
            smiteMap.Zoom(float.Parse(txtMapZoom.Text));
        }
        
        /// <summary>
        /// This function is to update the logs of specific
        /// Rich-text Box, used by both client and server
        /// </summary>
        /// <param name="text">What to write, logs or chat messages</param>
        /// <param name="type">Refer to richtextbox type enum, such as server log, cleint log etc</param>
        public void WriteLog(string text, LogType type)
        {
            switch (type)
            {
                case LogType.ServerLogin:
                    NativeModule.AppendText(rtbServerLogin, text);
                    break;
                case LogType.ClientLogin:
                    NativeModule.AppendText(rtbClientLogin, text);
                    break;
                case LogType.Main:
                    NativeModule.AppendText(rtbMainLog, text);
                    if (!tabChat.Focus())
                    {
                        notification++;
                        tabChat.Text = "Chatroom (" + notification + ")";
                    }
                    break;
            }
        }
        
        #endregion

        private void btnGodPickEdit_Click(object sender, EventArgs e)
        {
            teamTemp.Clear();
            for (int i = 0; i < BoardTeam1.Count; i++)
            {
                teamTemp.Add(BoardTeam1[i].ID);
            }

            for (int i = 0; i < BoardTeam2.Count; i++)
            {
                teamTemp.Add(BoardTeam2[i].ID);
            }

            tge = new TeamGodEdit();
            tge.ShowDialog();

            if (tge.DialogResult == DialogResult.Yes)
            {
                
            }
            else
            {
                for (int i = 0; i < BoardTeam1.Count; i++)
                {
                    BoardTeam1[i].ID = teamTemp[i];
                }
                for (int i = 0; i < BoardTeam2.Count; i++)
                {
                    BoardTeam2[i].ID = teamTemp[i + BoardTeam1.Count];
                }
            }
            UpdateTeamList();
            _server.SendUpdateTeam(BoardTeam1, BoardTeam2);
        }

        private void btnGodMoveTool_Click(object sender, EventArgs e)
        {
            CurrentTool = (int)ToolType.GodMove;
        }
    }
}
