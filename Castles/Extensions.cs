using OpenGL;
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
        public static float[] ToFloat(this Vector4 v) => new float[] { v.X, v.Y, v.Z, v.W };
        public static Vector4 Without(this Vector4 v, int i)
        {
            float[] f = v.ToFloat();
            f[i] = 0;
            return new Vector4(f[0], f[1], f[2], f[3]);
        }
        public static Matrix4 RemoveRow(this Matrix4 m, int row)
        {
            return new Matrix4(m[0].Without(row), m[1].Without(row), m[2].Without(row), m[3].Without(row));
        }
        public static Matrix4 RemoveColumn(this Matrix4 m, int column, int elements)
        {
            Vector4[] v = m.ToVector();
            for (int i = 0; i < elements; i++)
            {
                v[column] = v[column].Without(i);
            }
            return new Matrix4(v[0], v[1],v[2], v[3]);
        }
        public static Vector4[] ToVector(this Matrix4 m) => new Vector4[] { m[0], m[1], m[2], m[3] };
        public static Matrix4x4 ToMatrix4x4(this Matrix4 m) => new Matrix4x4(
            m[0].X, m[0].Y, m[0].Z, m[0].W,
            m[1].X, m[1].Y, m[1].Z, m[1].W,
            m[2].X, m[2].Y, m[2].Z, m[2].W,
            m[3].X, m[3].Y, m[3].Z, m[3].W);
        public static Matrix4 ToMatrix4(this Matrix4x4 m) => new Matrix4(new float[] {m.M11,m.M12,m.M13,m.M14,m.M21,m.M22,m.M23,m.M24,m.M31,m.M32,m.M33,m.M34,m.M41,m.M42,m.M43,m.M44});
        public static Vector3 Apply(this Vector3 v, Func<float, float> f) => new Vector3(f(v.X), f(v.Y), f(v.Z));
    }
    public static class IEnumerableExtensions
    {
        public static void ForEach<TSource>(this IEnumerable<TSource> e, Action<TSource> a)
        {
            foreach (TSource t in e)
                a(t);
        }
    }
    public static class ShaderExtensions
    {
        public static void SetTexture(this ShaderProgram p, string s, int i) => Gl.Uniform1i(Gl.GetUniformLocation(p.ProgramID, s), i);
        
    }
}
