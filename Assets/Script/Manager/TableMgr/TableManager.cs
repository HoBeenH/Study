using Script.Base.MonoSingleTone;
using Script.Manager.ResourceMgr;
using Script.Table;
using Script.TableParser;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

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

        public AsyncOperationHandle AwaitTableLoad()
        {
            var _handle = Addressables.LoadAssetAsync<TableScriptObject>(TableScriptObject.ADDRESSABLE_PATH);
            _handle.Completed += (data) =>
            {
                m_TableObj = data.Result;
                m_TableObj.AllOnLoadCompleteCall();
            };
            return _handle;
        }
        
        protected override void OnInit()
        {
        }

        protected override void OnClose()
        {
        }
    }
}