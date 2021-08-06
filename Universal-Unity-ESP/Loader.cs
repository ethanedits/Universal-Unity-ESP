using UnityEngine;

namespace Universal_Unity_ESP
{
    public class Loader
    {
        public static void Init()
        {
            Loader.Load = new UnityEngine.GameObject();
            Loader.Load.AddComponent<Hacks>();
            UnityEngine.Object.DontDestroyOnLoad(Loader.Load);
        }

        public static void Unload()
        {
            _Unload();
        }

        private static void _Unload()
        {
            GameObject.Destroy(Load);
        }

        private static GameObject Load;
    }
}
