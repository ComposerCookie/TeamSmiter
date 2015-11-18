using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SFML;
using SFML.Window;
using SFML.Graphics;

namespace TeamSmiter
{
    public partial class SmiteMap : UserControl
    {
        RenderWindow DrawWindow = null;
        SFML.Graphics.View ViewPort;

        Texture MapTexture;
        Sprite ToDraw;
        Text coordinate;
        SFML.Graphics.Font font;

        int curX;
        int curY;

        public SmiteMap()
        {
            InitializeComponent();
            OffsetX = 0;
            OffsetY = 0;
            imagePixels = new uint[1612900];

            
        }

        private void OnMouseRelease(object sender, MouseButtonEventArgs e)
        {
            RenderWindow window = (RenderWindow)sender;

        }

        private void OnMousePress(object sender, MouseButtonEventArgs e)
        {
            RenderWindow window = (RenderWindow)sender;


        }

        private void OnMouseMove(object sender, MouseMoveEventArgs e)
        {
            RenderWindow window = (RenderWindow)sender;
            if (Mouse.GetPosition(DrawWindow).X >= 0 && Mouse.GetPosition(DrawWindow).Y > 0 && Mouse.GetPosition(DrawWindow).X < 635 && Mouse.GetPosition(DrawWindow).Y < 635)
            {
                imagePixels[(Mouse.GetPosition(DrawWindow).Y + Mouse.GetPosition(DrawWindow).X) * 4] = 255; // R?
                imagePixels[(Mouse.GetPosition(DrawWindow).Y + Mouse.GetPosition(DrawWindow).X) * 4 + 1] = 255; // G?
                imagePixels[(Mouse.GetPosition(DrawWindow).Y + Mouse.GetPosition(DrawWindow).X) * 4 + 2] = 255; // B?
                imagePixels[(Mouse.GetPosition(DrawWindow).Y + Mouse.GetPosition(DrawWindow).X) * 4 + 3] = 255; // A?
            }

        }

        public int OffsetX { get; set; }
        public int OffsetY { get; set; }
        uint[] imagePixels;
        
        public void Zoom(float magnification)
        {
            ViewPort.Reset(new FloatRect(0f, 0f, picMap.Width, picMap.Height));
            ViewPort.Zoom(1 / (magnification / 100));
            
            if (ViewPort.Size.X < MapTexture.Size.X)
            {
                hScrollMap.Maximum = (int)MapTexture.Size.X - (int)ViewPort.Size.X;
                hScrollMap.Enabled = true;
            }
            else
            {
                OffsetX = 0;
                hScrollMap.Enabled = false;
            }
            if (ViewPort.Size.Y < MapTexture.Size.Y)
            {
                vScrollMap.Maximum = (int)MapTexture.Size.Y - (int)ViewPort.Size.Y;
                vScrollMap.Enabled = true;
            }
            else
            {
                OffsetY = 0;
                vScrollMap.Enabled = false;
            }

            Scrolled();  
        }

        public void ResetView()
        {
            ViewPort.Reset(new FloatRect(0f, 0f, picMap.Width, picMap.Height));
            DrawWindow.SetView(ViewPort);
        }

        public void Scrolled()
        {
            ViewPort.Center = new Vector2f(ViewPort.Size.X / 2 + OffsetX, ViewPort.Size.Y / 2 + OffsetY);
            DrawWindow.SetView(ViewPort);
        }

        private void SmiteMap_Load(object sender, EventArgs e)
        {
            DrawWindow = new RenderWindow(picMap.Handle);
            ViewPort = new SFML.Graphics.View(new FloatRect(0f, 0f, picMap.Width, picMap.Height));

            DrawWindow.MouseButtonPressed += new EventHandler<MouseButtonEventArgs>(OnMousePress);
            DrawWindow.MouseMoved += new EventHandler<MouseMoveEventArgs>(OnMouseMove);
            DrawWindow.MouseButtonReleased += new EventHandler<MouseButtonEventArgs>(OnMouseRelease);
            ResetView();
            
            MapTexture = new Texture("Map.png");
            font = new SFML.Graphics.Font("Georgia.ttf");
            coordinate = new Text("", font);
            Zoom(100);

            Updator.Enabled = true;
        }

        private void Updator_Tick(object sender, EventArgs e)
        {
            DrawWindow.DispatchEvents(); // handle SFML events - NOTE this is still required when SFML is hosted in another window
            DrawWindow.Clear(SFML.Graphics.Color.Black); // clear our SFML RenderWindow
            ToDraw = new Sprite(MapTexture);
            DrawWindow.Draw(ToDraw);

            
            //SFML.Graphics.Font = new SFML.Graphics.Font();
            coordinate.DisplayedString = "" + Mouse.GetPosition(DrawWindow).X + ", " + Mouse.GetPosition(DrawWindow).Y;
            DrawWindow.Draw(coordinate);
            
            DrawWindow.Display();
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

        private void vScrollMap_Scroll(object sender, ScrollEventArgs e)
        {
            OffsetY = vScrollMap.Value;
            Scrolled();
        }

        private void hScrollMap_Scroll(object sender, ScrollEventArgs e)
        {
            OffsetX = hScrollMap.Value;
            Scrolled();
        }
    }
}
