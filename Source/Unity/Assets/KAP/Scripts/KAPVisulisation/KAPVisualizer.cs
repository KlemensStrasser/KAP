using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KAPVisualizer : MonoBehaviour
{
    public void DrawIndicatorForElement(KAPElement element) 
    {
        if (element != null)
        {
            float borderWidth = 2;
            Rect frame = element.frame;
            KAPVisulizationDrawer.DrawRectBorder(frame, borderWidth, Color.black);

            Rect outerFrame = new Rect(
                frame.x - borderWidth,
                frame.y - borderWidth,
                frame.width + borderWidth * 2,
                frame.height + borderWidth * 2);

            KAPVisulizationDrawer.DrawRectBorder(outerFrame, borderWidth, Color.white);
        }
    }
}
