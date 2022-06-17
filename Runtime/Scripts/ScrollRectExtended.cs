using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrollRectExtended : ScrollRect
{
    public void OnContentPositionChanged(Vector2 delta)
    {
        m_ContentStartPosition += delta;
    }
}
