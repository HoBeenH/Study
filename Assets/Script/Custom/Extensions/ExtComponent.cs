using Script.Custom.CustomDebug;
using UnityEngine;

namespace Script.Custom.Extensions
{
    public static class ExtComponent 
    {
        public static bool TryGetOrAddComp<T>(this GameObject go, out T comp) where T : Component
        {
            comp = null;
            if (go == null)
            {
                D.E($"GameObject Is Null : {typeof(T)}");
                return false;
            }

            if (!go.TryGetComponent(out comp))
                comp = go.AddComponent<T>();

            return comp != null;
        }
    }
}
