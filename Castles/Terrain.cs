using OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Castles
{
    class Terrain : IDisposable
    {
        VAO vao;
        int length = 400;
        float spacing = 1f;
        public Terrain(ShaderProgram program)
        {

            List<Vector3> vertices = new List<Vector3>();
            List<Vector3> normals = new List<Vector3>();
            List<int> elements = new List<int>();

            heightFunction = (x, z) => h.GenerateHeight(x, z);

            for (int x = 0; x < length; x++)
            {
                for (int z = 0; z < length; z++)
                {
                    vertices.Add(new Vector3(x * spacing, GetHeight(x, z) ?? 0, z * spacing));
                }
            }

            for (int x = 0; x < length; x++)
            {
                for (int z = 0; z < length; z++)
                {
                    normals.Add(CalculateNormal(x, z));
                }
            }

            for (int x = 0; x < length - 1; x++)
            {
                for (int z = 0; z < length - 1; z++)
                {
                    elements.Add(z + x * length);
                    elements.Add(z + 1 + x * length);
                    elements.Add((x + 1) * length + z);
                    elements.Add(z + 1 + x * length);
                    elements.Add((x + 1) * length + z + 1);
                    elements.Add((x + 1) * length + z);
                }
            }

            vao = new VAO(program, new VBO<Vector3>(vertices.ToArray()), new VBO<Vector3>(normals.ToArray()), new VBO<int>(elements.ToArray(), BufferTarget.ElementArrayBuffer));
        }
        Random random = new Random();

        private Dictionary<(int, int), float> heightMap = new Dictionary<(int, int), float>();
        private Func<int, int, float> heightFunction;

        private float? GetHeight(int x, int z)
        {
            if (x >= length || z >= length || x < 0 || z < 0)
                return null;
            if (!heightMap.ContainsKey((x, z)))
                heightMap.Add((x, z), heightFunction(x, z));

            return heightMap[(x, z)];
        }
        private Vector3 CalculateNormal(int x, int z)
        {
            float myHeight = GetHeight(x, z) ?? 0;
            float[] heights = new float[] { GetHeight(x - 1, z) ?? myHeight, GetHeight(x + 1, z) ?? myHeight, GetHeight(x, z - 1) ?? myHeight, GetHeight(x, z + 1) ?? myHeight };
            Vector3 normal = new Vector3(heights[0] - heights[1], 2, heights[2] - heights[3]);
            return normal.Normalize();
        }




        public void Render()
        {
            vao.Program["transformation_matrix"]?.SetValue(Matrix4.Identity);
            vao.Draw();
        }

        public void Dispose()
        {
            vao.DisposeChildren = true;
            vao.Dispose();
        }
    }
}
