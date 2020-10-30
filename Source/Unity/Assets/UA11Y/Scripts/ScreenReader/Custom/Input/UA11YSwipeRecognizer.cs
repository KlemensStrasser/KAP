using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Based on the "Mobile Swipe Detection [Tutorial]" by N3K EN
// https://www.youtube.com/watch?v=poeXGuQ7eUo
public class UA11YSwipeInput : MonoBehaviour
{
    private Vector3 touchPosition;
    private Vector2 swipeResistance = new Vector2(50f, 50f);

    void Update()
    {
        if(Input.GetMouseButton(0))
        {
            touchPosition = Input.mousePosition;
        } else if(Input.GetMouseButtonUp(0))
        {
            Vector2 swipeDelta = touchPosition - Input.mousePosition;

            if(Mathf.Abs(swipeDelta.x) > swipeResistance.x)
            { 
                
            } else if (Mathf.Abs(swipeDelta.x) > swipeResistance.x) 
            {
            
            }
        }
    }
}
