using System;
using System.Collections.Generic;
using UnityEngine;

namespace Script.Table
{
    [Serializable]
    public abstract class TableNode<T> : ScriptableObject where T : class, new()
    {
        public List<T> TableDataList = new List<T>();

        public void SetTableData(List<object> dataList)
        {
            for (int i = 0; i < dataList.Count; i++)
                TableDataList.Add(dataList[i] as T);
        }
        
        public abstract void OnLoadComplete();                         

        public abstract void ClearTable();
    }
}