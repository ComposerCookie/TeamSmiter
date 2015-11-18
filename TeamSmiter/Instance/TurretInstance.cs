using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamSmiter
{
    public class TurretInstance : ObjectInstance
    {
        public TurretInstance(int x, int y, int id)
            : base(x, y, id)
        {

        }

        public override string OriginalName { get { return MainViewer.Instance.Data.TurretsList[ID].IdentifiedName; } }
    }
}
