using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Graphics;
using SFML.Window;

namespace TeamSmiter
{
    public class Graphics
    {
        public Graphics(string name, string idname)
        {
            FileName = name;
            IdentifiedName = idname;
            TextureImage = new Texture(name);
            Width = (int)TextureImage.Size.X;
            Height = (int)TextureImage.Size.Y;
            X = 0;
            Y = 0;
        }

        public string IdentifiedName { get; set; }
        public string FileName { get; set; }
        public Texture TextureImage { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public GraphicsType Type { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
    }

}
