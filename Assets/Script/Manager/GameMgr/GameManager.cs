using Script.Base.MonoSingleTone;
using Script.Map;
using UnityEngine;

namespace Script.Manager.GameMgr
{
    public class GameManager : MonoSingleTone<GameManager>
    {
        private MapBase m_CurrentMap = null;
        private Coroutine m_MapInitCoroutine = null;
        
        
        protected override void OnInit()
        {

        }

        protected override void OnClose()
        {
        }

        public void SetCurrentMap(MapBase map)
        {
            if (map == null)
                return;
            
            m_CurrentMap = map;
        }

        public T GetCurrentMap<T>() where T : MapBase => m_CurrentMap as T;
    }
}
