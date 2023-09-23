using Cinemachine;
using UnityEngine;

namespace Script.Manager.CameraMgr
{
    [System.Serializable]
    public class CameraObject
    {
        [SerializeField] private Camera m_Camera = null;
        [SerializeField] private CinemachineBrain m_Brain = null;
        [SerializeField] private CinemachineVirtualCamera m_DefaultVirtualCam = null;

        public CinemachineVirtualCamera DefaultVirtualCamera => m_DefaultVirtualCam;
    }
}
