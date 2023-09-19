using UnityEngine;

namespace Script.Custom.Extensions
{
    public static class ExtColor
    {
        public static Color HexToColor(this string hex) => HexToColor(hex, Color.white);

        public static Color HexToColor(this string hex, Color fallback) =>
            ColorUtility.TryParseHtmlString(hex, out var color) ? color : fallback;

        public static string ToARGB(this Color color) => ColorUtility.ToHtmlStringRGB(color);
    }
}