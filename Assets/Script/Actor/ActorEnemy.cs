using System;
using DG.Tweening;
using Script.Actor.Base;
using Script.Actor.Parameter;
using Script.Custom.CustomDebug;
using Script.Parameter.Enum;
using Script.TableParser;
using UnityEngine;
using UnityEngine.Pool;

namespace Script.Actor
{
    public class ActorEnemy : ActorBase
    {
        private ActorStatus m_Status = null;
        private Transform m_TargetTr;

        public int WayIdx => m_CurrentWayIdx;
        public int NextWayIdx => m_CurrentWayIdx + 1;
        
        private int m_CurrentWayIdx = -1;
        
        protected override void OnGet()
        {
        }

        protected override void OnClear()
        {
            if (m_Status != null)
            {
                GenericPool<ActorStatus>.Release(m_Status);
                m_Status = null;
            }
        }

        protected override void OnStateEnter()
        {
            switch (m_ActorState)
            {
                case EActorState.Empty:
                    break;
                case EActorState.LoadResources:
                    break;
                case EActorState.LoadComplete:
                    break;
                case EActorState.Spawn:
                    break;
                case EActorState.Move:
                    break;
                case EActorState.MoveEnd:
                    break;
                case EActorState.Die:
                    gameObject.SetActive(false);
                    break;
            }
        }

        protected override void OnStateUpdate(float deltaTime)
        {
            switch (m_ActorState)
            {
                case EActorState.Empty:
                case EActorState.LoadResources:
                default:
                return;
                case EActorState.LoadComplete:
                    D.L($"{m_TableData.ID.ToString()} Load Complete");
                    SetState(EActorState.Spawn);
                    break;
                
                case EActorState.Spawn:
                    SetState(EActorState.Move);
                    break;
                case EActorState.Move:
                    var _dir = m_TargetTr.position - transform.position;
                    _dir.Normalize();
                    _dir *= m_Status.Speed * deltaTime;
                    transform.Translate(_dir, Space.World);
                    if (Vector3.Distance(transform.position, m_TargetTr.position) <= 0.1f)
                        SetState(EActorState.MoveEnd);
                    
                    break;
                case EActorState.MoveEnd:
                    break;
                case EActorState.Die:
                    break;
            }
        }

        protected override void OnStateExit()
        {
        }

        public override bool IsDie() => m_Status.CurrentHP <= 0 || IsState(EActorState.Die);

        protected override void InitActor()
        {
            m_Status = GenericPool<ActorStatus>.Get();
            m_Status.SetData(m_TableData);
            m_CurrentWayIdx = 0;
        }

        public void SetTarget(Transform tr)
        {
            m_TargetTr = tr;
            var _look = transform.position - m_TargetTr.position;
            if (_look == Vector3.zero)
                return;
            
            transform.rotation = Quaternion.LookRotation(_look);
        }

        public void AddWayIdx(int addValue = 1) => m_CurrentWayIdx += addValue;

    }
}
