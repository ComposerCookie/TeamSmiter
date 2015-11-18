using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Graphics;

namespace TeamSmiter
{
    public class God : Graphics
    {
        public God(string name, string idname, int fsX, int fsY, int msX, int msY)
            : base(name, idname)
        {
            Type = GraphicsType.God;
            FaceSourceX = fsX;
            FaceSourceY = fsY;
            MiniSourceX = msX;
            MiniSourceY = msY;
        }

        public int FaceSourceX { get; set; }
        public int FaceSourceY { get; set; }
        public int MiniSourceX { get; set; }
        public int MiniSourceY { get; set; }

        public Sprite GetFace(int x, int y)
        {
            return new Sprite(TextureImage) { Position = new SFML.Window.Vector2f(x, y), TextureRect = new IntRect(FaceSourceX, FaceSourceY, MainViewer.Instance.Data.GodFaceSize, MainViewer.Instance.Data.GodFaceSize) };
            //return new Sprite(TextureImage) { TextureRect = new IntRect(FaceSourceX, FaceSourceY, 96, 96) };
        }

        public Sprite GetMini(int x, int y)
        {
            return new Sprite(TextureImage) { Position = new SFML.Window.Vector2f(x - MainViewer.Instance.Data.GodMiniFaceSize / 2, y - MainViewer.Instance.Data.GodMiniFaceSize / 2), TextureRect = new IntRect(MiniSourceX, MiniSourceY, MainViewer.Instance.Data.GodMiniFaceSize, MainViewer.Instance.Data.GodMiniFaceSize) };
        }
    }
}
