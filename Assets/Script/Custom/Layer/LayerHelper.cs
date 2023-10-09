using Script.Parameter.Enum;
using UnityEngine;

namespace Script.Custom.Layer
{
    public static class LayerHelper
    {
        public static string UIMain = nameof(UIMain);
        public static string UIMessageBox = nameof(UIMessageBox);
        
        public static int UI_Main
        {
            get
            {
                s_UILayer ??= LayerMask.NameToLayer(UIMain);
                return s_UILayer.Value;
            }
        }

        public static int UI_MessageBox
        {
            get
            {
                s_UIMessageBox ??= LayerMask.NameToLayer(UIMessageBox);
                return s_UIMessageBox.Value;
            }
        }

        public static int GetLayer(ECanvasType type) => type switch
        {
            ECanvasType.UIMain           => UI_Main,
            ECanvasType.UIMessageBox     => UI_MessageBox,
            _ => -1
        };
        
        private static int? s_UILayer;
        private static int? s_UIMessageBox;
    }
}
