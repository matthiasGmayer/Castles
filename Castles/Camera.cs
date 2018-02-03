using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenGL;
using System.Numerics;

namespace Castles
{
    class Camera
    {
        public Vector3 Position { get; set; }
        public Vector3 Rotation { get; set; }

        public Camera(Vector3 position, Vector3 rotation)
        {
            Position = position;
            Rotation = rotation;
        }

        public void SetView(ShaderProgram program)
        {
            ProgramParam p = program["view_matrix"];
            if (p == null)
                Console.WriteLine("view_matrix not found in shader");
            p.SetValue(GetViewMatrix());
        }

        protected virtual Matrix4 GetViewMatrix()
        {
            return
                Matrix4.CreateRotationX(-Rotation.X) *
                Matrix4.CreateRotationY(-Rotation.Y) *
                Matrix4.CreateRotationZ(-Rotation.Z) *
                Matrix4.CreateTranslation(-Position);
        }
    }

    class EntityCamera : Camera, IUpdatable
    {
        public Entity Entity { get; set; }
        private Vector3 targetLook;
        public Vector3 Offset { get; set; }
        public float Distance { get; set; }
        public EntityCamera(Entity entity) : base(new Vector3(), new Vector3())
        {
            Entity = entity;
            Distance = 30f;
        }

        public void Update(float delta)
        {
            float factor = (float)Math.Pow(0.2f, delta);
            targetLook = Entity.Position - ((Entity.Position - targetLook) * new Vector3(factor));
            Vector3 targetPos = Entity.Position - (Entity.Position - Position).Normalize() * new Vector3(Distance);
            Position = targetPos - ((targetPos - Position) * new Vector3(factor));
        }

        protected override Matrix4 GetViewMatrix() => Matrix4.LookAt(Position, targetLook + Offset, Vector3.UnitY);
        
    }
}
