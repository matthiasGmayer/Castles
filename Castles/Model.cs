using System;
using OpenGL;

namespace Castles
{
    public class Model
    {

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
        }



        public void Render()
        {
            Program["reflectivity"].SetValue(Reflectivity);
            Program["shineDamper"].SetValue(ShineDamper);
            if (texture != null)
                Gl.BindTexture(texture);
            Vao.Draw();
        }

        public void Bind()
        {
            Gl.BindVertexArray(Vao.ID);
        }

        internal void Dispose()
        {
            Vao.DisposeChildren = true;
            Vao.Dispose();
        }
    }
}
