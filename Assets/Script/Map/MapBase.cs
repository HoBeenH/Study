using Script.Manager.CameraMgr;
using Script.Manager.GameMgr;
using UnityEngine;

namespace Script.Map
{
    public abstract class MapBase : MonoBehaviour
    {
        [Header("# Camera Parameter")]
        [SerializeField] private Transform m_Follow = null;
        [SerializeField] private Transform m_LookAt = null;
        
        private void Awake()
        {
            OnInit();
            SetCameraProperty();
            LinkCurrentMap();
        }

        protected abstract void OnInit();

        protected virtual void SetCameraProperty()
        {
            CameraManager.Instance.SetFollow(m_Follow);
            CameraManager.Instance.SetLookAt(m_LookAt);
        }

        private void LinkCurrentMap()
        {
            GameManager.Instance.SetCurrentMap(this);
        }
    }
}
