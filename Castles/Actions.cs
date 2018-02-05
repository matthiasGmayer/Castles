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
        private static List<IScrollGetter> scrollGetters = new List<IScrollGetter>();
        private static Vector2 mousePosition, lastMousePosition;
        private static float scroll, lastScroll;
        public static void Update(float delta)
        {
            mousePosition = new Vector2(Mouse.GetState().X, Mouse.GetState().Y);
            if (mousePosition != lastMousePosition)
                foreach (IMouseGetter m in mouseGetters)
                    m.OnMouseMoved(lastMousePosition - mousePosition);
            lastMousePosition = mousePosition;

            scroll = Mouse.GetState().WheelPrecise;
            if (scroll != lastScroll)
                foreach (IScrollGetter s in scrollGetters)
                    s.OnScroll(lastScroll - scroll);

            lastScroll = scroll;
        }

        public static void Subscribe(object o)
        {
            if (o is IMouseGetter i)
            {
                mouseGetters.Add(i);
            }
            if (o is IScrollGetter s)
            {
                scrollGetters.Add(s);
            }
        }


    }
}
