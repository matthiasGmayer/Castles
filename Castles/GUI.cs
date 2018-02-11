using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenGL;
using System.Numerics;
namespace Castles
{
    public class GUITexture : IGUI
    {
        public static ShaderProgram guiShader = Shaders.GetShader("GUI");
        public Model Model { get; }
        public GUITexture(Texture t, Vector2 position, Vector2 size)
        {
            Model = new Model(Geometry.CreateQuad(guiShader, position, size), t);
        }
    }
}
