using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using JetBrains.Annotations;
using Script.Base.MonoSingleTone;
using Script.Custom.CustomEnum;
using Script.Custom.Extensions;
using Script.Manager.ResourceMgr;
using Script.Manager.TableMgr;
using Script.Parameter.Enum;
using Script.TableParser;
using Script.UI.UIBase;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.EventSystems;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;

namespace Script.Manager.UIMgr
{
    public class UIManager : MonoSingleTone<UIManager>
    {
        private readonly Dictionary<ECanvasType, UICanvasInfo> r_DicCanvas = new Dictionary<ECanvasType, UICanvasInfo>();
        private readonly Dictionary<EAddressableID, UIBase> r_DicUI = new Dictionary<EAddressableID, UIBase>();
        
        private readonly HashSet<EAddressableID> r_LockSet = new HashSet<EAddressableID>();
        private readonly HashSet<EAddressableID> r_OpenUI = new HashSet<EAddressableID>();
        
        private EventSystem m_EventSystem = null;
        private StandaloneInputModule m_InputModule = null;
        
        protected override void OnInit()
        {
            if (m_EventSystem == null)
                m_EventSystem = gameObject.AddComponent<EventSystem>();
            
            if (m_InputModule == null)
                m_InputModule = gameObject.AddComponent<StandaloneInputModule>();
            
            var _startIdx = (ECanvasType.None + 1).ToInt();
            for (var i = _startIdx; i < ECanvasType.End.ToInt(); i++)
            {
                var _type = i.ToEnum<ECanvasType>();
                r_DicCanvas.TryAdd(_type, new UICanvasInfo(transform, _type));
            }

            DOTween.Init(false, false, LogBehaviour.Default);
        }

        protected override void OnClose()
        {
            foreach (var pair in r_DicCanvas)
                pair.Value.Clear();
            
            r_DicCanvas.Clear();
        }
        
        private void OpenUI<T>(EAddressableID id, ECanvasType type, [NotNull] Action<T> onLoadComplete) where T : UIBase
        {
            if (r_DicUI.TryGetValue(id, out var _result))
            {
                var _asT = _result as T;
                if (_asT == null)
                {
                    Logger.E($"Can't Parse {typeof(T).Name}");
                    return;
                }
                r_OpenUI.Add(id);
                onLoadComplete.Invoke(_asT);
            }
            else
            {
                CreateUI<T>(id, new InstantiationParameters(r_DicCanvas[type].Root, false), ui =>
                {
                    r_OpenUI.Add(id);
                    onLoadComplete.Invoke(ui);
                });
            }
        }

        private void CreateUI<T>(EAddressableID id, InstantiationParameters param, Action<T> onLoadComplete) where T : UIBase
        {
            if (!IsValid<T>(id))
                return;

            var _tableData = TableManager.Instance.GetTable<AddressableTable>().GetData(id);
            if (_tableData == null)
            {
                Logger.E($"Can't Find {id.ToString()}");
                return;
            }
            
            ResourceManager.LoadCompAsync<T>(_tableData.Path, x =>
            {
                x.OnCreate();
                r_DicUI.Add(id, x);
                onLoadComplete.Invoke(x);
                r_LockSet.Remove(id);
            }, param);
        }

        private bool IsValid<T>(EAddressableID id)
        {
            if (!r_LockSet.Add(id))
            {
                Logger.E($"Already Loading {typeof(T).Name}");
                return false;
            }

            return true;
        }
        
        public void OpenUI<T>(EAddressableID id, [NotNull] Action<T> onLoadComplete) where T : UIMainBase
            => OpenUI(id, ECanvasType.UIMain, onLoadComplete);

        public void OpenMessageBox<T>(EAddressableID id, [NotNull] Action<T> onLoadComplete) where T : UIMessageBoxBase
            => OpenUI(id, ECanvasType.UIMessageBox, onLoadComplete);
        
        public void CloseUI<T>(T ui) where T : UIBase
        {
            if (!r_OpenUI.Contains(ui.AddressableID))
            {
                Logger.E($"Not Open UI {ui.AddressableID.ToString()}");
                return;
            }
            r_OpenUI.Remove(ui.AddressableID);
        }

        public void RefreshOpenUI()
        {
            foreach (var id in r_OpenUI)
                if (r_DicUI.TryGetValue(id, out var _ui))
                    _ui.RefreshUI();
        }

        public IEnumerator AwaitPreLoad()
        {
            var _loadHandler = Addressables.LoadResourceLocationsAsync("PreLoad");
            while (!_loadHandler.IsDone)
                yield return null;

            if (_loadHandler.Status != AsyncOperationStatus.Succeeded)
                yield break;

            var _set = _loadHandler.Result.ToHashSet();
            foreach (var location in _loadHandler.Result)
            {
                var _param = new InstantiationParameters(r_DicCanvas[ECanvasType.UIMain].Root, false);
                Addressables.InstantiateAsync(location, _param).Completed += ao =>
                {
                    if (ao.Status != AsyncOperationStatus.Succeeded)
                        return;

                    ao.Result.SetActive(false);
                    if (!ao.Result.TryGetComponent(out UIBase _ui))
                    {
                        _set.Remove(location);
                        return;
                    }
                
                    _ui.OnCreate();
                    _ui.RectTr.SetParent(r_DicCanvas[_ui.CanvasType].Root);
                    r_DicUI.Add(_ui.AddressableID, _ui);
                    _set.Remove(location);
                };
            }

            while (_set.Count > 0)
                yield return null;
        }
    }
}
