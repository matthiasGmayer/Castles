using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Castles
{
    class Humanoid
    {
        private Entity head, helm, chest;
        private Entity[] shoulders, feet, hands;

        public Model Head { set { head = new Entity(value, head); } }

    }
}
