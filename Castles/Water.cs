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
        public static Texture reflection;
        public static Texture refraction;
        public static Texture refractionDepth;

        static Water()
        {
            waterShader.Use();
            waterShader.SetTexture("reflectionTex", 1);
            waterShader.SetTexture("refractionTex", 0);
            waterShader.SetTexture("dudvTex", 2);
            waterShader.SetTexture("normalTex", 3);
            waterShader.SetTexture("depthTex", 4);
            waterShader.SetTexture("skyTex", 5);
            reflection = new Texture(Graphics.fbos[FrameBuffers.waterReflection].TextureID[0]);
            refraction = new Texture(Graphics.fbos[FrameBuffers.waterRefraction].TextureID[0]);
            refractionDepth = new Texture(Graphics.fbos[FrameBuffers.waterDepth].DepthID);
            //refractionDepth = new Texture(Graphics.fbos[FrameBuffers.waterRefraction].DepthID);
        }

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
        public float WaveSize { get; set; }
        public float WaveStrength { get; set; }

        public Water(Vector3 position, Vector2 scale, float waveSize = 1, float waveStrength = 1)
        {
            Position = position;
            Scale = scale;
            WaveSize = waveSize;
            WaveStrength = waveStrength;
        }

        public void Bind()
        {
            waterShader.Use();
            quad.Vao.BindAttributes(waterShader);
            waterShader["transformation_matrix"].SetValue(GetTransformationMatrix());
            waterShader["size"]?.SetValue(Scale);
            waterShader["waveSize"]?.SetValue(WaveSize);
            waterShader["waveStrength"]?.SetValue(WaveStrength);
            Graphics.Bind(refraction);
            Graphics.Bind(reflection, 1);
            Graphics.Bind(Loader.LoadTexture("!DuDv"), 2);
            Graphics.Bind(Loader.LoadTexture("!WaterNormal"), 3);
            Graphics.Bind(refractionDepth, 4);
            Graphics.Bind(new Texture(Graphics.fbos[FrameBuffers.skyBox].TextureID[0]), 5);

        }

        public Matrix4 GetTransformationMatrix() => Matrix4.CreateScaling(new Vector3(Scale.X, 0, Scale.Y)) * Matrix4.CreateTranslation(Position);
        
    }
}
