using UnityEngine;

enum TouchAction {None, SwipeLeft, SwipeRight, Browsing }

public class KAPMobileInput : KAPInput
{
    // The maximum time to recognize the touch+move as a gesture (like a swipe)
    static float MaximumGestureTime = 0.25f; 

    private Vector3 touchStartPosition;
    private Vector2 swipeResistance = new Vector2(50f, 50f);

    private float touchDownTime;

    private TouchAction currentTouchAction = TouchAction.None;

    void Update()
    {
        Touch touch = Input.GetTouch(0);

        float currentTime = Time.time;

        switch (touch.phase)
        {
            // Record initial touch position.
            case TouchPhase.Began:
                touchStartPosition = touch.position;
                touchDownTime = currentTime;
                break;

            case TouchPhase.Moved:
                if(currentTime - touchDownTime > MaximumGestureTime) {
                    currentTouchAction = TouchAction.Browsing;
                }

                break;

            // Report that a direction has been chosen when the finger is lifted.
            case TouchPhase.Ended:
                if (currentTime - touchDownTime <= MaximumGestureTime)
                {
                    Vector3 touchLiftPosition = touch.position;
                    Vector2 swipeDelta = touchStartPosition - touchLiftPosition;

                    if (swipeDelta.x > swipeResistance.x)
                    {
                        currentTouchAction = TouchAction.SwipeLeft;
                    }
                    else if (swipeDelta.x < -swipeResistance.x)
                    {
                        currentTouchAction = TouchAction.SwipeRight;
                    }

                } 
                else if(currentTouchAction == TouchAction.Browsing)
                {
                    currentTouchAction = TouchAction.None;
                }

                break;
        }

        if (inputReceiver != null)
        {
            switch (currentTouchAction)
            {
                case TouchAction.None:
                    break;
                case TouchAction.SwipeLeft:
                    inputReceiver.FocusPreviousElement();
                    currentTouchAction = TouchAction.None;
                    break;
                case TouchAction.SwipeRight:
                    inputReceiver.FocusNextElement();
                    currentTouchAction = TouchAction.None;
                    break;
                case TouchAction.Browsing:
                    Vector2 point = touch.position;
                    point.y = Screen.height - point.y;
                    inputReceiver.FocusElementAtPosition(point);
                    break;

            }
        }
    }
}
