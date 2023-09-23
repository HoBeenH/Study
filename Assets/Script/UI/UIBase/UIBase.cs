using Script.Custom.CustomDebug;
using Script.EnumField;
using Script.Manager.UIMgr;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Script.UI.UIBase
{
    public abstract class UIBase : UIBehaviour
    {
        [Header("# BG Btn")] 
        [SerializeField] private Button m_BtnBG = null;

        public EAddressableID AddressableID { get; private set; }

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

        public void SetID(EAddressableID id)
        {
            AddressableID = id;
        } 

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
