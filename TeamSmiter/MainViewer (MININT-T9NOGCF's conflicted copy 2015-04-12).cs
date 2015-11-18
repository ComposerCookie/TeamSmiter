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

namespace TeamSmiter
{
    public partial class MainViewer : Form
    {     
        static MainViewer _instance;
        Server _server;
        Client _client;
        RenderWindow test;
        SmiteMap smiteMap;
        int notification;

        public MainViewer()
        {
            InitializeComponent();
            smiteMap = new SmiteMap();
            smiteMap.Size = new System.Drawing.Size(723, 723);
            smiteMap.Location = new Point(6, 19);
            grpMap.Controls.Add(smiteMap);
            notification = 0;
            
            _instance = this;
            _client = new Client(txtHostIP.Text, int.Parse(txtClientPort.Text));
            _server = new Server(int.Parse(txtServerPort.Text), txtServerPass.Text, txtAdminName.Text);
            PickedColor = new SFML.Graphics.Color[4];
            for (int i = 0; i < PickedColor.Length; i++)
                PickedColor[i] = new SFML.Graphics.Color();

            IsServer = false;
            CurrentSelectedColor = 0;
            PencilSize = 5;
            CurrentTool = (int)ToolType.Pencil;
        }

        public static MainViewer Instance { get { return _instance; } }
        public bool IsServer { get; set; }
        public bool LoggedIn { get; set; }
        public string AdminName { get; set; }
        public string ClientUsername { get; set; }
        public SFML.Graphics.Color[] PickedColor { get; set; }
        public byte CurrentSelectedColor { get; set; }
        public int PencilSize { get; set; }
        public int CurrentTool { get; set; }

        private void StartServer()
        {
            AdminName = txtAdminName.Text;
            _server.StartServer(int.Parse(txtServerPort.Text), txtServerPass.Text, txtAdminName.Text);
            DisableLoginCommand();
            IsServer = true;
        }

        private void StartClient()
        {
            ClientUsername = txtNickname.Text;
            _client.Connect(txtHostIP.Text, int.Parse(txtClientPort.Text), txtClientPass.Text);
            DisableLoginCommand();
            IsServer = false;

        }

        public void ServerUpdateOnlineList(List<ConnectedSession> list)
        {
            lstOnline.DataSource = null;
            lstOnline.DataSource = list;
            lstOnline.DisplayMember = "Username";
        }

        public void ClientUpdateOnlineLog(string[] list)
        {
            lstOnline.DataSource = null;
            lstOnline.DataSource = list;
            //lstOnline.DisplayMember = "Username";
        }

        public void ServerHandleData()
        {
            _server.HandleIncomingData();
        }

        // Disable this while connecting
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

        // Enable this when fail to establish connection OR rejected by the server
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

        // Enable this once login
        public void EnableConnected()
        {
            startTab.Visible = false;
            mainTab.Visible = true;

            if (!IsServer)
            {
                DisableWhiteboard();
                btnGivePen.Enabled = false;
                btnTakePen.Enabled = false;
                btnDisconnect.Enabled = false;
            }
        }

        public void DisableWhiteboard()
        {
            grpMap.Enabled = false;
            grpColor.Enabled = false;
            grpProperty.Enabled = false;
            grpTool.Enabled = false;
        }

        public void EnableWhiteboard()
        {
            grpMap.Enabled = true;
            grpColor.Enabled = true;
            grpProperty.Enabled = true;
            grpTool.Enabled = true;
        }

        public void DisableConnected()
        {
            startTab.Visible = true;
            mainTab.Visible = false;
        }

        // This function is for writing logs
        public void WriteLog(string text, RichTextBoxLogType type)
        {
            switch (type)
            {
                case RichTextBoxLogType.ServerLogin:
			        NativeModule.AppendText(rtbServerLogin, text);
                    break;
                case RichTextBoxLogType.ClientLogin:
                    NativeModule.AppendText(rtbClientLogin, text);
                    break;
                case RichTextBoxLogType.Main:
                    NativeModule.AppendText(rtbMainLog, text);
                    notification++;
                    tabChat.Text = "Chatroom (" + notification + ")";
                    break;
            }
        }

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

        public void MapZoom()
        {
            smiteMap.Zoom(float.Parse(txtMapZoom.Text));
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

        private void lstOnline_Click(object sender, EventArgs e)
        {
            MouseEventArgs c = (MouseEventArgs)e;
            if (c.Button == System.Windows.Forms.MouseButtons.Right)
            {
                ctxMnuClients.Left = c.X;
                ctxMnuClients.Top = c.Y;
            }
        }


    }
}
