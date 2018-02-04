using OpenGL;
using System;
using System.Collections.Generic;
using System.IO;

namespace Castles
{
    public static class Shaders
    {
        private static Dictionary<string, ShaderProgram> shaderMap = new Dictionary<string, ShaderProgram>();

        private static void AddShader(string name)
        {
            try
            {
                ShaderProgram p = GetShader(name);
                if (p != null)
                    return;
                string file = "../../shaders/" + name;
                Console.WriteLine(file);
                p = new ShaderProgram(File.ReadAllText(file + ".vert"), File.ReadAllText(file + ".frag"));
                shaderMap.Add(name, p);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public static IEnumerable<ShaderProgram> GetShaders()
        {
            return shaderMap.Values;
        }

        static Shaders()
        {
            foreach (string file in Directory.EnumerateFiles("../../shaders"))
            {
                string s = Path.GetFileNameWithoutExtension(file);
                if (shaderMap.ContainsKey(s))
                    continue;
                AddShader(s);
            }
        }

        public static void Dispose()
        {
            foreach(var r in shaderMap)
            {
                r.Value.DisposeChildren = true;
                r.Value.Dispose();
            }
        }


        public static ShaderProgram GetShader(string name) => shaderMap.ContainsKey(name) ? shaderMap[name] : null;
            

    }
}
