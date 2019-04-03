using UnityEngine;

// Based on the "Mobile Swipe Detection [Tutorial]" by N3K EN
// https://www.youtube.com/watch?v=poeXGuQ7eUo
public class KAPMobileInput : KAPInput
{
    private Vector3 touchStartPosition;
    private Vector2 swipeResistance = new Vector2(50f, 50f);

    void Update()
    {
        Touch touch = Input.GetTouch(0);

        bool touchEnded = false;

        switch (touch.phase)
        {
            // Record initial touch position.
            case TouchPhase.Began:
                touchStartPosition = touch.position;
                break;

            case TouchPhase.Moved:
                // TODO: Decide here if we have a gesture or not depending on the time between first touchDown and Lift
                break;

            // Report that a direction has been chosen when the finger is lifted.
            case TouchPhase.Ended:
                touchEnded = true;
                break;
        }

        if (touchEnded && inputReceiver != null)
        {
            Vector3 touchLiftPosition = Input.mousePosition;

            Vector2 swipeDelta = touchStartPosition - touchLiftPosition;

            if(swipeDelta.x > swipeResistance.x) 
            {
                inputReceiver.FocusPreviousElement();
            } 
            else if(swipeDelta.x < -swipeResistance.x) 
            {
                inputReceiver.FocusNextElement();
            }
        }
    }
}
