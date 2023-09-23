using Script.Base.MonoSingleTone;
using Script.Manager.ResourceMgr;
using Script.Table;
using Script.TableParser;
using UnityEngine;

namespace Script.Manager.TableMgr
{
    public class TableManager : MonoSingleTone<TableManager>
    {
        private TableScriptObject m_TableObj = null;

        public T GetTable<T>() where T : ScriptableObject
        {
            if (m_TableObj == null)
                return null;

            return m_TableObj.GetTableObject<T>();
        }

        public string GetTableString(string key, params object[] param) =>
            GetTable<StringTable>().GetTableString(key, param);
      

        public string GetTableString(string key) => GetTableString(key, null);

        protected override void OnInit()
        {
            ResourceManager.LoadObjAsync<TableScriptObject>(TableScriptObject.ADDRESSABLE_PATH, _tableObj =>
            {
                m_TableObj = _tableObj;
                m_TableObj.AllOnLoadCompleteCall();
            });
        }

        protected override void OnClose()
        {
        }
    }
}