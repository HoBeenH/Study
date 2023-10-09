using System.Collections;
using Script.Custom.CustomDebug;
using Script.Manager.CameraMgr;
using Script.Manager.GameMgr;
using Script.Manager.ResourceMgr;
using Script.Manager.TableMgr;
using Script.Manager.UIMgr;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Script.Map
{
    public class Map_StartScene : MapBase
    {
        public override IEnumerator OnInit()
        {
            D.L("Play Game");
            if (!GameManager.HasInstance())
            {
                GameManager.Instance.Init();
                ResourceManager.Instance.Init();
                var _handle = ResourceManager.Instance.AwaitAddressable();

                CameraManager.Instance.Init();
                UIManager.Instance.Init();

                while (!_handle.IsDone)
                    yield return null;

                TableManager.Instance.Init();
                _handle = TableManager.Instance.AwaitTableLoad();
                while (!_handle.IsDone)
                    yield return null;
            }

            GotoTestGameMode();
            yield break;
        }

        private void GotoTestGameMode()
        {
            SceneManager.LoadScene("Scenes/Tarbo Rush_River");
        }
    }
}
