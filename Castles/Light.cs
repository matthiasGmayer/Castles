using System.Numerics;
using OpenGL;

namespace Castles
{
    
    public abstract class Light
    {
        public Light(Vector3 color)
        {
            Color = color;
        }

        public Vector3 Color { get; set; }
        public abstract void SetUniforms(int i, ShaderProgram program);
    }
    public class PointLight : Light
    {
        public PointLight(Vector3 position, Vector3 color, Vector3 attenuation) : base(color)
        {
            Position = position;
            Attenuation = attenuation;
        }
        public PointLight(Vector3 position, Vector3 color) : this(position, color, new Vector3(1, 0, 0)) { }
        public Vector3 Attenuation { get; set; }
        public Vector3 Position { get; set; }
        public override void SetUniforms(int i, ShaderProgram program)
        {
            program.Use();

            Gl.ProgramUniform3fv(program.ProgramID, Gl.GetUniformLocation(program.ProgramID, "pointLight["+i+"]"), 3, Position.ToFloat());
            Gl.ProgramUniform3fv(program.ProgramID, Gl.GetUniformLocation(program.ProgramID, "pointLightColor[" + i + "]"), 3, Color.ToFloat());
            Gl.ProgramUniform3fv(program.ProgramID, Gl.GetUniformLocation(program.ProgramID, "attenuation[" + i + "]"), 3, Attenuation.ToFloat());
        }
    }
    public class DirectionalLight : Light
    {

        public DirectionalLight(Vector3 position, Vector3 color) : base(color)
        {
            Position = position;
        }
        Vector3 Position { get; set; }
        public override void SetUniforms(int i, ShaderProgram program)
        {
            Gl.ProgramUniform3fv(program.ProgramID, Gl.GetUniformLocation(program.ProgramID, "dirLight[" + i + "]"), 3, Position.ToFloat());
            Gl.ProgramUniform3fv(program.ProgramID, Gl.GetUniformLocation(program.ProgramID, "dirLightColor[" + i + "]"), 3, Color.ToFloat());
        }
    }
}
