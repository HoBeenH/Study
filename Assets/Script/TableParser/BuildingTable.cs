using UnityEngine;
using System.Collections.Generic;
using System;
using Script.Custom.Extensions;
using Script.EnumField;
using Script.Table;

namespace Script.TableParser
{
    //StartDataRecord//
    [Serializable]
    public record BuildingTableData
    {
        [SerializeField] public int ID;
		[SerializeField] public int GroupID;
		[SerializeField] public EAddressableID AddressableID;
		[SerializeField] public int Lv;
    }
    //EndDataRecord//

    [Serializable]
    public class BuildingTable : TableNode<BuildingTableData>, IBaseTableNode
    {
        private Dictionary<int, List<BuildingTableData>> m_DicGroupId = new Dictionary<int, List<BuildingTableData>>();
        private Dictionary<int, List<BuildingTableData>> m_DicLv = new Dictionary<int, List<BuildingTableData>>();
        
        private void InitDicGroupID(BuildingTableData data)
        {
            if (!m_DicGroupId.TryGetValue(data.GroupID, out var _groupList))
            {
                _groupList = new List<BuildingTableData>();
                m_DicGroupId.Add(data.GroupID, _groupList);
            }
                
            if (_groupList.Find(x => x.ID == data.ID) != null)
            {
                Logger.E($"Already Contains {data.ID.ToString()}");
                return;
            }
                
            _groupList.Add(data);
        }     
        
        private void InitDicLv(BuildingTableData data)
        {
            if (!m_DicLv.TryGetValue(data.Lv, out var _lvList))
            {
                _lvList = new List<BuildingTableData>();
                m_DicLv.Add(data.Lv, _lvList);
            }
                
            if (_lvList.Find(x => x.ID == data.ID) != null)
            {
                Logger.E($"Already Contains {data.ID.ToString()}");
                return;
            }
                
            _lvList.Add(data);
        }
        
        public override void OnLoadComplete()
        {
            if (TableDataList.IsNullOrEmptyCollection())
            {
                Logger.E("Table Is Null");
                return;
            }

            foreach (var data in TableDataList)
            {
                if (data == null)
                    continue;

                InitDicGroupID(data);
                InitDicLv(data);
            }
        }
    
        public override void ClearTable()
        {
            foreach (var pair in m_DicGroupId)
                pair.Value.Clear();
           
            foreach (var pair in m_DicLv)
                pair.Value.Clear();
            
            m_DicGroupId.Clear();  
            m_DicLv.Clear();
        }
        
        public BuildingTableData GetData(int key)
        {
            BuildingTableData _result = TableDataList.Find(obj => obj.ID == key);
            if (_result == null)
                Logger.E($"No Key. Table : {nameof(BuildingTable)} Key : {key.ToString()}");
    
            return _result;
        }
    
        public bool TryGetDataListByLv(int lv) => m_DicLv.TryGetValue(lv, out var _list);
    
        public BuildingTableData GetEditorData(string key)
        {
            return TableDataList.Find(obj => string.Equals(obj.ID.ToString(), key));
        }
    }
}