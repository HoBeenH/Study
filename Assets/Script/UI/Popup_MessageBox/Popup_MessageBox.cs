using System;
using DG.Tweening;
using Script.Custom.CustomEnum;
using Script.Parameter.Enum;
using Script.Parameter.Struct;
using Script.UI.UIBase;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Script.UI.Popup_MessageBox
{
    public class Popup_MessageBox : UIMessageBoxBase
    {
        [Header("# Title")]
        [SerializeField] private TextMeshProUGUI m_TitleText = null;
        [SerializeField] private TextMeshProUGUI m_DescText = null;
        [SerializeField] private TextMeshProUGUI m_OkText = null;
        [SerializeField] private TextMeshProUGUI m_CancelText = null;

        [Header("# Button")] 
        [SerializeField] private Button m_ExitBtn = null;
        [SerializeField] private Button m_OkBtn = null;
        [SerializeField] private Button m_CancelBtn = null;
        [SerializeField] private Button m_BGBtn = null;

        [Header("# Tween Target")] 
        [SerializeField] private Transform m_Target = null;

        public override EAddressableID AddressableID => EAddressableID.Popup_MessageBox;

        private void SetActivateButton(EMessageBoxBtnFlag btnFlag)
        {
            m_OkBtn.gameObject.SetActive(btnFlag.HasFlagFast(EMessageBoxBtnFlag.Ok));
            m_CancelBtn.gameObject.SetActive(btnFlag.HasFlagFast(EMessageBoxBtnFlag.Cancel));
            m_ExitBtn.gameObject.SetActive(btnFlag.HasFlagFast(EMessageBoxBtnFlag.Exit));
            m_BGBtn.interactable = btnFlag.HasFlagFast(EMessageBoxBtnFlag.BG);
        }

        private void SetString(MessageBoxParameter param)
        {
            m_TitleText.text = param.Title;
            m_DescText.text = param.Desc;
            m_OkText.text = param.Ok;
            m_CancelText.text = param.Cancel;
        }

        private UnityAction OnClick(Action callback) => () =>
        {
            callback?.Invoke();
            SetActiveUI(false);
        };

        public override void OnCreate()
        {
            CreateActivateTween();
        }

        protected override void CreateActivateTween()
        {
            if (m_OpenTween == null)
            {
                var _sequence = DOTween.Sequence().SetAutoKill(false).Pause();
                
                _sequence.Append(m_Target.DOScale(Vector3.zero, 0f));
                _sequence.Append(m_Target.DOScale(Vector3.one, 0.05f));
                _sequence.OnPlay(() =>
                {
                    m_Target.localScale = Vector3.zero;
                });
                _sequence.OnComplete(() => { m_Target.localScale = Vector3.one; });
                m_OpenTween = _sequence;
            }

            if (m_CloseTween == null)
            {
                var _sequence = DOTween.Sequence().SetAutoKill(false).Pause();
                
                _sequence.Append(m_Target.DOScale(Vector3.zero, 0.05f));
                _sequence.OnPlay(() => m_Target.localScale = Vector3.one);
                _sequence.OnComplete(() =>
                {
                    base.SetActiveUI(false);
                    m_Target.localScale = Vector3.one;
                });
                m_CloseTween = _sequence;
            }
        }
        public override void SetActiveUI(bool isActive)
        {
            if (isActive)
            {
                base.SetActiveUI(true);
                OpenTween();
            }
            else
                CloseTween();
        }

        protected override void OpenTween() => m_OpenTween.Restart();
        protected override void CloseTween() => m_CloseTween.Restart();

        public void Show(EMessageBoxBtnFlag btnFlag, MessageBoxParameter param, Action ok, Action cancel)
        {
            SetActivateButton(btnFlag);
            SetString(param);
            
            m_OkBtn.onClick.AddListener(OnClick(ok));
            m_CancelBtn.onClick.AddListener(OnClick(cancel));
            m_ExitBtn.onClick.AddListener(OnClick(cancel));
            m_BGBtn.onClick.AddListener(OnClick(cancel));
            
            SetActiveUI(true);
        }

        protected override void OnDisable()
        {
            m_TitleText.text = string.Empty;
            m_DescText.text = string.Empty;
            m_OkText.text = string.Empty;
            m_CancelText.text = string.Empty;
            
            m_ExitBtn.onClick.RemoveAllListeners();
            m_OkBtn.onClick.RemoveAllListeners();
            m_CancelBtn.onClick.RemoveAllListeners();
        }
    }
}
