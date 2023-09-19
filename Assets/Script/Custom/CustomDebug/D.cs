namespace Script.Custom.CustomDebug
{
    public static class D
    {
        public const string ENABLE_DEBUG_SYMBOL =
#if UNITY_EDITOR
            "UNITY_EDITOR";
#else // UNITY_EDITOR
            "USE_LOG";
#endif // !UNITY_EDITOR

        [System.Diagnostics.Conditional(ENABLE_DEBUG_SYMBOL)]
        public static void L(object msg) => UnityEngine.Debug.Log(msg);

        [System.Diagnostics.Conditional(ENABLE_DEBUG_SYMBOL)]
        public static void W(object msg) => UnityEngine.Debug.LogWarning(msg);
        
        [System.Diagnostics.Conditional(ENABLE_DEBUG_SYMBOL)]
        public static void E(object msg) => UnityEngine.Debug.LogError(msg);

        [System.Diagnostics.Conditional(ENABLE_DEBUG_SYMBOL)]
        public static void As(bool condition, object msg)
        {
            if (condition)
                return;
            
            UnityEngine.Debug.LogError(msg);
        }
        
        [System.Diagnostics.Conditional(ENABLE_DEBUG_SYMBOL)]
        public static void DL(UnityEngine.Vector3 start, UnityEngine.Vector3 end, UnityEngine.Color color, float duration) => 
            UnityEngine.Debug.DrawLine(start, end, color, duration, true);
        
        [System.Diagnostics.Conditional(ENABLE_DEBUG_SYMBOL)]
        public static void DL(UnityEngine.Vector3 start, UnityEngine.Vector3 end, UnityEngine.Color color) => 
            UnityEngine.Debug.DrawLine(start, end, color, 0.0f,true);
        
        [System.Diagnostics.Conditional(ENABLE_DEBUG_SYMBOL)]
        public static void DrawLine(UnityEngine.Vector3 start, UnityEngine.Vector3 end) => 
            UnityEngine.Debug.DrawLine(start, end, UnityEngine.Color.white, 0.0f, true);
    }
}
