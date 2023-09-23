using Script.Custom.CustomDebug;
using Script.Manager.CameraMgr;
using Script.Manager.GameMgr;
using Script.Manager.ResourceMgr;
using Script.Manager.TableMgr;
using Script.Manager.UIMgr;
using UnityEngine;

namespace Script.Map
{
    public class Map_StartScene : MapBase
    {
        protected override void OnInit()
        {
            D.L("Play Game");
            
            GameManager.Instance.Init();
            ResourceManager.Instance.Init();
            TableManager.Instance.Init();
            CameraManager.Instance.Init();
            UIManager.Instance.Init();
        }
    }
}
