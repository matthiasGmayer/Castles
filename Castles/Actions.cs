using OpenTK.Input;
using System.Collections.Generic;
namespace Castles
{
    public static class Actions
    {
        private static Dictionary<Key, bool> keyMap = new Dictionary<Key, bool>();

        public static void SetPressed(Key key, bool pressed)=> keyMap[key] = pressed;

        public static bool IsPressed(Key key)
        {
            if (keyMap.ContainsKey(key))
                return keyMap[key];
            return false;
        }
    }
}
