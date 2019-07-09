using UnityEngine;

enum TouchAction {None, SwipeLeft, SwipeRight, Browsing, Tapping }

public class KAPMobileInput : KAPInput
{
    // The maximum time to recognize the touch+move as a gesture (like a swipe)
    static float MaximumSwipeGestureTime = 0.25f;
    static float MaximumTapTime = 0.175f;
    static float MaximumTapCooldownTime = 0.1f;

    private Vector3 touchStartPosition;
    private Vector2 swipeResistance = new Vector2(50f, 50f);

    private float touchDownTime;

    public int tapCount = 0;
    private float timeSinceLastTap = 0.0f;

    private TouchAction currentTouchAction = TouchAction.None;

    void Update()
    {
        float currentTime = Time.time;
        bool touchActive = false;

        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            touchActive = true;

            switch (touch.phase)
            {
                // Record initial touch position.
                case TouchPhase.Began:
                    touchStartPosition = touch.position;
                    touchDownTime = currentTime;
                    break;

                case TouchPhase.Moved:
                    if (currentTime - touchDownTime > MaximumSwipeGestureTime)
                    {
                        currentTouchAction = TouchAction.Browsing;
                    }

                    break;

                // Report that a direction has been chosen when the finger is lifted.
                case TouchPhase.Ended:

                    Vector3 touchLiftPosition = touch.position;
                    Vector2 swipeDelta = touchStartPosition - touchLiftPosition;

                    float timeDelta = currentTime - touchDownTime;

                    if (timeDelta <= MaximumTapTime && Mathf.Abs(swipeDelta.x) <= swipeResistance.x)
                    {
                        tapCount += 1;
                        timeSinceLastTap = currentTime;
                        currentTouchAction = TouchAction.Tapping;
                    }
                    else if (currentTime - touchDownTime <= MaximumSwipeGestureTime)
                    {
                        if (swipeDelta.x > swipeResistance.x)
                        {
                            currentTouchAction = TouchAction.SwipeLeft;
                        }
                        else if (swipeDelta.x < -swipeResistance.x)
                        {
                            currentTouchAction = TouchAction.SwipeRight;
                        }
                    }
                    else if (currentTouchAction == TouchAction.Browsing)
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
                    case TouchAction.Tapping:
                        // This needs to be handled outside
                        // The tapping action will be triggered when the last touch is already over. 
                        // And when the touch is over, Input.touchCount > 0 fails.
                        break;
                }
            }
        }

        if (!touchActive && currentTouchAction == TouchAction.Tapping && currentTime - timeSinceLastTap >= MaximumTapCooldownTime)
        {
            if (tapCount == 2)
            {
                inputReceiver.SelectFocusedElement();
            }
            else
            {
                // Handle tripple tpuches and stuff
            }
            currentTouchAction = TouchAction.None;
        }

        if (currentTouchAction != TouchAction.Tapping)
        {
            tapCount = 0;
        }
    }

    override public string GetStatusText() 
    {
        return currentTouchAction.ToString() +  " - " + tapCount.ToString();
    }
}
