using UnityEngine;
using System.Collections.Generic;
using System;
using Script.Custom.Extensions;
using Script.Parameter.Enum;
using Script.Table;

namespace Script.TableParser
{
    //StartDataRecord//
    [Serializable]
    public record ActorTableData
    {
        [SerializeField] public int ID;
		[SerializeField] public int GroupID;
		[SerializeField] public EAddressableID AddressableID;
		[SerializeField] public int HP;
		[SerializeField] public float Speed;
		[SerializeField] public int Damage;
    }
    //EndDataRecord//

    [Serializable]
    public class ActorTable : TableNode<ActorTableData>, IBaseTableNode
    {
        private Dictionary<int, List<ActorTableData>> m_DicGroupID = new Dictionary<int, List<ActorTableData>>();
        public override void OnLoadComplete()
        {
            var _list = GetDataList();
            if (_list.IsNullOrEmptyCollection())
                return;
            
            foreach (var data in _list)
            {
                if (data == null)
                    continue;

                if (!m_DicGroupID.TryGetValue(data.GroupID, out var _dataList))
                {
                    _dataList = new List<ActorTableData>();
                    m_DicGroupID.Add(data.GroupID, _dataList);
                }
                if (_dataList.Find(x => x.ID == data.ID) != null)
                {
                    Logger.E($"Same Key {data.ID.ToString()}");
                    continue;
                }
                    
                _dataList.Add(data);
            }
        }
    
        public override void ClearTable()
        {
            
        }

        public bool TryGetListByGroupIdO(int groupID, out List<ActorTableData> result) =>
            m_DicGroupID.TryGetValue(groupID, out result);
        
        public ActorTableData GetData(int key)
        {
            ActorTableData _result = TableDataList.Find(obj => obj.ID == key);
            if (_result == null)
                Logger.E($"No Key. Table : {nameof(ActorTable)} Key : {key.ToString()}");
    
            return _result;
        }
    
        public List<ActorTableData> GetDataList()
        {
            return TableDataList;
        }
    
        public ActorTableData GetEditorData(string key)
        {
            return TableDataList.Find(obj => string.Equals(obj.ID.ToString(), key));
        }
    }
}