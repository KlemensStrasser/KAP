using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KAPUIVisualizer : MonoBehaviour
{
    public void DrawIndicatorForElement(KAPScreenReaderElement element) 
    {
        if (element != null)
        {
            float borderWidth = 2;
            Rect frame = element.frame;
            KAPUIVisulizationDrawer.DrawRectBorder(frame, borderWidth, Color.black);

            Rect outerFrame = new Rect(
                frame.x - borderWidth,
                frame.y - borderWidth,
                frame.width + borderWidth * 2,
                frame.height + borderWidth * 2);

            KAPUIVisulizationDrawer.DrawRectBorder(outerFrame, borderWidth, Color.white);
        }
    }
}
