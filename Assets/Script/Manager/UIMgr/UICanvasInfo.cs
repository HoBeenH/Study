using Script.Base.Logger;
using Script.Custom.CustomEnum;
using Script.Custom.Extensions;
using Script.Custom.Layer;
using Script.EnumField;
using UnityEngine;
using UnityEngine.UI;

namespace Script.Manager.UIMgr
{
    [System.Serializable]
    public class UICanvasInfo : ILogger<UICanvasInfo>
    {
        public ILogger<UICanvasInfo> Logger => this;
        
        private ECanvasType m_Type;
        private GameObject m_RootGO;
        private Canvas m_Canvas = null;
        private GraphicRaycaster m_Raycaster = null;

        public RectTransform Root { get; private set; } = null;
        
        public UICanvasInfo(Transform parent, ECanvasType type)
        {
            m_Type = type;
            m_RootGO = new GameObject($"[{type.ToString()}] Canvas", typeof(Canvas), typeof(GraphicRaycaster));
            m_RootGO.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
            m_RootGO.transform.SetParent(parent);

            if (!m_RootGO.TryGetComponent(out m_Canvas))
            {
                Logger.E($"Can't Get Comp Canvas {m_Type.ToString()}");
                return;
            }

            m_Canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            m_Canvas.sortingOrder = type.ToInt();
            
            if (!m_RootGO.TryGetComponent(out m_Raycaster))
            {
                Logger.E($"Can't Get Comp GraphicRaycaster {m_Type.ToString()}");
                return;
            }

            m_Raycaster.ignoreReversedGraphics = true;
            m_Raycaster.blockingObjects = GraphicRaycaster.BlockingObjects.None;
            m_Raycaster.blockingMask = LayerHelper.GetLayer(type);

            Root = new GameObject($"{type.ToString()} Root", typeof(RectTransform)).transform as RectTransform;
            if (Root == null)
                return;
            
            Root.SetParent(m_RootGO.transform);
            Root.SetStretch(EStretchType.FullStretch, true, true);
        }

        public void Clear()
        {
            m_Canvas = null;
            m_Raycaster = null;
            
            if (m_RootGO != null)
                Object.Destroy(m_RootGO);
        }
    }
}
