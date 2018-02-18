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

        public float yaw, pitch, roll;

        public Camera(Vector3 position, float jaw, float pitch, float roll)
        {
            Position = position;
            this.yaw = jaw;
            this.pitch = pitch;
            this.roll = roll;
        }

        public Camera(Camera camera)
        {
            Position = camera.Position;
        }

        public void SetView(ShaderProgram program)
        {
            program.Use();
            program["view_matrix"]?.SetValue(GetViewMatrix());
            Gl.UseProgram(0);
        }
        public void SetRotateView(ShaderProgram program)
        {
            program.Use();
            program["rotate_view_matrix"]?.SetValue(GetRotateViewMatrix());
            Gl.UseProgram(0);
        }

        protected virtual Matrix4 GetViewMatrix()
        {
            return
                Matrix4.CreateTranslation(-Position) *
                Matrix4.CreateRotationY(yaw) *
                Matrix4.CreateRotationX(pitch) *
                Matrix4.CreateRotationZ(roll);
        }
        protected virtual Matrix4 GetRotateViewMatrix() => GetViewMatrix().RemoveColumn(3, 3);
        
    }

    class EntityCamera : Camera, IUpdatable, IMouseGetter, IScrollGetter
    {
        public Entity Entity { get; set; }
        private Vector3 targetLook;
        public Vector3 Offset { get; set; }

        private float distance;
        private float minDistance = 10f;
        private float maxDistance = 1000f;
        public float Distance { get => distance; set { distance = targetDistance = Math.Min(Math.Max(value, minDistance), maxDistance); } }
        private float targetDistance;
        private float targetPitch;
        private float targetYaw;

        private float TargetDistance { get => targetDistance; set { targetDistance = Math.Min(Math.Max(value, minDistance), maxDistance); } }

        public EntityCamera(Entity entity) : base(new Vector3(), 20,0,0)
        {
            Entity = entity;
            Distance = 100f;
            Actions.Subscribe(this);
            
        }

        public void Update(float delta)
        {
            float factor = (float)Math.Pow(0.01f, delta);
            targetLook = Entity.Position - ((Entity.Position - targetLook) * factor);
            distance = targetDistance - ((targetDistance - distance) * factor);
            yaw = targetYaw - ((targetYaw - yaw) * factor);
            pitch = targetPitch - ((targetPitch - pitch) * factor);


            float angle = (float)Math.PI - yaw;
            float h = (float)Math.Cos(pitch) * Distance;
            float v = (float)Math.Sin(pitch) * Distance;
            Position = new Vector3(-h * (float)Math.Sin(angle), v, -h * (float)Math.Cos(angle)) + targetLook;





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

        public void OnMouseMoved(Vector2 change)
        {
            targetYaw -= change.X / 1000f;
            targetPitch -= change.Y / 1000f;
        }

        public void OnScroll(float change)
        {
            TargetDistance += change * 10f;
        }
    }
}
