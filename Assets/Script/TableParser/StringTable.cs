using UnityEngine;
using System.Collections.Generic;
using System;
using Script.Custom.Extensions;
using Script.Table;

namespace Script.TableParser
{
    //StartDataRecord//
    [Serializable]
    public record StringTableData
    {
        [SerializeField] public string ID;
		[SerializeField] public string KR;
    }
    //EndDataRecord//

    [Serializable]
    public class StringTable : TableNode<StringTableData>, IBaseTableNode
    {
        private readonly Dictionary<string, StringTableData> m_Dic = new Dictionary<string, StringTableData>();
        public override void OnLoadComplete()
        {
            var _list = GetDataList();
            if (_list.IsNullOrEmptyCollection())
            {
                Logger.E("Data Is Null");
                return;
            }
            
            foreach (var data in _list)
            {
                if (data == null)
                    continue;

                if (!m_Dic.TryAdd(data.ID, data))
                    Logger.E($"Already Contains {data.ID}");
            }
        }
    
        public override void ClearTable()
        {
            
        }

        public string GetTableString(string key, params object[] param)
        {
            if (m_Dic.TryGetValue(key, out var _data))
            {
                var _result = _data.KR;
                if (param != null)
                    _result = TryFormat(_result, param);

                return _result;
            }

            return $"!!- ERROR {key}";
        }

        public string GetTableString(string key) => GetTableString(key, null);

        private string TryFormat(string str, params object[] format)
        {
            if (string.IsNullOrEmpty(str))
            {
                Logger.E("Format Key Is Null Or Empty");
                return string.Empty;
            }

            if (format.IsNullOrEmptyCollection())
            {
                Logger.E("Format parameter is Null");
                return $"!!- ERROR {str}";
            }
            try
            {
                return string.Format(str, format);
            }
            catch (Exception e)
            {
                Logger.E(e);
                return $"!!- ERROR {str}";
            }
        }
        
        public StringTableData GetData(string key)
        {
            m_Dic.TryGetValue(key, out var _result);
            if (_result == null)
                Logger.E($"No Key. Table : {nameof(StringTable)} Key : {key}");
    
            return _result;
        }
    
        public List<StringTableData> GetDataList()
        {
            return TableDataList;
        }
    
        public StringTableData GetEditorData(string key)
        {
            return TableDataList.Find(obj => string.Equals(obj.ID.ToString(), key));
        }
    }
}