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
    class Skybox : IDisposable, IRenderable, IUpdatable, ITransformable
    {
        public static ShaderProgram skyboxShader = Shaders.GetShader("Sky");
       

        private Model model;
        public Model Model => model;

        public Skybox(string file)
        {
            model = new Model(Geometry.CreateCube(skyboxShader, new Vector3(Graphics.viewDistance/2), new Vector3(-Graphics.viewDistance/2)), Loader.LoadCubeMap(file));
        }

        public void Dispose()
        {
            Model.Dispose();
        }

        float time;

        public void Update(float delta)
        {
            time += delta;
        }

        public Matrix4 GetTransformationMatrix() => Matrix4.CreateRotationY(time / 100f);
    }
}
