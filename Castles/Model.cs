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
        public bool HasNormalMapping => normalMap != null;
        public bool HasSpecularMapping => specularMap != null;

        public VAO Vao { get; }
        private Texture texture, normalMap, specularMap;

        public ShaderProgram Program { get { return Vao.Program; } }


        public Model(VAO vao, Texture texture, Texture normalMap = null, Texture specularMap = null)
        {
            this.Vao = vao;
            this.texture = texture;
            this.normalMap = normalMap;
            this.specularMap = specularMap;
            Reflectivity = 1f;
            ShineDamper = 100f;
            models.Add(this);
        }

        public virtual void Bind()
        {
            if (texture != null)
                Graphics.Bind(texture);
            if (normalMap != null)
                Graphics.Bind(normalMap, 1);
            if (specularMap != null)
                Graphics.Bind(specularMap, 2);
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
