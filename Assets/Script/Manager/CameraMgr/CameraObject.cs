using Cinemachine;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Script.Manager.CameraMgr
{
    [System.Serializable]
    public class CameraObject
    {
        [SerializeField] private Camera m_Camera = null;
        [SerializeField] private CinemachineBrain m_Brain = null;
        [SerializeField] private CinemachineVirtualCamera m_DefaultVirtualCam = null;

        public GameObject Root { get; private set; } = null;
        
        public CinemachineVirtualCamera DefaultVirtualCamera => m_DefaultVirtualCam;

        public CameraObject()
        {
            Root = new GameObject("Camera Obj Root", typeof(Camera), typeof(CinemachineBrain), typeof(UniversalAdditionalCameraData));
            m_Camera = Root.GetComponent<Camera>();
            m_Brain = Root.GetComponent<CinemachineBrain>();
            var _go = new GameObject("V Cam", typeof(CinemachineVirtualCamera));
            _go.transform.SetParent(Root.transform);
            m_DefaultVirtualCam = _go.GetComponent<CinemachineVirtualCamera>();
            m_DefaultVirtualCam.AddCinemachineComponent<CinemachineFramingTransposer>();
            m_DefaultVirtualCam.AddCinemachineComponent<CinemachineComposer>();
        }
    }
}
