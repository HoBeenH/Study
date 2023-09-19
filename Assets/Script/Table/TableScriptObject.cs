using System;
using System.Collections.Generic;
using UnityEngine;

namespace Script.Table
{
    [Serializable]
    public class TableScriptObject : ScriptableObject
    {
        public const string ADDRESSABLE_PATH = "Assets/Addressables/Table/TableDatas.asset";
        public List<ScriptableObject> nodes = new List<ScriptableObject>();

        public T GetTableObject<T>() where T : class
        {
            object checkObject = nodes.Find(obj => obj is T);

            return checkObject as T;
        }

        public void AllOnLoadCompleteCall()
        {
            foreach (var so in nodes)
                if (so is IBaseTableNode _node)
                    _node.OnLoadComplete();
        }

        public void CloseAllTable()
        {
            foreach (var so in nodes)
                if (so is IBaseTableNode target)
                    target.ClearTable();
        }
    }
}