using System;
using System.Collections;
using Script.Custom.CustomDebug;
using Script.Manager.TableMgr;
using Script.Map.Controller;
using Script.Map.Parameters;
using Script.Parameter.Enum;
using Script.Parameter.Struct;
using Script.TableParser;
using UnityEngine;

namespace Script.Map
{
    public class Map_Test : MapBase
    {
        [Header("# Slot")] 
        [SerializeField] private BattleMapParameter m_BattleMapParameter = null;

        [SerializeField] private ActorCtrl m_ActorCtrl = null;

        public EMapState MapState { get; private set; } = EMapState.Load;

        private WaveParameter m_CurrentWaveParameter = null;
        private int[] m_CurrentWaveArr => m_CurrentWaveParameter.GetCurrentWaveEnemy(CurWave);
        private int CurWave = -1;
        private int CurWaveSpawnIdx = -1;
        
        private int m_CurrentWaveTotalSpawnCnt = -1;
        private int m_CurrentWaveSpawnCnt = -1;
        private bool m_CanSpawn => m_CurrentWaveSpawnCnt < m_CurrentWaveTotalSpawnCnt;
        
        private const float TEST_SPAWN_TIMER = 3f;
        private float m_SpawnTimer = 0f;

        private void InitWaveData()
        {
            if (m_CurrentWaveArr == null)
                return;
            
            m_CurrentWaveTotalSpawnCnt = m_CurrentWaveArr.Length;
            m_CurrentWaveSpawnCnt = 0;
            CurWaveSpawnIdx = 0;
            m_ActorCtrl.SetTotalSpawnCnt(m_CurrentWaveTotalSpawnCnt);
        }
        
        public override IEnumerator OnInit()
        {
            // TODO : ERROR
            // Test
            if (!TableManager.Instance.GetTable<WaveTable>().TryGetData(1, out m_CurrentWaveParameter))
                throw new Exception("Can't Get Data 1");

            CurWave = 1;
            // Map
            m_BattleMapParameter.InitBuildPoint(transform);
            
            // Actor
            m_ActorCtrl = new ActorCtrl();
            m_ActorCtrl.SetWayPoint(m_BattleMapParameter.Way);

            InitWaveData();
            
            MapState = EMapState.Update;
            yield break;
        }

        private void Update()
        {
            switch (MapState)
            {
                case EMapState.Load:
                    break;
 
                case EMapState.Update:
                    var _deltaTime = Time.deltaTime;
                    if (m_CanSpawn)
                    {
                        m_SpawnTimer += _deltaTime;
                        if (TEST_SPAWN_TIMER <= m_SpawnTimer)
                        {
                            m_SpawnTimer = 0f;
                            m_ActorCtrl.SpawnActor(m_CurrentWaveArr[CurWaveSpawnIdx], x =>
                            {
                                x.transform.position = m_BattleMapParameter.SpawnPoint.position;
                                x.SetTarget(m_BattleMapParameter.SpawnPoint);
                                m_CurrentWaveSpawnCnt++;
                            });
                            CurWaveSpawnIdx += 1;
                        }
                    }
                    
                    m_ActorCtrl.OnUpdate(_deltaTime);

                    if (!m_CanSpawn && m_ActorCtrl.IsAllDie)
                        MapState = EMapState.WaveClear;


                    if (Input.GetKeyDown(KeyCode.A))
                    {
                        m_ActorCtrl.Test(EActorState.Die);
                    }
                    break;
                case EMapState.WaveClear:
                    CurWave += 1;
                    if (m_CurrentWaveArr == null)
                    {
                        MapState = EMapState.StageClear;
                        return;
                    }
                    InitWaveData();
                    MapState = EMapState.Update;
                    break;
                case EMapState.StageClear:
                    D.E("Clear");
                    break;
                default:
                    return;
            }
        }
    }
}
