using System;
using System.Collections.Generic;
using Script.Actor;
using Script.Actor.Base;
using Script.Custom.Extensions;
using Script.Manager.ResourceMgr;
using Script.Manager.TableMgr;
using Script.Parameter.Enum;
using Script.TableParser;
using UnityEngine;

namespace Script.Map.Controller
{
    [Serializable]
    public class ActorCtrl
    {
        private readonly List<ActorBase> m_ActorList = new List<ActorBase>();
        private Transform[] m_Way = null;
        private int m_CurrentTotalSpawnCnt = -1;
        
        public int SpawnCnt { get; private set; } = 0;
        public bool IsPause = false;
        public bool IsAllDie { get; private set; } = false;

        public void SetTotalSpawnCnt(int cnt) => m_CurrentTotalSpawnCnt = cnt;
        
        public void OnUpdate(float deltaTime)
        {
            if (IsPause)
                return;

            int _dieCnt = 0;
            foreach (var actorBase in m_ActorList)
            {
                if (actorBase is not ActorEnemy _enemy) 
                    continue;
                
                if (_enemy.IsDie())
                {
                    if (!_enemy.IsState(EActorState.Die))
                        _enemy.SetState(EActorState.Die);

                    _dieCnt++;
                    continue;
                }
                
                _enemy.OnUpdate(deltaTime);

                if (_enemy.IsState(EActorState.MoveEnd))
                {
                    if (m_Way.IsValidIndex(_enemy.NextWayIdx))
                    {
                        _enemy.SetTarget(m_Way[_enemy.NextWayIdx]);
                        _enemy.SetState(EActorState.Move);
                        _enemy.AddWayIdx();
                    }
                }
            }


            IsAllDie = _dieCnt >= m_CurrentTotalSpawnCnt;
        }

        public void SpawnActor(int[] enemyArr, Action<ActorEnemy> callback)
        {
            if (enemyArr == null)
                return;
            
            if (!m_ActorList.IsNullOrEmptyCollection())
                Clear();
            
            foreach (var i in enemyArr)
                SpawnActor(i, callback);
        }

        public void SpawnActor(int enemy, Action<ActorEnemy> callback)
        {
            SpawnCnt++; 
            var _enemyTable = TableManager.Instance.GetTable<ActorTable>().GetData(enemy);
            var _addressable = TableManager.Instance.GetTable<AddressableTable>().GetData(_enemyTable.AddressableID);
            ResourceManager.LoadPool<ActorEnemy>(_addressable.Path, false, x =>
            {
                x.SetState(EActorState.LoadComplete);
                m_ActorList.Add(x);
                --SpawnCnt;
                x.Init(_enemyTable);
                callback?.Invoke(x);
            });
        }

        public void SetWayPoint(Transform[] arr) => m_Way = arr;

        public void Test(EActorState state)
        {
            m_ActorList.ForEach(x => x.SetState(state));
        }
        
        public void Clear()
        {
            foreach (var actor in m_ActorList)
            {
                if (actor == null)
                    return;
                
                actor.ClearModel();
            }
            m_ActorList.Clear();
        }
    }
}
