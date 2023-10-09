using UnityEngine;
using System.Collections.Generic;
using System;
using Script.Custom.Extensions;
using Script.Parameter.Struct;
using Script.Table;

namespace Script.TableParser
{
    //StartDataRecord//
    [Serializable]
    public record WaveTableData
    {
        [SerializeField] public int ID;
		[SerializeField] public int Stage;
		[SerializeField] public int Wave;
		[SerializeField] public int SpawnCount;
		[SerializeField] public int[] EnemyIDArr;
    }
    //EndDataRecord//

    [Serializable]
    public class WaveTable : TableNode<WaveTableData>, IBaseTableNode
    {
        private readonly Dictionary<int, WaveParameter> m_StageDic = new Dictionary<int, WaveParameter>();
        
        public override void OnLoadComplete()
        {
            var _list = GetDataList();
            if (_list.IsNullOrEmptyCollection())
                return;
            
            foreach (var data in _list)
            {
                if (data == null)
                {
                    Logger.E("Data Is Null");
                    continue;
                }

                if (!m_StageDic.TryGetValue(data.Stage, out var _param))
                {
                    _param = new WaveParameter();
                    m_StageDic.Add(data.Stage, _param);
                }
                
                _param.Add(data.Wave, data.EnemyIDArr);
            }
        }
    
        public override void ClearTable()
        {
            m_StageDic.Clear();
        }

        public bool TryGetData(int stage, out WaveParameter param) => m_StageDic.TryGetValue(stage, out param);
    
        public List<WaveTableData> GetDataList()
        {
            return TableDataList;
        }
    
        public WaveTableData GetEditorData(string key)
        {
            return TableDataList.Find(obj => string.Equals(obj.ID.ToString(), key));
        }
    }
}