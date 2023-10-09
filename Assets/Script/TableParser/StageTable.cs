using UnityEngine;
using System.Collections.Generic;
using System;
using Script.Table;

namespace Script.TableParser
{
    //StartDataRecord//
    [Serializable]
    public record StageTableData
    {
        [SerializeField] public int ID;
		[SerializeField] public int Stage;
		[SerializeField] public int WaveTotalCount;
		[SerializeField] public float WaveMaxTime;
    }
    //EndDataRecord//

    [Serializable]
    public class StageTable : TableNode<StageTableData>, IBaseTableNode
    {
        public override void OnLoadComplete()
        {
             
        }
    
        public override void ClearTable()
        {
            
        }
    
        public StageTableData GetData(int key)
        {
            StageTableData _result = TableDataList.Find(obj => obj.ID == key);
            if (_result == null)
                Logger.E($"No Key. Table : {nameof(StageTable)} Key : {key.ToString()}");
    
            return _result;
        }
    
        public List<StageTableData> GetDataList()
        {
            return TableDataList;
        }
    
        public StageTableData GetEditorData(string key)
        {
            return TableDataList.Find(obj => string.Equals(obj.ID.ToString(), key));
        }
    }
}