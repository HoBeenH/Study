using System;
using Script.Custom.Extensions;
using Script.Obj;
using UnityEngine;
using UnityEngine.Pool;

namespace Script.Map.Parameters
{
    [System.Serializable]
    public class BattleMapParameter
    {
        [SerializeField] private SpawnParameter[] m_SpawnArr = Array.Empty<SpawnParameter>();
        [SerializeField] private BuildSlot[] m_BuildArr = Array.Empty<BuildSlot>();

        public void Init(Transform root)
        {
            var _build = root.GetComponentsInChildren<BuildingModelBase>();
            if (!_build.IsNullOrEmptyCollection())
            {
                var _len = _build.Length;
                Array.Resize(ref m_BuildArr, _len);
                 
                for (var i = 0; i < _len; i++)
                {
                    m_BuildArr[i] = GenericPool<BuildSlot>.Get();
                    m_BuildArr[i].Init(i, _build[i], _build[i].transform);
                }
            }
        }

        public void Clear()
        {
            foreach (var buildParam in m_BuildArr)
            {
                buildParam.Clear();
                GenericPool<BuildSlot>.Release(buildParam);
            }

            m_BuildArr = Array.Empty<BuildSlot>();
        }
    }
}
