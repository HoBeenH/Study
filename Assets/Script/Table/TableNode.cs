using System;
using System.Collections.Generic;
using Script.Base.Logger;
using Script.Custom.CustomDebug;
using UnityEngine;

namespace Script.Table
{
    [Serializable]
    public abstract class TableNode<T> : ScriptableObject, IBaseTableNode, ILogger<T> where T : class, new()
    {
        public List<T> TableDataList = new List<T>();
        public ILogger<T> Logger => this;

        public void SetTableData(List<object> dataList)
        {
            for (int i = 0; i < dataList.Count; i++)
                TableDataList.Add(dataList[i] as T);
        }

        public abstract void OnLoadComplete();                         

        public abstract void ClearTable();
    }
}