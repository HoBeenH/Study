using Script.Base.MonoSingleTone;
using UnityEngine;

namespace Script.Manager.CameraMgr
{
    public class CameraManager : MonoSingleTone<CameraManager>
    {
        [SerializeField] private CameraObject m_CameraObject = null;
        protected override void OnInit()
        {
            if (m_CameraObject == null)
            {
                m_CameraObject = new CameraObject();
                m_CameraObject.Root.transform.SetParent(transform);
            }
        }

        protected override void OnClose()
        {
        }

        public void SetFollow(Transform tr) => m_CameraObject.DefaultVirtualCamera.Follow = tr;
        
        public void SetLookAt(Transform tr) => m_CameraObject.DefaultVirtualCamera.LookAt = tr;
    }
}
