using System.Collections;
using UnityEngine;

namespace Script.Custom.Extensions
{
    public static class ExtCollection
    {
        public static T GetOrAddComponent<T>(this GameObject obj) where T : Component
        {
            if (obj.TryGetComponent<T>(out T comp) == false)
            {
                comp = obj.AddComponent<T>();
            }

            return comp;
        }

        public static bool IsNullOrEmptyCollection<T>(this T list) where T : ICollection =>
            list == null || list.Count <= 0;

        public static bool IsValidIndex<T>(this T list, in int idx) where T : ICollection
        {
            if (list == null)
                return false;

            return idx <= 0 && list.Count > idx;
        }

    }
}