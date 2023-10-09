using System;
using DG.Tweening;
using UnityEngine;

namespace Script.Obj.CustomSequence
{
    [System.Serializable]
    public class BuildingSequence
    {
        [Header("# Build Sequence")] 
        [SerializeField] private GameObject m_ConstructModel = null;
        [SerializeField] private GameObject m_CompleteModel = null;
        
        private Tween m_ConstructTween;

        private Action m_ConstructCallback = null;

        private void CreateTween()
        {
            if (m_ConstructTween == null)
            {
                var _sequence = DOTween.Sequence().SetAutoKill(false).Pause();
                _sequence.OnPlay(() =>
                {
                    m_ConstructModel.gameObject.SetActive(true);
                    m_CompleteModel.gameObject.SetActive(false);
                });
                
                // _sequence.Append(m_ConstructModel.transform.DOScale())

                _sequence.OnComplete(() =>
                {
                    m_ConstructModel.gameObject.SetActive(false);
                    m_CompleteModel.gameObject.SetActive(true);
                    m_ConstructCallback?.Invoke();
                });

                m_ConstructTween = _sequence;
            }
        }

        public void Clear()
        {
            m_ConstructTween?.Kill();
        }
        
        public void Init()
        {
            
        }
    }
}
