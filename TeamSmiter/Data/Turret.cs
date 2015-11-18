using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamSmiter
{
    public class Turret : Graphics
    {
        public Turret(string name, string idname)
            : base(name, idname)
        {
            Type = GraphicsType.Turrets;
        }
    }
}
