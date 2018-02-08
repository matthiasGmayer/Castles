using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenGL;
using System.Numerics;

namespace Castles
{
    public class Entity : IDisposable, IRenderable, ITransformable
    {
        public Model Model { get; }

        public Vector3 Rotation { get; set; }
        public float Scale { get; set; }
        public Vector3 Position { get; set; }
        private Entity parent;
        public Entity Parent
        {
            get => parent;
            set
            {
                parent?.childs.Remove(this);
                parent = value;
                parent?.childs.Add(this);
            }
        }
        private List<Entity> childs = new List<Entity>();
        public IEnumerable<Entity> Childs { get => childs; }


        public Entity(Model model, Vector3 position = new Vector3(), Vector3 rotation = new Vector3(), float scale = 1, Entity parent = null)
        {
            Position = position;
            Rotation = rotation;
            Scale = scale;
            Parent = parent;
            Model = model;
        }

        public Entity(Model model, Entity entity) : this(model, entity.Position, entity.Rotation, entity.Scale, entity.Parent) { }

        public virtual void Render()
        {
            Model?.Program.Use();
            Model?.Program["transformation_matrix"]?.SetValue(GetTransformationMatrix());
            Model?.Render();
        }

        public virtual Matrix4 GetTransformationMatrix()
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
            Model?.Dispose();
        }
    }
}
