using Script.Manager.CameraMgr;
using Script.Manager.GameMgr;
using Script.Manager.ResourceMgr;
using Script.Manager.TableMgr;
using Script.Manager.UIMgr;
using Script.Map.Parameters;
using UnityEngine;

namespace Script.Map
{
    public class Map_Test : MapBase
    {
        [Header("# Slot")] 
        [SerializeField] private BattleMapParameter m_BattleMapParameter = null;
        
        protected override void OnInit()
        {
            
            GameManager.Instance.Init();
            ResourceManager.Instance.Init();
            TableManager.Instance.Init();
            CameraManager.Instance.Init();
            UIManager.Instance.Init();
            m_BattleMapParameter.Init(transform);
        }
    }
}
