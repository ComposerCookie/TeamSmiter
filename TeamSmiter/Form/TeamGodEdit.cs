using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SFML;
using SFML.Window;
using SFML.Graphics;

namespace TeamSmiter
{
    public partial class TeamGodEdit : Form
    {
        #region Private Variables

        // Drawing Window
        RenderWindow DrawWindow = null;

        // Viewport
        SFML.Graphics.View ViewPort;

        // Graphics related stuff, used for drawing only
        Sprite ToDraw;
        Sprite Selector;

        // Decide which one we are clicking on:
        bool isTeam1;

        // For temporary holding
        int temp;

        #endregion

        #region Constructor

        public TeamGodEdit()
        {
            InitializeComponent();
            Multiline = true;
            isTeam1 = true;
        }

        #endregion

        #region Property

        public int OffsetX { get; set; }
        public int OffsetY { get; set; }
        public bool Multiline { get; set; }
        public int ImagePerRow { get { return picChampBox.Size.Width / MainViewer.Instance.Data.GodFaceSize; } }
        public int NumberOfRow { get { return MainViewer.Instance.Data.GodList.Count / ImagePerRow + 1; } }

        #endregion

        public void UpdateTeamList()
        {
            lstTeam1.DataSource = null;
            lstTeam1.DataSource = MainViewer.Instance.BoardTeam1;
            lstTeam1.DisplayMember = "OriginalName";

            lstTeam2.DataSource = null;
            lstTeam2.DataSource = MainViewer.Instance.BoardTeam2;
            lstTeam2.DisplayMember = "OriginalName";
        }

        public void ResetScroll()
        {
            Vector2u BoxSize = Multiline ? new Vector2u((uint)(ImagePerRow * MainViewer.Instance.Data.GodFaceSize), (uint)NumberOfRow * (uint)MainViewer.Instance.Data.GodFaceSize) : new Vector2u((uint)MainViewer.Instance.Data.GodList.Count * (uint)MainViewer.Instance.Data.GodFaceSize, (uint)MainViewer.Instance.Data.GodFaceSize);
            if (ViewPort.Size.X < BoxSize.X)
            {
                hScrollBox.Maximum = (int)BoxSize.X - (int)ViewPort.Size.X;
                hScrollBox.Enabled = true;
            }
            else
            {
                OffsetX = 0;
                hScrollBox.Enabled = false;
            }
            if (ViewPort.Size.Y < BoxSize.Y)
            {
                vScrollBox.Maximum = (int)BoxSize.Y - (int)ViewPort.Size.Y;
                vScrollBox.Enabled = true;
            }
            else
            {
                OffsetY = 0;
                vScrollBox.Enabled = false;
            }

            Scrolled();
        }

        /// <summary>
        /// Reset the view, literally
        /// </summary>
        public void ResetView()
        {
            ViewPort.Reset(new FloatRect(0f, 0f, picChampBox.Width, picChampBox.Height));
            DrawWindow.SetView(ViewPort);
        }

        /// <summary>
        /// Scrolling the view
        /// </summary>
        public void Scrolled()
        {
            ViewPort.Center = new Vector2f(ViewPort.Size.X / 2 + OffsetX, ViewPort.Size.Y / 2 + OffsetY);
            DrawWindow.SetView(ViewPort);
        }

        private void TeamGodEdit_Load(object sender, EventArgs e)
        {
            DrawWindow = new RenderWindow(picChampBox.Handle);
            ViewPort = new SFML.Graphics.View(new FloatRect(0f, 0f, picChampBox.Width, picChampBox.Height));
            Selector = new Sprite(MainViewer.Instance.Data.Miscanellous[0].TextureImage);

            DrawWindow.MouseButtonPressed += new EventHandler<MouseButtonEventArgs>(OnMousePress);
            DrawWindow.MouseMoved += new EventHandler<MouseMoveEventArgs>(OnMouseMove);
            DrawWindow.MouseButtonReleased += new EventHandler<MouseButtonEventArgs>(OnMouseRelease);

            ResetView();
            ResetScroll();
            UpdateTeamList();

            isTeam1 = true;

            Updator.Enabled = true;
        }

