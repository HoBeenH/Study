using System;
using System.Collections.Generic;
using UnityEngine;

namespace Script.Table
{
    [Serializable]
    public class TableScriptObject : ScriptableObject
    {
        public List<ScriptableObject> nodes = new List<ScriptableObject>();

        public T GetTableObject<T>() where T : class
        {
            object checkObject = nodes.Find(obj => obj is T);

            return checkObject as T;
        }

        public void AllOnLoadCompleteCall()
        {
            List<IBaseTableNode> sortingList = new List<IBaseTableNode>();
            for (int i = 0; i < nodes.Count; i++)
                sortingList.Add(nodes[i] as IBaseTableNode);

            for (int i = 0; i < sortingList.Count; i++)
                sortingList[i].OnLoadComplete();
        }

        public void CloseAllTable()
        {
            for (int i = 0; i < nodes.Count; i++)
            {
                if (nodes[i] is IBaseTableNode target)
                    target.ClearTable();
            }
        }
    }
}