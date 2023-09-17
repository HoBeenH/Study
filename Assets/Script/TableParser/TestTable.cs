using UnityEngine;
using System.Collections.Generic;
using System;
using Script.Table;
namespace Script.TableParser
{
    
    //StartDataRecord//
    [Serializable]
    public record TestTableData
    {
        [SerializeField] public int ID;
		[SerializeField] public int intV;
		[SerializeField] public string strV;
    }
    //EndDataRecord//

    [Serializable]
    public class TestTable : TableNode<TestTableData>, IBaseTableNode
    {
        public override void OnLoadComplete()
        {
            
        }
    
        public override void ClearTable()
        {
            
        }
    
        public TestTableData GetData(int key)
        {
            TestTableData _result = TableDataList.Find(obj => obj.ID == key);
            if (_result == null)
                Debug.LogError($"No Key. Table : {nameof(TestTable)} Key : {key.ToString()}");
    
            return _result;
        }
    
        public List<TestTableData> GetDataList()
        {
            return TableDataList;
        }
    
        public TestTableData GetEditorData(string key)
        {
            return TableDataList.Find(obj => string.Equals(obj.ID.ToString(), key));
        }
    }
}