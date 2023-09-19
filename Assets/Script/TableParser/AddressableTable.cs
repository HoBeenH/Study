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
    public record AddressableTableData
    {
        [SerializeField] public EAddressableID ID;
		[SerializeField] public string Path;
		[SerializeField] public bool IsPool;
		[SerializeField] public EAssetType AssetType;
    }
    //EndDataRecord//

    [Serializable]
    public class AddressableTable : TableNode<AddressableTableData>
    {
        private Dictionary<EAddressableID, AddressableTableData> m_DicData =
            new Dictionary<EAddressableID, AddressableTableData>();
        
        public override void OnLoadComplete()
        {
            var _dataList = GetDataList();
            if (_dataList.IsNullOrEmptyCollection())
                return;
            
            foreach (var data in _dataList)
            {
                if (data == null)
                    continue;

                if (!m_DicData.TryAdd(data.ID, data))
                {
                    
                }
            }
        }
    
        public override void ClearTable()
        {
            
        }
    
        public AddressableTableData GetData(EAddressableID key)
        {
            AddressableTableData _result = TableDataList.Find(obj => obj.ID == key);
            if (_result == null)
                Debug.LogError($"No Key. Table : {nameof(AddressableTable)} Key : {key.ToString()}");
    
            return _result;
        }
    
        public List<AddressableTableData> GetDataList()
        {
            return TableDataList;
        }
    
        public AddressableTableData GetEditorData(string key)
        {
            return TableDataList.Find(obj => string.Equals(obj.ID.ToString(), key));
        }
    }
}