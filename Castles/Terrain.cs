using OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Castles
{
    class Terrain
    {
        public static Texture grassTexture = Loader.LoadTexture("!Grass.jpg");
        public static Texture stoneTexture = Loader.LoadTexture("!Stone.jpg");
        public static Dictionary<(int X, int Z), Terrain> terrainMap = new Dictionary<(int, int), Terrain>();
        public (int X, int Z) Position { get; }
        public VAO Vao { get; }
        public static int length = 100;
        public static int spacing = 10;
        public static ShaderProgram terrainShader = Shaders.GetShader("Terrain");

        static Terrain()
        {
            //terrainShader["grassTex"].SetValue(0);
            //terrainShader["stoneTex"].SetValue(1);
            Gl.Uniform1i(Gl.GetUniformLocation(terrainShader.ProgramID, "grassTex"), 0);
            Gl.Uniform1i(Gl.GetUniformLocation(terrainShader.ProgramID, "stoneTex"), 1);
        }

        public static int Width { get { return length * spacing; } }

        public Terrain(int _x, int _y)
        {
            (int, int) position = (_x, _y);
            Position = position;
            terrainMap.Add(position, this);
            List<Vector3> vertices = new List<Vector3>();
            List<Vector3> normals = new List<Vector3>();
            List<Vector2> uv = new List<Vector2>();
            List<int> elements = new List<int>();


            heightFunction = HeightGenerator.GetHeight;
            for (int x = 0; x <= length; x++)
            {
                for (int z = 0; z <= length; z++)
                {
                    vertices.Add(new Vector3(x * spacing, CalculateHeight(x, z), z * spacing));
                    uv.Add(new Vector2((float)x / length, (float)z / length));
                }
            }

            for (int x = 0; x <= length; x++)
            {
                for (int z = 0; z <= length; z++)
                {
                    normals.Add(CalculateNormal(x, z));
                }
            }

            for (int x = 0; x < length; x++)
            {
                for (int z = 0; z < length; z++)
                {
                    int len = length + 1;
                    elements.Add(z + x * len);
                    elements.Add(z + 1 + x * len);
                    elements.Add((x + 1) * len + z);
                    elements.Add(z + 1 + x * len);
                    elements.Add((x + 1) * len + z + 1);
                    elements.Add((x + 1) * len + z);
                }
            }

            Vao = new VAO(terrainShader, new VBO<Vector3>(vertices.ToArray()), new VBO<Vector3>(normals.ToArray()), new VBO<Vector2>(uv.ToArray()), new VBO<int>(elements.ToArray(), BufferTarget.ElementArrayBuffer));
        }
        Random random = new Random();

        private static Dictionary<(int, int), float> heightMap = new Dictionary<(int, int), float>();
        private Func<int, int, float> heightFunction;

        private float CalculateHeight(int x, int z)
        {
            x = x + Position.X * length;
            z = z + Position.Z * length;
            if (!heightMap.ContainsKey((x, z)))
                heightMap.Add((x, z), heightFunction(x * spacing, z * spacing));
            return heightMap[(x, z)];
        }
        private Vector3 CalculateNormal(int x, int z)
        {

            //float[] heights = new float[] { CalculateHeight(x - 1, z), CalculateHeight(x + 1, z), CalculateHeight(x, z - 1) , CalculateHeight(x, z + 1) };
            //Vector3 normal = new Vector3(heights[0] - heights[1], 2 , heights[2] - heights[3]);


            Vector3 v1 = new Vector3((x - 1) * spacing, CalculateHeight(x - 1, z), z) - new Vector3((x + 1) * spacing, CalculateHeight(x + 1, z), z);
            Vector3 v2 = new Vector3(x, CalculateHeight(x, z - 1), (z - 1) * spacing) - new Vector3(x, CalculateHeight(x, z + 1), (z + 1) * spacing);
            Vector3 normal = Vector3.Cross(v2, v1);
            //Vector3 u = new Vector3(spacing, 0.0f, CalculateHeight(x + 1, z) - CalculateHeight(x - 1, z));
            //Vector3 v = new Vector3(0.0f, spacing, CalculateHeight(x, z + 1) - CalculateHeight(x, z - 1));
            //Vector3 normal = Vector3.Cross(u, v);
            normal = normal.Normalize();
            //if (normal.Y < 0)
            //    normal = -normal;

            return normal;
        }


        public static float GetHeight(float x, float z)
        {

            int xi = (int)Math.Floor(x) / spacing;
            int zi = (int)Math.Floor(z) / spacing;
            float xf = (x - xi * spacing) / spacing;
            float zf = (z - zi * spacing) / spacing;

            (int tx, int tz) = GetTerrainTile(x, z);

            Terrain t;
            if (!terrainMap.ContainsKey((tx, tz)))
                return 0;
            else t = terrainMap[(tx, tz)];

            xi -= tx * length;
            zi -= tz * length;

            if (xf <= 1 - zf)
                return Tools.BarryCentric(
                    new Vector3(0, t.CalculateHeight(xi, zi), 0),
                    new Vector3(1, t.CalculateHeight(xi + 1, zi), 0),
                    new Vector3(0, t.CalculateHeight(xi, zi + 1), 1),
                    new Vector2(xf, zf));
            return Tools.BarryCentric(
                    new Vector3(1, t.CalculateHeight(xi + 1, zi), 0),
                    new Vector3(1, t.CalculateHeight(xi + 1, zi + 1), 1),
                    new Vector3(0, t.CalculateHeight(xi, zi + 1), 1),
                    new Vector2(xf, zf));
        }
        public static (int, int) GetTerrainTile(float x, float z) => ((int)Math.Floor(x / Width), (int)Math.Floor(z / Width));
        public static Terrain GetTerrain(int x, int y)
        {
            if (!terrainMap.ContainsKey((x, y)))
            {
                Console.WriteLine("new");
                new Terrain(x, y);
            }
            return terrainMap[(x, y)];
        }

        public void Render()
        {
            Gl.ActiveTexture(0);
            Gl.BindTexture(grassTexture);
            Gl.ActiveTexture(1);
            Gl.BindTexture(stoneTexture);
            Vao.Program["transformation_matrix"]?.SetValue(Matrix4.CreateTranslation(new Vector3(Position.X * spacing * length, 0, Position.Z * spacing * length)));
            Vao.Draw();
        }

        public static void Dispose()
        {
            foreach (var r in terrainMap)
            {
                r.Value.Vao.DisposeChildren = true;
                r.Value.Vao.Dispose();
            }
        }
        static class HeightGenerator
        {

            private static int seed = 10000;
            private static float amplitude = 100f;
            static OpenSimplexNoise noise;

            static HeightGenerator()
            {
                noise = new OpenSimplexNoise(seed);
            }

            public static float GetHeight(int x, int z)
            {
                float total = 0;

                total += GetInterpolatedNoise(x / 128f, z / 128f);
                total += GetInterpolatedNoise(x / 8f, z / 8f) / 128f;

                return total * amplitude;
            }

            private static float GetInterpolatedNoise(float x, float z)
            {
                int xi = (int)Math.Floor(x);
                int zi = (int)Math.Floor(z);
                float xf = x - xi;
                float zf = z - zi;
                float[] v = new float[] { GetSmoothNoise(xi, zi), GetSmoothNoise(xi + 1, zi), GetSmoothNoise(xi, zi + 1), GetSmoothNoise(xi + 1, zi + 1) };
                float[] i = new float[] { Interpolate(v[0], v[1], xf), Interpolate(v[2], v[3], xf) };
                return Interpolate(i[0], i[1], zf);
            }

            private static float Interpolate(float a, float b, float blend)
            {
                double theta = blend * Math.PI;
                float f = (float)(1 - Math.Cos(theta)) * 0.5f;
                return a * (1 - f) + b * f;
            }

            private static float GetSmoothNoise(int x, int z)
            {
                return (
                    GetNoise(x - 1, z - 1) +
                    GetNoise(x + 1, z - 1) +
                    GetNoise(x - 1, z + 1) +
                    GetNoise(x - 1, z + 1) +
                    GetNoise(x - 1, z) +
                    GetNoise(x + 1, z) +
                    GetNoise(x, z + 1) +
                    GetNoise(x, z - 1) +
                    GetNoise(x, z)
                    ) / 9f;
            }

            private static Dictionary<(int, int), float> noiseMap = new Dictionary<(int, int), float>();
            private static float GetNoise(int x, int z)
            {
                if (!noiseMap.ContainsKey((x, z)))
                {
                    noiseMap.Add((x, z), (float)noise.Eval(x + int.MaxValue / 2, z + int.MaxValue / 2));
                }
                return noiseMap[(x, z)];
                //return (float)noise.Eval(x, z);
            }



        }
    }
}
