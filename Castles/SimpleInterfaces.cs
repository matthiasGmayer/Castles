using OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Castles
{
   // public interface IDisposable { void Dispose(); }
    public interface IUpdatable { void Update(float delta); }
    public interface IRenderable { Model Model { get;  } }
    public interface ITransformable { Matrix4 GetTransformationMatrix(); }
    public interface IMouseGetter { void OnMouseMoved(Vector2 change); }
    public interface IScrollGetter { void OnScroll(float change); }
}
