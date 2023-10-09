using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Pool;
using Object = UnityEngine.Object;

namespace Script.Custom.CoroutineHelper
{
    public static class Co
    {
        private static CoHandler s_Handle
        {
            get
            {
                if (s_ == null)
                {
                    s_ = new GameObject("Coroutine Helper").AddComponent<CoHandler>();
                    Object.DontDestroyOnLoad(s_);
                }

                return s_;
            }
        }
        
        private static CoHandler s_;

        public static void Start(ref Coroutine c, IEnumerator j)
        {
            Stop(ref c);
            c = s_Handle.StartCoroutine(j);
        }

        public static void Stop(ref Coroutine c)
        {
            if (c == null)
                return;
            
            s_Handle.StopCoroutine(c);
            c = null;
        }

        public static WFS GetWFS() => GenericPool<WFS>.Get();
        
        public class WFS : CustomYieldInstruction, IDisposable
        {
            private float m_WaitTime = -1f;
            private float m_MaxTime = -1f;

            public override bool keepWaiting
            {
                get
                {
                    if (m_MaxTime <= m_WaitTime)
                    {
                        m_WaitTime = 0f;
                        return false;
                    }

                    m_WaitTime += Time.deltaTime;
                    return true;
                }
            }

            public WFS Set(float time)
            {
                m_WaitTime = 0f;
                m_MaxTime = time;
                return this;
            }

            public void Dispose()
            {
                GenericPool<WFS>.Release(this);
            }
        }
        
        private class CoHandler : MonoBehaviour { }
    }

}
