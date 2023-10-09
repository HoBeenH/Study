using System;
using System.Collections.Generic;
using Script.Base.MonoSingleTone;
using Script.Base.Pool;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;

namespace Script.Manager.ResourceMgr
{
    public class ResourceManager : MonoSingleTone<ResourceManager>
    {
        private readonly Dictionary<string, IPool> r_PoolDic = new Dictionary<string, IPool>();
        private readonly HashSet<string> r_LockSet = new HashSet<string>();
        
        private Transform m_PoolRoot = null;
        
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
            var _go = new GameObject("Pool Root");
            m_PoolRoot = _go.transform;
            m_PoolRoot.localPosition = Vector3.zero;
            m_PoolRoot.SetParent(transform);
        }

        protected override void OnClose()
        {
            Addressables.ClearResourceLocators();
            Addressables.CleanBundleCache();
            foreach (var pool in r_PoolDic)
                pool.Value.Clear();

            r_PoolDic.Clear();
            ClearMemory(false, false);
        }

        public AsyncOperationHandle AwaitAddressable() =>
            Addressables.InitializeAsync();
        
        #region API

        #region Static

        public static Transform GetPoolRoot() => Instance.m_PoolRoot;
        
        public static void LoadObjAsync<T>(string path, Action<T> onLoadComplete) where T : UnityEngine.Object
        {
            if (!IsValid(path, false))
                return;
            
            Instance.LoadObj(path, onLoadComplete);
        }      
        
        public static void LoadGOAsync(string path, Action<GameObject> onLoadComplete, InstantiationParameters? paramOrNull = null)
        {
            if (!IsValid(path, false))
                return;
            
            var _param = new InstantiationParameters(null, true);
            if (paramOrNull.HasValue)
                _param = paramOrNull.Value;

            Instance._LoadGO(path, onLoadComplete, _param);
        }    
        
        public static void LoadCompAsync<T>(string path, Action<T> onLoadComplete, InstantiationParameters? paramOrNull = null, bool enableValidCheck = true) where T : Component
        {
            if (enableValidCheck)
                if (!IsValid(path, false))
                    return;
            
            var _param = new InstantiationParameters(null, true);
            if (paramOrNull.HasValue)
                _param = paramOrNull.Value;
            
            Instance._LoadComp(path, onLoadComplete, _param);
        }
        
        public static T LoadObjSync<T>(string path, Action<T> onLoadComplete) where T : UnityEngine.Object
        {
            if (!IsValid(path, true))
                return null;

            var _obj = Instance._LoadObjSync<T>(path);
            Instance.Logger.As(_obj != null, $"Can't Load {typeof(T)}");
            onLoadComplete?.Invoke(_obj);
            return _obj;
        }        
                
        public static T LoadCompSync<T>(string path, Action<T> onLoadComplete, InstantiationParameters? paramOrNull = null) where T : UnityEngine.Object
        {
            if (!IsValid(path, true))
                return null;
            
            var _param = new InstantiationParameters(null, true);
            if (paramOrNull.HasValue)
                _param = paramOrNull.Value;

            var _obj = Instance._LoadGOSync(path, _param);
            Instance.Logger.As(_obj != null, $"Can't Load {typeof(T)}");

            _obj.TryGetComponent<T>(out var _result);
            Instance.Logger.As(_result != null, $"Can't Get Comp {typeof(T)}");
            
            onLoadComplete?.Invoke(_result);
            return _result;
        }
        
        public static GameObject LoadGOSync(string path, Action<GameObject> onLoadComplete, InstantiationParameters? paramOrNull = null)
        {
            if (!IsValid(path, true))
                return null;
            
            var _param = new InstantiationParameters(null, true);
            if (paramOrNull.HasValue)
                _param = paramOrNull.Value;
            
            var _obj = Instance._LoadGOSync(path, _param);
            Instance.Logger.As(_obj != null, $"Can't Load GameObject {path}");
            onLoadComplete?.Invoke(_obj);
            return _obj;
        }        
        
        public static T LoadPool<T>(string path, bool isSync = true, Action<T> callback = null) where T : PoolMonoObj
        {
            if (isSync)
                if (!IsValid(path, true))
                    return null;

            var _poolDic = Instance.r_PoolDic;

            if (!_poolDic.TryGetValue(path, out var _poolBase))
            {
                if (isSync)
                    _poolBase = new SyncPool<T>(path);
                else
                    _poolBase = new AsyncPool<T>(path);
                
                _poolDic.Add(path, _poolBase);
            }

            switch (_poolBase)
            {
                case SyncPool<T> _syncPool:
                    return _syncPool.GetObj();
                case AsyncPool<T> _asyncPool:
                    _asyncPool.GetObj(callback);
                    return null;
            }

            return null;
        }   

        public static void ReleaseObj<T>(T obj) => Addressables.Release(obj);

        public static void ReleaseGO(GameObject go) => Addressables.ReleaseInstance(go);

        public static void ReleaseGO(Component co) => ReleaseGO(co.gameObject);
        
        #endregion

        private void LoadObj<T>(string path, Action<T> onLoadComplete) where T : UnityEngine.Object =>
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
                r_LockSet.Remove(path);
            };

        private void _LoadGO(string path, Action<GameObject> onLoadComplete, InstantiationParameters param) =>
            Addressables.InstantiateAsync(path, param).Completed += handle =>
            {
                switch (handle.Status)
                {
                    case AsyncOperationStatus.Succeeded:
                        onLoadComplete?.Invoke(handle.Result);
                        break;
                    default:
                    case AsyncOperationStatus.Failed:
                    case AsyncOperationStatus.None:
                        Logger.E($"Can't Load GameObject {path}");
                        if (handle.Result != null)
                            ReleaseObj(handle.Result);

                        break;
                }
                r_LockSet.Remove(path);
            };

        private void _LoadComp<T>(string path, Action<T> onLoadComplete, InstantiationParameters param) where T : class
        {
            _LoadGO(path, go =>
            {
                if (go.TryGetComponent<T>(out var _result))
                    onLoadComplete?.Invoke(_result);
                else
                    Logger.E($"Can't Get Comp {typeof(T)}");
            }, param);
        }

        private T _LoadObjSync<T>(string path) where T : UnityEngine.Object
        {
            var _result = Addressables.LoadAssetAsync<T>(path).WaitForCompletion();
            Logger.As(_result != null, $"The {typeof(T)} Is NULL\n{path}");

            return _result;
        }

        private GameObject _LoadGOSync(string path, InstantiationParameters param)
        {
            var _result = Addressables.InstantiateAsync(path, param).WaitForCompletion();
            Logger.As(_result != null, $"The GameObject Is NULL\n{path}");
            return _result;
        }

        private T LoadCompSync<T>(string path, InstantiationParameters param) where T : class
        {
            LoadGOSync(path, null, param).TryGetComponent<T>(out var _result);
            Logger.As(_result != null,$"Can't Get Comp {typeof(T)}");
            return null;
        }

        
        #endregion //API

        private static bool IsValid(object obj, bool isSync)
        {
            switch (obj)
            {
                case string path:
                {
                    var _result = string.IsNullOrEmpty(path);
                    if (_result)
                        Instance.Logger.E($"Path Can Not Be Null Or Empty");

                    if (!isSync)
                    {
                        var _isFirst = Instance.r_LockSet.Add(path);
                        if (!_isFirst)
                            Instance.Logger.E($"Already Loading");

                        _result = !_result && _isFirst;
                    }

                    return _result;
                }
            }

            return false;
        }
    }
}