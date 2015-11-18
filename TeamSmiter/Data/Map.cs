using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamSmiter
{
    public class Map : Graphics
    {
        List<TurretInstance> turretList;

        public Map(string name, string idname, int max1, int max2)
            : base(name, idname)
        {
            Type = GraphicsType.Map;
            turretList = new List<TurretInstance>();
            Team1Max = max1;
            Team2Max = max2;

        }

        public List<TurretInstance> TurretList { get { return turretList; } }
        public int Team1Max { get; set; }
        public int Team2Max { get; set; }
    }
}
