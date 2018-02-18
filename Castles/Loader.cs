using OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.IO;
using ImageSharp;

namespace Castles
{
    public static class Loader
    {
        public static readonly string defaultResources = "..\\..\\Resources\\";
        public static readonly string defaultObj = defaultResources + "Models\\";
        public static readonly string defaultDiffuse = defaultResources + "Textures\\";
        public static readonly string defaultNormal = defaultResources + "NormalMaps\\";
        public static readonly string defaultSpecular = defaultResources + "SpecularMaps\\";
        public static readonly string defaultDisplacement = defaultResources + "Displacement\\";
        public static readonly string defaultOcclusion = defaultResources + "Occlusion\\";
        public static readonly string defaultSpec = defaultResources + "Specs\\";


        public static Dictionary<string, Model> modelMap = new Dictionary<string, Model>();
        public static Dictionary<string, Texture> textureMap = new Dictionary<string, Texture>();

        private static Func<string, string> dictionaryFunction = s => s;


        public static TexturePack LoadTextures(string file)
        {
            file = file.Replace("*!", "");
            return new TexturePack(
                LoadTexture("!" + file),
                LoadTexture("n!" + file),
                LoadTexture("s!" + file),
                LoadTexture("d!" + file),
                LoadTexture("o!" + file));
        }
        public static Texture LoadTexture(string file)
        {
            file = file.Replace("d!", defaultDisplacement);
            file = file.Replace("o!", defaultOcclusion);
            file = file.Replace("n!", defaultNormal);
            file = file.Replace("s!", defaultSpecular);
            file = file.Replace("!", defaultDiffuse);
            if (!Path.HasExtension(file))
            {
                if (File.Exists(file + ".png"))
                    file = file + ".png";
                else if (File.Exists(file + ".jpg"))
                    file = file + ".jpg";
            }


            string name = dictionaryFunction(file);
            Texture t;
            if (!textureMap.ContainsKey(name))
            {
                if (!File.Exists(file))
                    return null;

                textureMap.Add(name, t = new Texture(file));
                

            }
            else
                t = textureMap[name];

            return t;
        }

        public static Texture LoadCubeMap(string file)
        {
            file = file.Replace("n!", defaultNormal);
            file = file.Replace("s!", defaultSpecular);
            file = file.Replace("!", defaultDiffuse);


            string name = dictionaryFunction(file);
            if (!textureMap.ContainsKey(name))
            {
                Gl.ActiveTexture(0);
                Gl.Enable(EnableCap.TextureCubeMap);
                Texture t;
                textureMap.Add(name, t = new Texture());
                Gl.BindTexture(TextureTarget.TextureCubeMap, t.ID);
                for (int i = 0; i < 6; i++)
                {
                    Texture.TextureGl(TextureTarget.TextureCubeMapPositiveX + i, file + "/" + nameMap[i] + ".png", System.Drawing.RotateFlipType.RotateNoneFlipNone);
                }
                Gl.TexParameteri(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapS, TextureParameter.ClampToEdge);
                Gl.TexParameteri(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapT, TextureParameter.ClampToEdge);
                Gl.TexParameteri(TextureTarget.TextureCubeMap, TextureParameterName.TextureMagFilter, TextureParameter.Linear);
                Gl.TexParameteri(TextureTarget.TextureCubeMap, TextureParameterName.TextureMinFilter, TextureParameter.Linear);
                //Gl.TexParameteri(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapR, TextureParameter.ClampToEdge);
            }
            return textureMap[name];
        }

