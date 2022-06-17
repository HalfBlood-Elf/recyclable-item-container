//MIT License
//Copyright (c) 2020 Mohammed Iqubal Hussain
//Website : Polyandcode.com 

using UnityEngine;

/// <summary>
/// Extension methods for Rect Transform
/// </summary>
public static class UICornersExtension
{
    public const int BOTTOM_LEFT = 0;
    public const int TOP_LEFT = 1;
    public const int TOP_RIGHT = 2;
    //private const int BOTTOM_RIGHT = 3;


    public static Vector3[] GetCorners(this RectTransform rectTransform)
    {
        Vector3[] corners = new Vector3[4];
        rectTransform.GetWorldCorners(corners);
        return corners;
    }

    /// <summary>
    /// Returns world y position of top-left corner.
    /// </summary>
    public static float MaxY(this RectTransform rectTransform)
    {
        return rectTransform.GetCorners()[TOP_LEFT].y;
    }

    /// <summary>
    /// Returns world y position of bottom-left corner.
    /// </summary>
    public static float MinY(this RectTransform rectTransform)
    {
        return rectTransform.GetCorners()[BOTTOM_LEFT].y;
    }

    public static float MaxX(this RectTransform rectTransform)
    {
        return rectTransform.GetCorners()[TOP_RIGHT].x;
    }

    public static float MinX(this RectTransform rectTransform)
    {
        return rectTransform.GetCorners()[BOTTOM_LEFT].x;
    }

}