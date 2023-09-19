using System;
using System.Collections.Generic;
using Script.Base.MonoSingleTone;
using Script.Base.Pool;
using Script.Custom.CustomDebug;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Script.Manager
{
    public class ResourceManager : MonoSingleTone<ResourceManager>
    {
        private readonly Dictionary<Type, IPool> m_PoolDic = new Dictionary<Type, IPool>();
        
        private void ClearMemory(bool unloadUnusedAsset, bool useGCCollect)
        {
            Addressables.ClearResourceLocators();
            Addressables.CleanBundleCache();
            
            if (unloadUnusedAsset)
                Resources.UnloadUnusedAssets();
            
            if (useGCCollect)
                GC.Collect();
        }
        
        protected override void OnInit()
        {
            Addressables.InitializeAsync().WaitForCompletion();
        }

        protected override void OnClose()
        {
            Addressables.ClearResourceLocators();
            Addressables.CleanBundleCache();
            foreach (var pool in m_PoolDic)
                pool.Value.Clear();
            
            m_PoolDic.Clear();
            ClearMemory(false, false);
        }

        #region API

        public T LoadPoolObj<T>(string path) where T : PoolMonoObj
        {
            var _type = typeof(T);
            if (!m_PoolDic.TryGetValue(_type, out var _poolBase))
            {
                _poolBase = new Pool<T>(path);
                m_PoolDic.Add(_type, _poolBase);
            }

            if (_poolBase is Pool<T> _pool)
                return _pool.GetObj();

            return null;
        }

        public void LoadObj<T>(string path, Action<T> onLoadComplete) where T : UnityEngine.Object
        {
            Addressables.LoadAssetAsync<T>(path).Completed += handle =>
            {
                switch (handle.Status)
                {
                    case AsyncOperationStatus.Succeeded:
                        onLoadComplete?.Invoke(handle.Result);
                        break;
                    default:
                    case AsyncOperationStatus.Failed:
                    case AsyncOperationStatus.None:
                        Logger.E($"Can't Load {typeof(T)} {path}");
                        if (handle.Result != null)
                            ReleaseObj(handle.Result);
                        
                        break;
                }
            };
        }

        public void LoadGO(string path, Action<GameObject> onLoadComplete)
        {
            Addressables.InstantiateAsync(path).Completed += handle =>
            {
                switch (handle.Status)
                {
                    case AsyncOperationStatus.Succeeded:
                        onLoadComplete?.Invoke(handle.Result);
                        break;
                    default:
                    case AsyncOperationStatus.Failed:
                    case AsyncOperationStatus.None:
                        Logger.E($"Can't Load {path}");
                        if (handle.Result != null)
                            ReleaseGO(handle.Result);
                        
                        break;
                }
            };
        }

        public void LoadComp<T>(string path, Action<T> onLoadComplete) where T : Component
        {
            LoadGO(path, go =>
            {
                if (go.TryGetComponent<T>(out var _result))
                    onLoadComplete?.Invoke(_result);
                
                Logger.E($"Can't Get Comp {typeof(T)}");
            });
        }

        public T LoadObjSync<T>(string path) where T : UnityEngine.Object
        {
            var _handle = Addressables.LoadAssetAsync<T>(path);
            var _result = _handle.WaitForCompletion();
            D.As(_result != null, $"The {typeof(T)} Is NULL\n{path}");
            
            return _result;
        }

        public GameObject LoadGOSync(string path)
        {
            var _handle = Addressables.InstantiateAsync(path);
            var _result = _handle.WaitForCompletion();
            D.As(_result != null, $"The GameObject Is NULL\n{path}");
            
            return _result;
        }

        public T LoadCompSync<T>(string path) where T : Component
        {
            var _go = LoadGOSync(path);
            if (_go.TryGetComponent<T>(out var _result))
                return _result;
            
            Logger.E($"Can't Get Comp {typeof(T)}");
            return null;
        }

        public void ReleaseObj<T>(T obj) => Addressables.Release(obj);

        public void ReleaseGO(GameObject go) => Addressables.ReleaseInstance(go);

        public void ReleaseGO(Component co) => ReleaseGO(co.gameObject);

        public void ReleasePool<T>(T poolObj) where T : PoolMonoObj
        {
            if (poolObj == null)
                return;
            
            var _type = typeof(T);
            if (m_PoolDic.TryGetValue(_type, out var _result) && _result is Pool<T> _pool)
                _pool.ReleaseObj(poolObj);
            else
            {
                Logger.E($"Can't Release Pool Obj {typeof(T).Name}");
                Destroy(poolObj);
            }
        }
        
        #endregion //API
    }
}