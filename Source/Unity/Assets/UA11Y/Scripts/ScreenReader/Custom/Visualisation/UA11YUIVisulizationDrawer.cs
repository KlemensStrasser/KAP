using UnityEngine;

// Based on RTS Style Unit Selection in Unity 5 by Jeff Zimmer
// Link https://hyunkell.com/blog/rts-style-unit-selection-in-unity-5/
public static class UA11YUIVisulizationDrawer
{
    static Texture2D _borderTexture;
    public static Texture2D borderTexture
    {
        get
        {
            if (_borderTexture == null)
            {
                _borderTexture = new Texture2D(1, 1);
                _borderTexture.SetPixel(0, 0, Color.white);
                _borderTexture.Apply();
            }

            return _borderTexture;
        }
    }

    public static void DrawRect(Rect rect, Color color)
    {
        GUI.color = color;
        GUI.DrawTexture(rect, borderTexture);
        GUI.color = Color.white;
    }

    public static void DrawRectBorder(Rect rect, float borderWidth, Color color)
    {
        // Top
        DrawRect(new Rect(rect.xMin - borderWidth / 2, rect.yMin - borderWidth, rect.width + borderWidth / 2, borderWidth), color);
        // Left
        DrawRect(new Rect(rect.xMin - borderWidth / 2, rect.yMin - borderWidth, borderWidth, rect.height + borderWidth), color);
        // Right
        DrawRect(new Rect(rect.xMax, rect.yMin - borderWidth, borderWidth, rect.height + borderWidth), color);
        // Bottom
        DrawRect(new Rect(rect.xMin - borderWidth / 2, rect.yMax - borderWidth / 2, rect.width + borderWidth * 1.5f, borderWidth), color);
    }
}