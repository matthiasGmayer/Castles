using System;
using OpenGL;
using System.Collections.Generic;

namespace Castles
{
    public class Model
    {
        private static List<Model> models = new List<Model>();
        public float Reflectivity { get; set; }
        public float ShineDamper { get; set; }
        public bool HasNormalMapping => Textures.Normal != null;
        public bool HasSpecularMapping => Textures.Specular != null;

        public VAO Vao { get; }
        private TexturePack Textures { get; }

        public ShaderProgram Program { get { return Vao.Program; } }


        public Model(VAO vao, TexturePack textures)
        {
            Vao = vao;
            Textures = textures;
            Reflectivity = 1f;
            ShineDamper = 100f;
            models.Add(this);
        }
        public Model(VAO vao, Texture diffuse) : this(vao, new TexturePack(diffuse))
        {
        }

        public Model(VAO vao) : this(vao, (TexturePack)null)
        {
        }

        public virtual void Bind()
        {
            Graphics.Bind(Textures);
            Gl.BindVertexArray(Vao.ID);
        }

        public static void Dispose()
        {
            foreach (Model m in models)
            {
                m.Vao.DisposeChildren = true;
                m.Vao.Dispose();
            }
        }
    }
}
