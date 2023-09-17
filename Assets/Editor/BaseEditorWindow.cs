using System;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    public class BaseEditorWindow : EditorWindow
    {
        protected Action m_GuiDraw = null;

        protected void OnGUI()
        {
            m_GuiDraw?.Invoke();

            var evt = Event.current;
            switch (evt.type)
            {
                case EventType.KeyDown:
                    if (evt.keyCode == KeyCode.Escape)
                        Close();

                    break;
            }
        }

        protected static void MessageBox(string _message)
        {
            EditorUtility.DisplayDialog(
                "안내",
                _message,
                "확인" );
        }
    }
}