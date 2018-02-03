using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenGL;
using System.Numerics;

namespace Castles
{
    public class Entity : IDisposable
    {
        private Model model;

        public Vector3 Rotation { get; set; }
        public float Scale { get; set; }
        public Vector3 Position { get; set; }
        public Entity Parent { get; set; }


        public Entity(Model model, Vector3 position, Vector3 rotation, float scale, Entity parent)
        {
            Position = position;
            Rotation = rotation;
            Scale = scale;
            Parent = parent;
            this.model = model;
        }

        public Entity(Model model, Vector3 position, Vector3 rotation, float scale) : this(model, position, rotation, scale, null) { }

        public virtual void Render()
        {
            model.Program.Use();
            model.Program["transformation_matrix"].SetValue(GetTransformationMatrix());
            model.Render();
        }

        protected virtual Matrix4 GetTransformationMatrix()
        {

            Matrix4 transformation = Matrix4.CreateScaling(new Vector3(Scale, Scale, Scale)) *
                Matrix4.CreateRotationX(Rotation.X) *
                Matrix4.CreateRotationY(Rotation.Y) *
                Matrix4.CreateRotationZ(Rotation.Z) *
                Matrix4.CreateTranslation(Position);

            
            return Parent == null ? transformation : Parent.GetTransformationMatrix() * transformation;
        }

        public void Dispose()
        {
            model.Dispose();   
        }
    }
}
