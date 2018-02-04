using OpenTK.Input;
using System.Collections.Generic;
using System.Numerics;

namespace Castles
{
    public static class Actions
    {
        private static Dictionary<Key, bool> keyMap = new Dictionary<Key, bool>();

        public static void SetPressed(Key key, bool pressed) => keyMap[key] = pressed;

        public static bool IsPressed(Key key)
        {
            if (keyMap.ContainsKey(key))
                return keyMap[key];
            return false;
        }

        private static List<IMouseGetter> mouseGetters = new List<IMouseGetter>();
        private static Vector2 mousePosition, lastMousePosition;

        public static void Update(float delta)
        {
            mousePosition = new Vector2(Mouse.GetState().X, Mouse.GetState().Y);
            //if (mousePosition != lastMousePosition)
                foreach (IMouseGetter m in mouseGetters)
                    m.OnMouseMoved(lastMousePosition - mousePosition);
            lastMousePosition = mousePosition;
            //if (App.app.CursorVisible = !App.app.Focused)
            //{
            //    Mouse.SetPosition(300, 300);
            //}
        }

        public static void Subscribe(object o)
        {
            if (o is IMouseGetter i)
            {
                mouseGetters.Add(i);
            }
        }


    }
}
