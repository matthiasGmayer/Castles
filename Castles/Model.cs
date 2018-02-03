using System;
using OpenGL;

namespace Castles
{
    public class Model
    {

        public float Reflectivity { get; set; }
        public float ShineDamper { get; set; }

        private VAO vao;
        private Texture texture;

        public ShaderProgram Program { get { return vao.Program; } }

        public Model(VAO vao, Texture texture)
        {
            this.vao = vao;
            this.texture = texture;
            Reflectivity = 0.1f;
            ShineDamper = 10f;
        }



        public void Render()
        {
            Program["reflectivity"].SetValue(Reflectivity);
            Program["shineDamper"].SetValue(ShineDamper);

            if (texture != null)
                Gl.BindTexture(texture);
            vao.Draw();
        }

        internal void Dispose()
        {
            vao.DisposeChildren = true;
            vao.Dispose();
            texture?.Dispose();            
        }
    }
}