        private static Dictionary<int, string> nameMap = new Dictionary<int, string>();
        static Loader()
        {
            int h = 0;
            nameMap.Add(h++, "left");
            nameMap.Add(h++, "right");
            nameMap.Add(h++, "top");
            nameMap.Add(h++, "bottom");
            nameMap.Add(h++, "front");
            nameMap.Add(h++, "back");
        }
        public static Model LoadModel(string file, ShaderProgram program)
        {

            if (!Path.HasExtension(file))
                file = file + ".obj";
            file = file.Replace("!", defaultObj);
            string save = dictionaryFunction(file);
            if (modelMap.ContainsKey(save))
                return modelMap[save];
            string name = Path.GetFileNameWithoutExtension(file);

            List<Vertex> verticesList = new List<Vertex>();
            List<Vector3> normalsList = new List<Vector3>();
            List<Vector2> textureList = new List<Vector2>();
            List<int> elementList = new List<int>();


            IEnumerable<string> data = System.IO.File.ReadLines(file);
            //read data
            int index = 0;
            foreach (string line in System.IO.File.ReadLines(file))
            {
                string[] s = line.Split(' ');
                if (line.StartsWith("v "))
                    verticesList.Add(new Vertex(new Vector3(ToFloat(s[1]), ToFloat(s[2]), ToFloat(s[3])), index++));
                else if (line.StartsWith("vn "))
                    normalsList.Add(new Vector3(ToFloat(s[1]), ToFloat(s[2]), ToFloat(s[3])));
                else if (line.StartsWith("vt "))
                    textureList.Add(new Vector2(ToFloat(s[1]), ToFloat(s[2])));
            }

            //insert data
            foreach (string line in File.ReadLines(file))
            {
                if (line.StartsWith("f "))
                {
                    string[] strings = line.Split(' ');
                    if (strings.Length == 5)
                    {
                        ProcessFace(new string[] { strings[1], strings[2], strings[3] }, elementList, verticesList, normalsList, textureList, ref index);
                        ProcessFace(new string[] { strings[1], strings[3], strings[4] }, elementList, verticesList, normalsList, textureList, ref index);
                    }
                    else
                    {
                        ProcessFace(new string[] { strings[1], strings[2], strings[3] }, elementList, verticesList, normalsList, textureList, ref index);
                    }
                }
            }

            int length = verticesList.Count;
            Vector3[] verticesArray = new Vector3[length];
            Vector3[] normalsArray = new Vector3[length];
            Vector2[] textureArray = new Vector2[length];
            Vector3[] tangentArray = new Vector3[length];

            foreach (Vertex v in verticesList)
            {

                int vertexIndex = v.Index;
                verticesArray[vertexIndex] = v.Position;
                normalsArray[vertexIndex] = normalsList[v.NormalIndex ?? 0];
                textureArray[vertexIndex] = textureList[v.TextureIndex ?? 0];

                if (v.Tangents.Count > 0)
                    tangentArray[vertexIndex] = (v.Tangents.Aggregate((a, b) => a + b) / v.Tangents.Count).Normalize();
            }


            VBO<Vector3> vertices = new VBO<Vector3>(verticesArray);
            VBO<Vector3> normals = new VBO<Vector3>(normalsArray);
            VBO<Vector2> texture = new VBO<Vector2>(textureArray);
            VBO<Vector3> tangent = new VBO<Vector3>(tangentArray);
            VBO<int> elements = new VBO<int>(elementList.ToArray(), BufferTarget.ElementArrayBuffer);





            Model m = new Model(new VAO(program, vertices, normals, tangent, texture, elements), LoadTextures(name));
            modelMap.Add(save, m);

            //specs
            CSV specs = new CSV(defaultSpec + Path.GetFileNameWithoutExtension(file));
            if (specs.Any())
            {
                LoadSpec(s => m.Reflectivity = float.Parse(s.Replace('.', ',')), "reflectivity", specs);
                LoadSpec(s => m.ShineDamper = float.Parse(s.Replace('.', ',')), "shineDamper", specs);
            }
            return m;
        }
        private static void ProcessFace(string[] vertices, List<int> elementList, List<Vertex> verticesList, List<Vector3> normalsList, List<Vector2> textureList, ref int index)
        {
            List<Vertex> v = new List<Vertex>();
            foreach (string str in vertices)
            {
                string[] strs = str.Split('/');
                v.Add(ProcessVertex(strs, elementList, verticesList, normalsList, textureList, ref index));
            }
            Vector3 tangent = CalculateTangent(v, textureList);
            v.ForEach(a => { a.Tangents.Add(tangent); });
        }

