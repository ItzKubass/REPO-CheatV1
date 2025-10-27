using System;
using UnityEngine;

namespace REPO
{
    public class Loader
    {
        public static void Init()
        {
            Load = new GameObject();
            Load.AddComponent<Hacks>();
            UnityEngine.Object.DontDestroyOnLoad(Load);
        }

        public static void Unload()
        {
            _Unload();
        }

        private static void _Unload()
        {
            UnityEngine.Object.Destroy(Load);
        }

        private static GameObject Load;
    }
}
