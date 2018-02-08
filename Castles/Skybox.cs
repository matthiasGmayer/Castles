using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenGL;
using System.Numerics;
using System.Drawing;

namespace Castles
{
    class Skybox : IDisposable, IRenderable
    {
        public static ShaderProgram skyboxShader = Shaders.GetShader("Sky");

        public static float size = 10000f;

        private SkyModel model;
        static uint id = Loader.LoadCubeMap("!Sky2");
        private class SkyModel : Model
        {
            public override void Bind()
            {
                Gl.ActiveTexture(0);
                Gl.BindTexture(TextureTarget.TextureCubeMap, id);
            }
            public SkyModel(VAO vao) : base(vao, null)
            {}
        }

        public Model Model => model;

        public Skybox()
        {
            Console.WriteLine(  skyboxShader.ProgramLog);
           
            model = new SkyModel(Geometry.CreateCube(skyboxShader, new Vector3(size / 2f), new Vector3(-size / 2f)));
        }

        public void Dispose()
        {
            Model.Dispose();
        }
    }
}
