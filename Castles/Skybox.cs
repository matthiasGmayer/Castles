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

        public static float size = 100f;

        private SkyModel model;

        private class SkyModel : Model
        {
            public override void Bind()
            {

            }
            public SkyModel(VAO vao) : base(vao, null)
            {}
        }

        public Model Model => model;

        public Skybox()
        {
            Console.WriteLine(  skyboxShader.ProgramLog);
            uint id = Loader.LoadCubeMap("Sky");
            model = new SkyModel(Geometry.CreateCube(skyboxShader, new Vector3(-size / 2f), new Vector3(size / 2f)));
        }

        public void Dispose()
        {
            Model.Dispose();
        }
    }
}
