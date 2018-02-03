using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Castles
{
    public static class VectorExtensions
    {
        public static float[] ToFloat(this Vector3 v) => new float[] { v.X, v.Y, v.Z };
    }
}
