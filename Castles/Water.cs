using OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

namespace Castles
{
    public class Water
    {
        public static ShaderProgram waterShader = Shaders.GetShader("Water");

        public static Model quad = new Model(new VAO(waterShader,
            new VBO<Vector3>(new Vector3[]
            {
                new Vector3(-0.5f,0, 0.5f),
                new Vector3(0.5f, 0, 0.5f),
                new Vector3(-0.5f,0, -0.5f),
                new Vector3(0.5f, 0, -0.5f)
            }),
            new VBO<int>(new int[] 
            {
                0,1,2,3,2,1
            }, BufferTarget.ElementArrayBuffer, BufferUsageHint.StaticRead)), null);
        public Vector3 Position { get; set; }
        public Vector2 Scale { get; set; }

        public Water(Vector3 position, Vector2 scale)
        {
            Position = position;
            Scale = scale;
        }

        public Matrix4 GetTransformationMatrix() => Matrix4.CreateScaling(new Vector3(Scale.X, 0, Scale.Y)) * Matrix4.CreateTranslation(Position);
        
    }
}
