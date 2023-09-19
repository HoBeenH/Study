using UnityEngine;
using Script.Table;
using Script.Base.MonoSingleTone;

namespace Script.Manager
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
        
        protected override void OnInit()
        {
            ResourceManager.Instance.LoadObj<TableScriptObject>(TableScriptObject.ADDRESSABLE_PATH, _tableObj =>
            {
                m_TableObj = _tableObj;
                m_TableObj.AllOnLoadCompleteCall();
            });
        }

        protected override void OnClose()
        {
            if (m_TableObj != null)
            {
                m_TableObj.CloseAllTable();
                ResourceManager.Instance.ReleaseObj(m_TableObj);
            }
        }
    }
}