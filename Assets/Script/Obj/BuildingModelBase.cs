using DG.Tweening;
using Script.Custom.CustomDebug;
using Script.TableParser;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Script.Obj
{
    public abstract class BuildingModelBase : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
    {
        public BuildingTableData TableData { get; protected set; } = null;
        private Sequence m_ClickSequence = null;

        public virtual void Clear()
        {
            m_ClickSequence?.Kill();
            m_ClickSequence = null;
        }

        protected virtual void ClickTween()
        {
            if (GetTweenTarget() == null)
                return;
            
            if (m_ClickSequence == null)
            {
                m_ClickSequence = DOTween.Sequence().SetAutoKill(false);
                m_ClickSequence.Prepend(GetTweenTarget().DOScale(0.8f, 0.05f));
                m_ClickSequence.Append(GetTweenTarget().DOScale(Vector3.one, 0.1f));
                m_ClickSequence.OnComplete(() => GetTweenTarget().localScale = Vector3.one);
            }
            if (m_ClickSequence.IsActive())
                m_ClickSequence.Complete(true);
            
            m_ClickSequence.Restart();
        }

        protected abstract Transform GetTweenTarget();
        
        protected abstract void OnClick();
        
        // 누르고 땔때
        public void OnPointerClick(PointerEventData eventData)
        {
            D.L("Click");
            OnClick();
            ClickTween();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            D.L("Enter");
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            D.L("Exit");
        }

        // 눌렀을때
        public void OnPointerDown(PointerEventData eventData)
        {
            D.L("Down");
        }

        // 동일위치 클릭후 땠을때
        public void OnPointerUp(PointerEventData eventData)
        {
            D.L("Up");
        }
    }
}