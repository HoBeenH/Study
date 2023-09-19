using UnityEngine;

namespace Script.Custom.Extensions
{
    public static class ExtVector
    {
        public static Vector3 Add(this Vector3 a, Vector3 b) => a + b;
    }
}