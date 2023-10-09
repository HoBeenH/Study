using System;
using System.Collections.Generic;
using Script.Base.Logger;
using Script.Manager.ResourceMgr;
using UnityEngine.Pool;
using UnityEngine.ResourceManagement.ResourceProviders;
using Object = UnityEngine.Object;

namespace Script.Base.Pool
{
    public class SyncPool<T> : IDisposable, IPool, ILogger<T> where T : PoolMonoObj
    {
        public ILogger<T> Logger => this;

        private ObjectPool<T> m_Pool = null;
        private readonly string r_Path = string.Empty;

        public SyncPool(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                Logger.E($"Can Not Be Null Or Empty Path");
                return;
            }

            r_Path = path;
            m_Pool = new ObjectPool<T>(Create, Get, Release, Destroy);
        }

        private T Create() => ResourceManager.LoadCompSync<T>(r_Path, null,
            new InstantiationParameters(ResourceManager.GetPoolRoot(), false));

        private void Get(T obj)
        {
            obj.Get();
            obj.SetHandler(this);
        }

        public void Release(Object obj)
        {
            var _asT = obj as T;
            if (_asT != null)
            {
                _asT.Clear();
                _asT.transform.SetParent(ResourceManager.GetPoolRoot());
            }
        }

        private void Destroy(T obj) => ResourceManager.ReleaseGO(obj);

        public T GetObj() => m_Pool.Get();

        public void ReleaseObj(T obj) => m_Pool.Release(obj);

        public void Clear() => Dispose();

        public void Dispose()
        {
            m_Pool.Dispose();
            m_Pool = null;
        }
    }  
    
    public class AsyncPool<T> : IDisposable, IPool, ILogger<T> where T : PoolMonoObj
    {
        public ILogger<T> Logger => this;

        private Queue<T> m_Pool = null;
        private readonly string r_Path = string.Empty;

        public AsyncPool(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                Logger.E($"Can Not Be Null Or Empty Path");
                return;
            }

            r_Path = path;
            m_Pool = new Queue<T>();
        }

        private void Create(Action<T> callback)
        {
            ResourceManager.LoadCompAsync(r_Path, callback, new InstantiationParameters(ResourceManager.GetPoolRoot(), false), false);
        }

        public void Release(Object obj) => ReleaseObj(obj as T);

        private void Destroy(T obj) => ResourceManager.ReleaseGO(obj);

        public void GetObj(Action<T> callback)
        {
            if (m_Pool.Count > 0)
                callback.Invoke(m_Pool.Dequeue());
            else
                Create(callback);
        }

        public void ReleaseObj(T obj)
        {
            obj.Clear();
            m_Pool.Enqueue(obj);
        }

        public void Clear() => Dispose();

        public void Dispose()
        {
            foreach (var obj in m_Pool)
                ResourceManager.ReleaseGO(obj);
            
            m_Pool.Clear();
            m_Pool = null;
        }
    }
}