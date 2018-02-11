using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Castles
{
    public static class Tools
    {
        public static float BarryCentric(Vector3 p1, Vector3 p2, Vector3 p3, Vector2 pos)
        {
            float det = (p2.Z - p3.Z) * (p1.X - p3.X) + (p3.X - p2.X) * (p1.Z - p3.Z);
            float l1 = ((p2.Z - p3.Z) * (pos.X - p3.X) + (p3.X - p2.X) * (pos.Y - p3.Z)) / det;
            float l2 = ((p3.Z - p1.Z) * (pos.X - p3.X) + (p1.X - p3.X) * (pos.Y - p3.Z)) / det;
            float l3 = 1.0f - l1 - l2;
            return l1 * p1.Y + l2 * p2.Y + l3 * p3.Y;
        }

        public static float GetAngle(Vector2 start, Vector2 end)
        {
            Vector2 distance = end - start;
            distance.X = Math.Abs(distance.X);
            float angle = (float)Math.Atan((distance.Y) / (distance.Y));
            if (start.X > end.X)
                angle = (float)Math.PI - angle;
            return angle;
        }

        public static float GetAngle(Vector2 point) => GetAngle(new Vector2(0, 0), point);
        
    }
}
