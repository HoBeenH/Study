using System;
using Script.Parameter.Enum;
using UnityEngine;

namespace Script.Custom.Extensions
{
    public static class ExtRectTransform
    {
        public static void SetStretch(this RectTransform tr, EStretchType Type, bool SetPivot = false,
            bool SetPos = false)
        {
            if (tr == null)
                return;

            AnchorJob[(int)Type].Invoke(tr, SetPivot);
            if (SetPos)
            {
                tr.offsetMax = Vector2.zero;
                tr.offsetMin = Vector2.zero;
                tr.anchoredPosition = Vector2.zero;
                tr.anchoredPosition3D = Vector3.zero;
            }
        }

        private static readonly Action<RectTransform, bool>[] AnchorJob = new Action<RectTransform, bool>[]
        {
            (src, setPivot) =>
            {
                src.anchorMin = src.anchorMax = Vector2.up;
                if (setPivot) src.pivot = Vector2.up;
            },
            (src, setPivot) =>
            {
                src.anchorMin = src.anchorMax = new Vector2(0.5f, 1);
                if (setPivot) src.pivot = new Vector2(0.5f, 1f);
            },
            (src, setPivot) =>
            {
                src.anchorMin = src.anchorMax = Vector2.one;
                if (setPivot) src.pivot = Vector2.one;
            },

            (src, setPivot) =>
            {
                src.anchorMin = src.anchorMax = new Vector2(0, 0.5f);
                if (setPivot) src.pivot = new Vector2(0f, 0.5f);
            },
            (src, setPivot) =>
            {
                src.anchorMin = src.anchorMax = new Vector2(0.5f, 0.5f);
                if (setPivot) src.pivot = new Vector2(0.5f, 0.5f);
            },
            (src, setPivot) =>
            {
                src.anchorMin = src.anchorMax = new Vector2(1, 0.5f);
                if (setPivot) src.pivot = new Vector2(1, 0.5f);
            },

            (src, setPivot) =>
            {
                src.anchorMin = src.anchorMax = Vector2.zero;
                if (setPivot) src.pivot = Vector2.zero;
            },
            (src, setPivot) =>
            {
                src.anchorMin = src.anchorMax = new Vector2(0.5f, 0);
                if (setPivot) src.pivot = new Vector2(0.5f, 0);
            },
            (src, setPivot) =>
            {
                src.anchorMin = src.anchorMax = Vector2.right;
                if (setPivot) src.pivot = Vector2.right;
            },

            (src, setPivot) =>
            {
                src.anchorMin = Vector2.zero;
                src.anchorMax = Vector2.up;
                if (setPivot) src.pivot = new Vector2(0f, 0.5f);
            },
            (src, setPivot) =>
            {
                src.anchorMin = new Vector2(0.5f, 0);
                src.anchorMax = new Vector2(0.5f, 1);
                if (setPivot) src.pivot = new Vector2(0.5f, 0.5f);
            },
            (src, setPivot) =>
            {
                src.anchorMin = Vector2.right;
                src.anchorMax = Vector2.one;
                if (setPivot) src.pivot = new Vector2(1f, 0.5f);
            },

            (src, setPivot) =>
            {
                src.anchorMin = Vector2.up;
                src.anchorMax = Vector2.one;
                if (setPivot) src.pivot = new Vector2(0.5f, 1f);
            },
            (src, setPivot) =>
            {
                src.anchorMin = new Vector2(0, 0.5f);
                src.anchorMax = new Vector2(1, 0.5f);
                if (setPivot) src.pivot = new Vector2(0.5f, 0.5f);
            },
            (src, setPivot) =>
            {
                src.anchorMin = Vector2.zero;
                src.anchorMax = Vector2.right;
                if (setPivot) src.pivot = new Vector2(0.5f, 0f);
            },

            (src, setPivot) =>
            {
                src.anchorMin = Vector2.zero;
                src.anchorMax = Vector2.one;
                if (setPivot) src.pivot = new Vector2(0.5f, 0.5f);
            },
        };
    }
}