        private void OnMouseRelease(object sender, MouseButtonEventArgs e)
        {
            RenderWindow window = (RenderWindow)sender;

            temp = (Mouse.GetPosition(DrawWindow).Y + OffsetY) / MainViewer.Instance.Data.GodFaceSize * ImagePerRow + (Mouse.GetPosition(DrawWindow).X + OffsetX) / MainViewer.Instance.Data.GodFaceSize;
            if (temp >= MainViewer.Instance.Data.GodList.Count)
                return;
            if (isTeam1 && lstTeam1.SelectedIndex > -1)
            {
                MainViewer.Instance.BoardTeam1[lstTeam1.SelectedIndex].ID = temp;
            }
            else if (!isTeam1 && lstTeam2.SelectedIndex > -1)
            {
                MainViewer.Instance.BoardTeam2[lstTeam2.SelectedIndex].ID = temp;
            }

            UpdateTeamList();
        }

        private void OnMousePress(object sender, MouseButtonEventArgs e)
        {
            RenderWindow window = (RenderWindow)sender;
        }

        private void OnMouseMove(object sender, MouseMoveEventArgs e)
        {
            
        }

        protected override void OnPaint(System.Windows.Forms.PaintEventArgs e)
        {
            // don't call base.OnPaint(e) to prevent forground painting
            // base.OnPaint(e);
        }
        protected override void OnPaintBackground(System.Windows.Forms.PaintEventArgs pevent)
        {
            // don't call base.OnPaintBackground(e) to prevent background painting
            //base.OnPaintBackground(pevent);
        }

        private void Updator_Tick(object sender, EventArgs e)
        {
            DrawWindow.DispatchEvents(); // handle SFML events - NOTE this is still required when SFML is hosted in another window

            DrawWindow.Clear(SFML.Graphics.Color.Black); // clear our SFML RenderWindow

            for (int i = 0; i < MainViewer.Instance.Data.GodList.Count; i++)
            {
                if (Multiline)
                {
                    DrawWindow.Draw(MainViewer.Instance.Data.GodList[i].GetFace(i % ImagePerRow * MainViewer.Instance.Data.GodFaceSize, i / ImagePerRow * MainViewer.Instance.Data.GodFaceSize));
                }
            }

            if (isTeam1 && lstTeam1.SelectedIndex > -1)
            {
                if (Multiline)
                    Selector.Position = new Vector2f(MainViewer.Instance.BoardTeam1[lstTeam1.SelectedIndex].ID % ImagePerRow * MainViewer.Instance.Data.GodFaceSize, MainViewer.Instance.BoardTeam1[lstTeam1.SelectedIndex].ID / ImagePerRow * MainViewer.Instance.Data.GodFaceSize);
                DrawWindow.Draw(Selector);
            }
            else if (!isTeam1 && lstTeam2.SelectedIndex > -1)
            {
                if (Multiline)
                    Selector.Position = new Vector2f(MainViewer.Instance.BoardTeam2[lstTeam2.SelectedIndex].ID % ImagePerRow * MainViewer.Instance.Data.GodFaceSize, MainViewer.Instance.BoardTeam2[lstTeam2.SelectedIndex].ID / ImagePerRow * MainViewer.Instance.Data.GodFaceSize);
                DrawWindow.Draw(Selector);
            }

            DrawWindow.Display();
        }

        private void hScrollBox_Scroll(object sender, ScrollEventArgs e)
        {
            OffsetX = hScrollBox.Value;
            Scrolled();
        }

        private void vScrollBox_Scroll(object sender, ScrollEventArgs e)
        {
            OffsetY = vScrollBox.Value;
            Scrolled();
        }

        private void lstTeam1_Click(object sender, EventArgs e)
        {
            isTeam1 = true;
        }

        private void lstTeam2_Click(object sender, EventArgs e)
        {
            isTeam1 = false;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void TeamGodEdit_FormClosing(object sender, FormClosingEventArgs e)
        {
            DrawWindow.Close();
        }
    }
}
