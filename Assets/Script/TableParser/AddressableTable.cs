using UnityEngine;
using System.Collections.Generic;
using System;
using Script.Custom.CustomEnum;
using Script.Custom.Extensions;
using Script.Parameter.Enum;
using Script.Table;

namespace Script.TableParser
{ 
    //StartDataRecord//
    [Serializable]
    public record AddressableTableData
    {
        [SerializeField] public EAddressableID ID;
		[SerializeField] public string Path;
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
                    Logger.E($"Already Contains Key {data.ID.ToString()}");
            }
        }
    
        public override void ClearTable()
        {
            m_DicData.Clear();
        }
    
        public AddressableTableData GetData(EAddressableID key)
        {
            if (!m_DicData.TryGetValue(key, out var _result))
                Logger.E($"Can't Find {key.ToString()}");

            return _result;
        }
        
        public List<AddressableTableData> GetDataList()
        {
            return TableDataList;
        }
    
        public AddressableTableData GetEditorData(string key)
        {
            return TableDataList.Find(obj => string.Equals(obj.ID.ToInt().ToString(), key));
        }
    }
}