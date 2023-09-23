using System;
using System.Collections.Generic;
using DG.Tweening;
using JetBrains.Annotations;
using Script.Base.MonoSingleTone;
using Script.Custom.CustomEnum;
using Script.EnumField;
using Script.Manager.ResourceMgr;
using Script.Manager.TableMgr;
using Script.TableParser;
using Script.UI.UIBase;
using UnityEngine.EventSystems;
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

            EventSystem.current.IsPointerOverGameObject();

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
                x.SetID(id);
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
    }
}
