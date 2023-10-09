using Script.Custom.CustomDebug;
using Script.Manager.UIMgr;
using Script.Parameter.Enum;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Script.UI.UIBase
{
    public abstract class UIBase : UIBehaviour
    {
        [Header("# BG Btn")] 
        [SerializeField] private Button m_BtnBG = null;

        public abstract EAddressableID AddressableID { get; }
        public abstract ECanvasType CanvasType { get; }
        
        public RectTransform RectTr
        {
            get
            {
                if (m_rt == null)
                    m_rt = transform as RectTransform;

                return m_rt;
            }
        }
        private RectTransform m_rt = null;

        public virtual void OnCreate() { }

        public virtual void RefreshUI() { }

        public virtual void SetActiveUI(bool isActive)
        {
            gameObject.SetActive(isActive);
            if (!isActive)
            {
                UIManager.Instance.CloseUI(this);
            }
        }
    }
}
