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
        #region Private Variables

        // Drawing Window
        RenderWindow DrawWindow = null;
        
        // Viewport
        SFML.Graphics.View ViewPort;
        
        // Graphics related stuff, used for drawing only
        Sprite MapSprite;
        Sprite ToDraw;
        SFML.Graphics.Font font;
        RenderTexture forDraw;

        // State for drawing eraser
        RenderStates rState = new RenderStates(BlendMode.None);

        // For drawing lines
        VertexArray lines = new VertexArray(PrimitiveType.TrianglesStrip);
        VertexArray transparent = new VertexArray(PrimitiveType.TrianglesStrip);
        float halfSize = .5f;
        float x1; float x2; float y1; float y2;
        float differenceX; float differenceY;
        Vector2f[] vertexesToDraw;

        // When mouse is down
        bool down = false;

        // Tell which team is being focused
        bool selectedTeam1;

        // A temporary ordering system so that we draw whichever being most focused recently
        List<KeyValuePair<int, int>> renderOrder;
        KeyValuePair<int, int> tempGodPair;

        // Zooming variable
        float zoom;

        // Mouse last position
        Vector2f lastposition = new Vector2f();
        Vector2f lastClickPosition = new Vector2f();

        // tmeporary data for last god position prior to move
        int tempGodX;
        int tempGodY;
        
        // Image of the board at specific time
        SFML.Graphics.Image startBoard;

        // Board to draw
        Texture boardImage;

        // define whether map had been loaded
        bool loadedMap;

        #endregion

        #region Constructor

        public SmiteMap()
        {
            InitializeComponent();

            // By default no offset (draw at 0, 0)
            OffsetX = 0;
            OffsetY = 0;

            // initialize empty int for temporary god location:
            tempGodX = 0;
            tempGodY = 0;

            // Set the default zoom
            zoom = 1;

            //Defining the default shape for both images and drawing stuffs
            startBoard = new SFML.Graphics.Image(963, 891);
            forDraw = new RenderTexture(963, 891);

            // Defining drawing vertexes for lines
            vertexesToDraw = new Vector2f[6];

            // Define the whiteboard data for transmiting between client and server
            WhiteBoardPixel = new List<byte>();
            WhiteBoardX = new List<int>();
            WhiteBoardY = new List<int>();
            
            // Create a texture from the image
            boardImage = new Texture(startBoard);

            // No map loaded by default
            loadedMap = false;

            // Default selected Team is team1
            selectedTeam1 = true;

            // Initialize god team draw order
            renderOrder = new List<KeyValuePair<int, int>>();
        }

        #endregion

        public void TeamSizeChanged()
        {
            renderOrder.Clear();
            for (int i = 0; i < MainViewer.Instance.BoardTeam1.Count; i++)
            {
                renderOrder.Add(new KeyValuePair<int, int>(1, i));
            }
            for (int i = 0; i < MainViewer.Instance.BoardTeam2.Count; i++)
            {
                renderOrder.Add(new KeyValuePair<int, int>(2, i));
            }
        }

        #region Property

        public int OffsetX { get; set; }
        public int OffsetY { get; set; }
        public List<byte> WhiteBoardPixel { get; set; }
        public List<int> WhiteBoardX { get; set; }
        public List<int> WhiteBoardY { get; set; }
        public byte[] Whiteboard;

        #endregion

        #region View Port Related

        /// <summary>
        /// Reset the scrolling, literally, called whenever map is resized or zoomed to calculate the scroll max value
        /// </summary>
        public void ResetScroll()
        {
            Vector2u mapSize = MapSprite == null ? new Vector2u(963, 891) : MapSprite.Texture.Size;
            if (ViewPort.Size.X < mapSize.X)
            {
                hScrollMap.Maximum = (int)mapSize.X - (int)ViewPort.Size.X;
                hScrollMap.Enabled = true;
            }
            else
            {
                OffsetX = 0;
                hScrollMap.Enabled = false;
            }
            if (ViewPort.Size.Y < mapSize.Y)
            {
                vScrollMap.Maximum = (int)mapSize.Y - (int)ViewPort.Size.Y;
                vScrollMap.Enabled = true;
            }
            else
            {
                OffsetY = 0;
                vScrollMap.Enabled = false;
            }

            Scrolled();
        }

        /// <summary>
        /// To zoom the view port
        /// </summary>
        /// <param name="magnification">the percentage of how big you want the view port to be</param>
        public void Zoom(float magnification)
        {
            ViewPort.Reset(new FloatRect(0f, 0f, picMap.Width, picMap.Height));
            ViewPort.Zoom(1 / (magnification / 100));

            zoom = magnification / 100;

            ResetScroll();
        }

        /// <summary>
        /// Reset the view, literally
        /// </summary>
        public void ResetView()
        {
            ViewPort.Reset(new FloatRect(0f, 0f, picMap.Width, picMap.Height));
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

        #endregion

        #region Map and Map Changing Related

        /// <summary>
        /// This method will change the map being drawn to the map index that was given
        /// </summary>
        /// <param name="id">an integer that represent the map index inside the data
        /// content pipeline for maps</param>
        public void changeMap(int id)
        {
            // Reset map
            MapSprite = new Sprite(MainViewer.Instance.Data.MapsList[id].TextureImage);
            forDraw = new RenderTexture(MapSprite.Texture.Size.X, MapSprite.Texture.Size.Y);
            startBoard = new SFML.Graphics.Image(MapSprite.Texture.Size.X, MapSprite.Texture.Size.Y);
            loadedMap = true;
            boardImage.Update(startBoard);
            if (ViewPort != null && DrawWindow != null)
            {
                Zoom(100);
                MainViewer.Instance.UpdateDrawTurretList(id);
            }
            else
            {
                MainViewer.Instance.CurrentMap = id;
            }
        }
        
        #endregion

        #region Whiteboard and Drawing Related

        /// <summary>
        /// Change the global tool size, in the future this might be changed so size of everything is the same
        /// </summary>
        /// <param name="value">Give the size of the tool</param>
        public void ChangeToolSize(int value)
        {
            halfSize = value / 2;
        }

        /// <summary>
        /// This method draw the received vertex, used by both the client and the server
        /// Call when whether the server draw something or the client draw something and
        /// had sent the information over
        /// </summary>
        /// <param name="v">V is the vertex array of the drawn line</param>
        /// <param name="tool">Tool is the byte that define which tool is being drawn, corresponse to ToolType enum</param>
        public void DrawReceivedVertex(VertexArray v, byte tool)
        {
            if (tool == (byte)ToolType.Pencil)
                forDraw.Draw(v);
            else
                forDraw.Draw(v, rState);
        }

        /// <summary>
        /// This method is called on the client side, called when the client just connect
        /// or make sure that it got everything drawn correctly
        /// </summary>
        /// <param name="p">an array of byte representing the image, its size is width * height * 4</param>
        public void DrawReceivedWhiteboard(int curMap, byte[] p)
        {
            if (curMap < 0)
                return;
            if (p == null)
                return;
            changeMap(curMap);
            startBoard = new SFML.Graphics.Image(MapSprite.Texture.Size.X, MapSprite.Texture.Size.Y, p);
            boardImage.Update(startBoard);
            ToDraw = new Sprite(boardImage);
            forDraw.Draw(ToDraw);
        }

        /// <summary>
        /// This method is called by the server to copy the texture's image's array of pixels
        /// to the whiteboard array, so that they can be send over to the client
        /// </summary>
        public void GetWhiteboard()
        {
            Whiteboard = forDraw.Texture.CopyToImage().Pixels;
        }

        /// <summary>
        /// This method is called by both server and client to draw the current map
        /// </summary>
        public void DrawMap()
        {
            DrawWindow.Draw(MapSprite);
            for (int i = 0; i < MainViewer.Instance.Data.MapsList[MainViewer.Instance.CurrentMap].TurretList.Count; i++)
            {
                if(MainViewer.Instance.Data.MapsList[MainViewer.Instance.CurrentMap].TurretList[i].Visible)
                    DrawWindow.Draw(new Sprite(MainViewer.Instance.Data.TurretsList[MainViewer.Instance.Data.MapsList[MainViewer.Instance.CurrentMap].TurretList[i].ID].TextureImage) { Position = new Vector2f(MainViewer.Instance.Data.MapsList[MainViewer.Instance.CurrentMap].TurretList[i].X, MainViewer.Instance.Data.MapsList[MainViewer.Instance.CurrentMap].TurretList[i].Y) });
            }
        }

        #endregion

        #region Instance and Instance Creation/Modification Related


        #endregion

        #region Form-controls Specific Methods

        private void Updator_Tick(object sender, EventArgs e)
        {
            DrawWindow.DispatchEvents(); // handle SFML events - NOTE this is still required when SFML is hosted in another window

            forDraw.Draw(lines);
            forDraw.Draw(transparent, rState);

            if (lines.VertexCount > 0)
            {
                MainViewer.Instance.SendNewDrawnSomething((MainViewer.Instance.CurrentTool == (int)ToolType.Pencil) ? lines : transparent, (byte)MainViewer.Instance.CurrentTool);
            }
            if (transparent.VertexCount > 0)
            {
                MainViewer.Instance.SendNewDrawnSomething((MainViewer.Instance.CurrentTool == (int)ToolType.Pencil) ? lines : transparent, (byte)MainViewer.Instance.CurrentTool);
            }

            lines.Clear();
            transparent.Clear();
            forDraw.Display();

            DrawWindow.Clear(SFML.Graphics.Color.Black); // clear our SFML RenderWindow

            //ToDraw = new Sprite(MapTexture);
            // Why store the texture and not the sprite?
            if (loadedMap)
                DrawMap();

            ToDraw = new Sprite(forDraw.Texture);
            DrawWindow.Draw(ToDraw);

            for (int i = 0; i < MainViewer.Instance.BoardTeam1.Count; i++)
            {
                if (MainViewer.Instance.BoardTeam1[i].ID > -1)
                    DrawWindow.Draw(MainViewer.Instance.Data.GodList[MainViewer.Instance.BoardTeam1[i].ID].GetMini(MainViewer.Instance.BoardTeam1[i].X, MainViewer.Instance.BoardTeam1[i].Y));
            }

            for (int i = 0; i < MainViewer.Instance.BoardTeam2.Count; i++)
            {
                if (MainViewer.Instance.BoardTeam2[i].ID > -1)
                    DrawWindow.Draw(MainViewer.Instance.Data.GodList[MainViewer.Instance.BoardTeam2[i].ID].GetMini(MainViewer.Instance.BoardTeam2[i].X, MainViewer.Instance.BoardTeam2[i].Y));
            }

            if (renderOrder.Count > 0)
            {
                if (renderOrder[0].Key == 1)
                {
                    DrawWindow.Draw(new Sprite(MainViewer.Instance.Data.Miscanellous[1].TextureImage) { Position = new Vector2f(MainViewer.Instance.BoardTeam1[renderOrder[0].Value].X - MainViewer.Instance.Data.GodMiniFaceSize / 2, MainViewer.Instance.BoardTeam1[renderOrder[0].Value].Y - MainViewer.Instance.Data.GodMiniFaceSize / 2) });
                }
                else
                {
                    DrawWindow.Draw(new Sprite(MainViewer.Instance.Data.Miscanellous[1].TextureImage) { Position = new Vector2f(MainViewer.Instance.BoardTeam2[renderOrder[0].Value].X - MainViewer.Instance.Data.GodMiniFaceSize / 2, MainViewer.Instance.BoardTeam2[renderOrder[0].Value].Y - MainViewer.Instance.Data.GodMiniFaceSize / 2) });
                }
            }
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

        private void OnMouseRelease(object sender, MouseButtonEventArgs e)
        {
            RenderWindow window = (RenderWindow)sender;
            down = !(down && e.Button == Mouse.Button.Left);
            
        }

        private void OnMousePress(object sender, MouseButtonEventArgs e)
        {
            RenderWindow window = (RenderWindow)sender;
            if (!loadedMap)
                return;
            if (!down && MainViewer.Instance.CurrentTool == (int)ToolType.GodMove)
            {
                for (int i = 0; i < renderOrder.Count; i++)
                {
                    if (renderOrder[i].Key == 1)
                    {
                        if (MainViewer.Instance.BoardTeam1[renderOrder[i].Value].X - MainViewer.Instance.Data.GodMiniFaceSize / 2 < Mouse.GetPosition(DrawWindow).X
                            && MainViewer.Instance.BoardTeam1[renderOrder[i].Value].X + MainViewer.Instance.Data.GodMiniFaceSize / 2 > Mouse.GetPosition(DrawWindow).X
                            && MainViewer.Instance.BoardTeam1[renderOrder[i].Value].Y - MainViewer.Instance.Data.GodMiniFaceSize / 2 < Mouse.GetPosition(DrawWindow).Y
                            && MainViewer.Instance.BoardTeam1[renderOrder[i].Value].Y - MainViewer.Instance.Data.GodMiniFaceSize / 2 < Mouse.GetPosition(DrawWindow).Y)
                        {
                            tempGodPair = renderOrder[i];
                            renderOrder.RemoveAt(i);
                            renderOrder.Insert(0, tempGodPair);
                            selectedTeam1 = true;
                            tempGodX = MainViewer.Instance.BoardTeam1[tempGodPair.Value].X;
                            tempGodY = MainViewer.Instance.BoardTeam1[tempGodPair.Value].Y;
                        }
                    }
                    else
                    {
                        if (MainViewer.Instance.BoardTeam2[renderOrder[i].Value].X - MainViewer.Instance.Data.GodMiniFaceSize / 2 < Mouse.GetPosition(DrawWindow).X
                            && MainViewer.Instance.BoardTeam2[renderOrder[i].Value].X + MainViewer.Instance.Data.GodMiniFaceSize / 2 > Mouse.GetPosition(DrawWindow).X
                            && MainViewer.Instance.BoardTeam2[renderOrder[i].Value].Y - MainViewer.Instance.Data.GodMiniFaceSize / 2 < Mouse.GetPosition(DrawWindow).Y
                            && MainViewer.Instance.BoardTeam2[renderOrder[i].Value].Y - MainViewer.Instance.Data.GodMiniFaceSize / 2 < Mouse.GetPosition(DrawWindow).Y)
                        {
                            tempGodPair = renderOrder[i];
                            renderOrder.RemoveAt(i);
                            renderOrder.Insert(0, tempGodPair);
                            selectedTeam1 = false;
                            tempGodX = MainViewer.Instance.BoardTeam2[tempGodPair.Value].X;
                            tempGodY = MainViewer.Instance.BoardTeam2[tempGodPair.Value].Y;
                        }
                    }
                }
                lastClickPosition = new Vector2f(Mouse.GetPosition(DrawWindow).X, Mouse.GetPosition(DrawWindow).Y);
                
            }
            down = Mouse.IsButtonPressed(Mouse.Button.Left) && Mouse.GetPosition(DrawWindow).X >= 0 && Mouse.GetPosition(DrawWindow).Y >= 0 && Mouse.GetPosition(DrawWindow).X < MapSprite.Texture.Size.X && Mouse.GetPosition(DrawWindow).Y < MapSprite.Texture.Size.Y;
            lastposition = new Vector2f(Mouse.GetPosition(DrawWindow).X, Mouse.GetPosition(DrawWindow).Y);
        }

        private void OnMouseMove(object sender, MouseMoveEventArgs e)
        {
            RenderWindow window = (RenderWindow)sender;
            if (down && (MainViewer.Instance.CurrentTool == (int)ToolType.Pencil || MainViewer.Instance.CurrentTool == (int)ToolType.Eraser))
            {
                x1 = lastposition.X / zoom + OffsetX;
                y1 = lastposition.Y / zoom + OffsetY;

                lastposition = new Vector2f(e.X, e.Y);

                x2 = lastposition.X / zoom + OffsetX;
                y2 = lastposition.Y / zoom + OffsetY;

                differenceX = x1 - x2;
                differenceY = y1 - y2;

                Console.Out.WriteLine(zoom);

                if (differenceX == 0)
                {
                    if (differenceY <= 0)
                    {
                        vertexesToDraw[0] = new Vector2f(x1 - halfSize, y1 - halfSize);
                        vertexesToDraw[1] = new Vector2f(x1 + halfSize, y1 - halfSize);
                        vertexesToDraw[2] = new Vector2f(x1 - halfSize, y1 + halfSize);
                        vertexesToDraw[3] = new Vector2f(x2 + halfSize, y2 + halfSize);
                        vertexesToDraw[4] = new Vector2f(x2 - halfSize, y2 + halfSize);
                        vertexesToDraw[5] = new Vector2f(x2 - halfSize, y2 + halfSize);
                    }

                    else
                    {
                        vertexesToDraw[0] = new Vector2f(x1 + halfSize, y1 + halfSize);
                        vertexesToDraw[1] = new Vector2f(x1 - halfSize, y1 + halfSize);
                        vertexesToDraw[2] = new Vector2f(x1 + halfSize, y1 - halfSize);
                        vertexesToDraw[3] = new Vector2f(x2 - halfSize, y2 - halfSize);
                        vertexesToDraw[4] = new Vector2f(x2 + halfSize, y2 - halfSize);
                        vertexesToDraw[5] = new Vector2f(x2 + halfSize, y2 - halfSize);
                    }
                }

                else if (differenceX < 0)
                {
                    if (differenceY <= 0)
                    {
                        vertexesToDraw[0] = new Vector2f(x1 - halfSize, y1 - halfSize);
                        vertexesToDraw[1] = new Vector2f(x1 + halfSize, y1 - halfSize);
                        vertexesToDraw[2] = new Vector2f(x1 - halfSize, y1 + halfSize);
                        vertexesToDraw[3] = new Vector2f(x2 + halfSize, y2 - halfSize);
                        vertexesToDraw[4] = new Vector2f(x2 - halfSize, y2 + halfSize);
                        vertexesToDraw[5] = new Vector2f(x2 + halfSize, y2 + halfSize);
                    }

                    else
                    {
                        vertexesToDraw[0] = new Vector2f(x1 - halfSize, y1 + halfSize);
                        vertexesToDraw[1] = new Vector2f(x1 - halfSize, y1 - halfSize);
                        vertexesToDraw[2] = new Vector2f(x1 + halfSize, y1 + halfSize);
                        vertexesToDraw[3] = new Vector2f(x2 - halfSize, y2 - halfSize);
                        vertexesToDraw[4] = new Vector2f(x2 + halfSize, y2 + halfSize);
                        vertexesToDraw[5] = new Vector2f(x2 + halfSize, y2 - halfSize);
                    }
                }

                else
                {
                    if (differenceY <= 0)
                    {
                        vertexesToDraw[0] = new Vector2f(x1 + halfSize, y1 - halfSize);
                        vertexesToDraw[1] = new Vector2f(x1 - halfSize, y1 - halfSize);
                        vertexesToDraw[2] = new Vector2f(x1 + halfSize, y1 + halfSize);
                        vertexesToDraw[3] = new Vector2f(x2 - halfSize, y2 - halfSize);
                        vertexesToDraw[4] = new Vector2f(x2 + halfSize, y2 + halfSize);
                        vertexesToDraw[5] = new Vector2f(x2 - halfSize, y2 + halfSize);
                    }

                    else
                    {
                        vertexesToDraw[0] = new Vector2f(x1 + halfSize, y1 + halfSize);
                        vertexesToDraw[1] = new Vector2f(x1 - halfSize, y1 + halfSize);
                        vertexesToDraw[2] = new Vector2f(x1 + halfSize, y1 - halfSize);
                        vertexesToDraw[3] = new Vector2f(x2 - halfSize, y2 + halfSize);
                        vertexesToDraw[4] = new Vector2f(x2 + halfSize, y2 - halfSize);
                        vertexesToDraw[5] = new Vector2f(x2 - halfSize, y2 - halfSize);
                    }
                }

                switch (MainViewer.Instance.CurrentTool)
                {
                    case (int)ToolType.Pencil:
                        for (int i = 0; i < 6; i++)
                        {
                            lines.Append(new Vertex(vertexesToDraw[i], MainViewer.Instance.PickedColor[MainViewer.Instance.CurrentSelectedColor]));
                        }

                        break;
                    case (int)ToolType.Eraser:
                        for (int i = 0; i < 6; i++)
                        {
                            transparent.Append(new Vertex(vertexesToDraw[i], new SFML.Graphics.Color(0, 0, 0, 0)));
                        }
                        break;
                }
            }
            else if (down && MainViewer.Instance.CurrentTool == (int)ToolType.GodMove && renderOrder.Count > 0)
            {
                lastposition = new Vector2f(Mouse.GetPosition(DrawWindow).X, Mouse.GetPosition(DrawWindow).Y);
                if (renderOrder[0].Key == 1)
                {
                    MainViewer.Instance.BoardTeam1[renderOrder[0].Value].X = tempGodX + (int)(lastposition.X - lastClickPosition.X);
                    MainViewer.Instance.BoardTeam1[renderOrder[0].Value].Y = tempGodY + (int)(lastposition.Y - lastClickPosition.Y);

                    MainViewer.Instance.SendGodMoved(1, renderOrder[0].Value, MainViewer.Instance.BoardTeam1[renderOrder[0].Value].X, MainViewer.Instance.BoardTeam1[renderOrder[0].Value].Y);
                }
                else
                {
                    MainViewer.Instance.BoardTeam2[renderOrder[0].Value].X = tempGodX + (int)(lastposition.X - lastClickPosition.X);
                    MainViewer.Instance.BoardTeam2[renderOrder[0].Value].Y = tempGodY + (int)(lastposition.Y - lastClickPosition.Y);

                    MainViewer.Instance.SendGodMoved(2, renderOrder[0].Value, MainViewer.Instance.BoardTeam2[renderOrder[0].Value].X, MainViewer.Instance.BoardTeam2[renderOrder[0].Value].Y);
                }
            }
        }

        private void SmiteMap_Load(object sender, EventArgs e)
        {
            DrawWindow = new RenderWindow(picMap.Handle);
            ViewPort = new SFML.Graphics.View(new FloatRect(0f, 0f, picMap.Width, picMap.Height));

            DrawWindow.MouseButtonPressed += new EventHandler<MouseButtonEventArgs>(OnMousePress);
            DrawWindow.MouseMoved += new EventHandler<MouseMoveEventArgs>(OnMouseMove);
            DrawWindow.MouseButtonReleased += new EventHandler<MouseButtonEventArgs>(OnMouseRelease);

            ResetView();

            //MapTexture = new Texture("Map.png");
            //MapSprite = new Sprite(new Texture("Map.png"));
            font = new SFML.Graphics.Font("Georgia.ttf");
            Zoom(100);

            if (MainViewer.Instance.CurrentMap > -1)
                changeMap(MainViewer.Instance.CurrentMap);

            Updator.Enabled = true;
        }

        #endregion

    }
}
