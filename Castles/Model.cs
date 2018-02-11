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

        public VAO Vao { get; }
        private Texture texture;

        public ShaderProgram Program { get { return Vao.Program; } }

        public Model(VAO vao, Texture texture)
        {
            this.Vao = vao;
            this.texture = texture;
            Reflectivity = 1f;
            ShineDamper = 100f;
            models.Add(this);
        }



        public void Render()
        {
            Program["reflectivity"]?.SetValue(Reflectivity);
            Program["shineDamper"]?.SetValue(ShineDamper);
            if (texture != null)
                Graphics.Bind(texture);
            Vao.Draw();
        }

        public virtual void Bind()
        {
            if (texture != null)
                Graphics.Bind(texture);
            else
                Gl.BindTexture(TextureTarget.Texture2D, 0);

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
