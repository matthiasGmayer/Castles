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
        public virtual Vector3 Position { get; set; }
        public Vector3 Rotation { get; set; }

        public Camera(Vector3 position, Vector3 rotation)
        {
            Position = position;
            Rotation = rotation;
        }

        public void SetView(ShaderProgram program)
        {
            program.Use();
            program["view_matrix"]?.SetValue(GetViewMatrix());
            Gl.UseProgram(0);
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

    class EntityCamera : Camera, IUpdatable, IMouseGetter, IScrollGetter
    {
        public Entity Entity { get; set; }
        private Vector3 targetLook;
        public Vector3 Offset { get; set; }

        public float Distance { get; set; }
        private float targetDistance;

        public float Horizontal { get; private set; }
        public float Vertical{ get; private set;}

        public override Vector3 Position {
            get
            {
                return targetLook + new Vector3((float)Math.Cos(Horizontal), (float)Math.Sin(Vertical), (float)Math.Sin(Horizontal)) * Distance;
            }
            //set
            //{
            //    Vector3 pos = value;
            //    Vector3 t = targetLook + Offset;
            //    Vector2 pos2 = new Vector2(pos.X, pos.Y);
            //    Vector2 t2 = new Vector2(t.X, t.Y);
            //    Horizontal = (float)Tools.GetAngle(t2, pos2);
            //    Vertical = (float)Tools.GetAngle(new Vector2(0, pos.Y), new Vector2((pos2 - t2).Length(), t.Y));
            //    minDistance = (value - pos).Length();
            //}
        }


        public EntityCamera(Entity entity) : base(new Vector3(), new Vector3())
        {
            Entity = entity;
            Distance = 100f;
            Actions.Subscribe(this);
        }

        public void Update(float delta)
        {
            float factor = (float)Math.Pow(0.1f, delta);
            targetLook = Entity.Position - ((Entity.Position - targetLook) * new Vector3(factor));
            Distance = targetDistance - ((targetDistance - Distance) * factor);
            //Vector3 targetPos = Entity.Position - (Entity.Position - Position).Normalize() * new Vector3(Distance);
            //Position = targetPos - ((targetPos - Position) * new Vector3(factor));
            //Vector3 p = Position;
            //Vector3 t = targetLook;
            //float h = Terrain.GetHeight(p.X, p.Z) + 1f;
            //if(p.Y < h)
            //{
            //    Vertical = 0.26f * (float)Math.PI - Tools.GetAngle(new Vector2(0, p.Y), new Vector2((new Vector2(t.X,t.Z)-new Vector2(p.X,p.Z)).Length(), t.Z));
            //}
        }

        protected override Matrix4 GetViewMatrix() => Matrix4.LookAt(Position, targetLook + Offset, Vector3.UnitY);

        public void OnMouseMoved(Vector2 change)
        {
            Horizontal -= change.X / 1000f;
            Vertical -= change.Y / 1000f;
        }

        public void OnScroll(float change)
        {
            targetDistance += change * 10f;
        }
    }
}
