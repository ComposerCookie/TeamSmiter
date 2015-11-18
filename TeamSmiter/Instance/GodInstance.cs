using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamSmiter
{
    public class GodInstance : ObjectInstance
    {
        public GodInstance(int x, int y, int id)
            : base(x, y, id)
        {

        }

        public override string OriginalName { get { return ID > -1 ? MainViewer.Instance.Data.GodList[ID].IdentifiedName : "<empty>"; } }
    }
}
