using System;
using Script.Base.Logger;
using Script.Custom.CustomDebug;
using Script.Manager;
using UnityEngine.Pool;

namespace Script.Base.Pool
{
    public class Pool<T> : IDisposable, IPool, ILogger<T> where T : PoolMonoObj
    {
        public ILogger<T> Logger => this;

        private ObjectPool<T> m_Pool = null;
        private readonly string r_Path = string.Empty;

        public Pool(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                Logger.E($"Can Not Be Null Or Empty Path");
                return;
            }

            r_Path = path;
            m_Pool = new ObjectPool<T>(Create, Get, Release, Destroy);
        }

        private T Create() => ResourceManager.Instance.LoadCompSync<T>(r_Path);

        public T GetObj() => m_Pool.Get();

        public void ReleaseObj(T obj) => m_Pool.Release(obj);

        public void Clear() => Dispose();
        
        public void Dispose()
        {
            m_Pool.Clear();
            m_Pool = null;
        }

        private void Get(T obj)
        {
            if (obj == null)
                return;

            obj.Get();
        }

        private void Release(T obj)
        {
            if (obj == null)
                return;

            obj.Clear();
        }

        private void Destroy(T obj)
        {
            if (obj == null)
                return;

            ResourceManager.Instance.ReleaseGO(obj);
        }
    }
}