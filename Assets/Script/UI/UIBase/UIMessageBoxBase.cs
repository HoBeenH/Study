using DG.Tweening;
using UnityEngine;

namespace Script.UI.UIBase
{
    public abstract class UIMessageBoxBase : UIBase
    {
        protected Tween m_OpenTween;
        protected Tween m_CloseTween;
        
        protected virtual void OpenTween()
        {
            CreateActivateTween();
            
            if (m_OpenTween.IsActive())
                m_OpenTween.Complete(true);
            
            m_OpenTween.Restart();
        }       
        
        protected virtual void CloseTween()
        {
            CreateActivateTween();
            
            if (m_CloseTween.IsActive())
                m_CloseTween.Complete(true);
            
            m_CloseTween.Restart();
        }

        protected virtual void CreateActivateTween()
        {
            if (m_OpenTween == null)
            {
                m_OpenTween = transform.DOScale(Vector3.one, 0.1f).Pause().SetAutoKill(false);
                m_OpenTween.OnStart(() => this.transform.localScale = Vector3.zero);
                m_OpenTween.OnComplete(() => this.transform.localScale = Vector3.one);
            }

            if (m_CloseTween == null)
            {
                m_CloseTween = transform.DOScale(Vector3.zero, 0.1f).Pause().SetAutoKill(false);
                m_CloseTween.OnStart(() => this.transform.localScale = Vector3.one);
                m_CloseTween.OnComplete(() => this.transform.localScale = Vector3.one);
            }
        }
    }
}