        private static Vector3 CalculateTangent(List<Vertex> v, List<Vector2> textureList)
        {
            Vector3 dPos1 = v[1].Position - v[0].Position;
            Vector3 dPos2 = v[2].Position - v[0].Position;
            Vector2 dUV1 = textureList[(int)v[1].TextureIndex] - textureList[(int)v[0].TextureIndex];
            Vector2 dUV2 = textureList[(int)v[2].TextureIndex] - textureList[(int)v[0].TextureIndex];
            return (dPos1 * dUV2.Y - dPos2 * dUV1.Y) / (dUV1.X * dUV2.Y - dUV1.Y * dUV2.X);
        }

        private static Vertex ProcessVertex(string[] vertex, List<int> elementList, List<Vertex> verticesList, List<Vector3> normalsList, List<Vector2> textureList, ref int index)
        {
            int vertexIndex = int.Parse(vertex[0]) - 1;
            int texIndex = int.Parse(vertex[1]) - 1;
            int normalIndex = int.Parse(vertex[2]) - 1;
            Vertex v = verticesList[vertexIndex];
            if (v.IsSet())
            {
                Vertex v2 = v.GetVertexWithIndices(texIndex, normalIndex);
                if (v2 == null)
                {
                    v2 = v.AddDuplicate(texIndex, normalIndex, index++);
                    verticesList.Add(v2);
                }
                elementList.Add(v2.Index);
                return v2;
            }
            else
            {
                v.TextureIndex = texIndex;
                v.NormalIndex = normalIndex;
                elementList.Add(v.Index);
                return v;
            }


        }
        private static void LoadSpec(Action<string> f, string name, CSV csv)
        {
            string s = csv?[name]?[1];
            if (s != null)
                f(s);
        }

        private static float ToFloat(string s)
        {
            return float.Parse(s.Replace('.', ','));
        }
    }

    class Vertex
    {

        public Vector3 Position { get; }
        public int? TextureIndex { get; set; }
        public int? NormalIndex { get; set; }
        private List<Vertex> duplicateVertex = new List<Vertex>();
        public List<Vector3> Tangents { get; }
        public int Index { get; }
        //private float length;

        public Vertex(Vector3 position, int? textureIndex, int? normalIndex, int index, List<Vector3> tangent)
        {
            Position = position;
            TextureIndex = textureIndex;
            NormalIndex = normalIndex;
            Index = index;
            //length = position.Length();
            Tangents = tangent;
        }
        public Vertex(Vector3 position, int? textureIndex, int? normalIndex, int index) : this(position, textureIndex, normalIndex, index, new List<Vector3>()) { }
        public Vertex(Vector3 position, int index) : this(position, null, null, index) { }

        public Vertex AddDuplicate(int? tex, int? normal, int index)
        {
            Vertex v = new Vertex(Position, tex, normal, index, Tangents);
            duplicateVertex.Add(v);
            return v;
        }

        public bool IsSet() => TextureIndex != null && NormalIndex != null;
        public bool HasSameIndices(int? tex, int? normal) => tex == TextureIndex && normal == NormalIndex;
        public bool HasSameIndices(Vertex x) => HasSameIndices(x.TextureIndex, x.NormalIndex);
        public Vertex GetVertexWithIndices(int? tex, int? normal)
        {
            if (HasSameIndices(tex, normal))
                return this;
            foreach (Vertex v in duplicateVertex)
            {
                if (v.HasSameIndices(tex, normal))
                    return v;
            }
            return null;
        }
    }
}
