using System;
using System.Collections.Generic;
using Script.Base.Logger;
using Script.Custom.CustomDebug;
using UnityEngine;

namespace Script.Base.MonoSingleTone
{
    public abstract class MonoSingleTone<T> : MonoBehaviour, ILogger<T> where T : MonoSingleTone<T>
    {
        public ILogger<T> Logger => this;
        
        private static T m_Instance = null;

        public static T Instance
        {
            get
            {
                if (m_Instance == null)
                {
                    if (m_Instance == null)
                        m_Instance = FindObjectOfType<T>();
                    
                    if (m_Instance == null)
                    {
                        var _go = new GameObject($"[{typeof(T).Name} SingleTone]");
                        m_Instance = _go.AddComponent<T>();
                        DontDestroyOnLoad(_go);
                        m_Instance.Logger.L("Create Mono SingleTone");
                        m_Instance.Init();
                    }
                    
                    D.As(m_Instance != null, $"Can't Create {typeof(T)}");
                }

                return m_Instance;
            }
        }

        public static bool HasInstance() => m_Instance != null;

        private bool m_IsInit = false;
        
        protected abstract void OnInit();

        protected abstract void OnClose();
        
        public void Init()
        {
            if (m_IsInit)
                return;

            m_IsInit = true;
            Logger.L("Init");
            OnInit();
        }

        public void Close()
        {
            if (!m_IsInit)
                return;

            m_IsInit = false;
            OnClose();
        }
    }
}
