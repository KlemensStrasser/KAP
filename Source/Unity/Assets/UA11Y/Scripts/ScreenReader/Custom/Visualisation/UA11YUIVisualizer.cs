using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UA11YUIVisualizer : MonoBehaviour
{
    public void DrawIndicatorForElement(UA11YElement element) 
    {
        if (element != null)
        {
            float borderWidth = 2;
            Rect frame = element.frame;
            UA11YUIVisulizationDrawer.DrawRectBorder(frame, borderWidth, Color.black);

            Rect outerFrame = new Rect(
                frame.x - borderWidth,
                frame.y - borderWidth,
                frame.width + borderWidth * 2,
                frame.height + borderWidth * 2);

            UA11YUIVisulizationDrawer.DrawRectBorder(outerFrame, borderWidth, Color.white);
        }
    }
}
