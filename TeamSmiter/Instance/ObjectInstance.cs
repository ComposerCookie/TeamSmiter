using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamSmiter
{
    public class ObjectInstance
    {
        public ObjectInstance(int x, int y, int id)
        {
            X = x;
            Y = y;
            ID = id;
            Visible = true;
        }

        public int X { get; set; }
        public int Y { get; set; }
        public int ID { get; set; }
        public bool Visible { get; set; }
        public virtual string OriginalName { get; set; }
    }
}
