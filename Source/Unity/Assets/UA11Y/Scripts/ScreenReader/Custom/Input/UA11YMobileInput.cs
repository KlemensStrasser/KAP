using UnityEngine;

enum UA11YTouchActionState {
    None,
    SwipeLeft,
    SwipeRight,
    Browsing,
    Tapping
}

public class UA11YMobileInput : UA11YInput
{
    // The maximum time to recognize the touch+move as a gesture (like a swipe)
    private static float MaximumSwipeGestureTime = 0.25f;
    private static float MaximumTapTime = 0.175f;
    private static float MaximumTapCooldownTime = 0.1f;
    private static Vector2 SwipeResistance = new Vector2(50f, 50f);

    private UA11YTouchActionState currentTouchActionState = UA11YTouchActionState.None;

    private Vector3 touchStartPosition;

    private float touchDownTime;
    private int tapCount = 0;
    private float timeSinceLastTap = 0.0f;

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
                        currentTouchActionState = UA11YTouchActionState.Browsing;
                    }

                    break;

                // Report that a direction has been chosen when the finger is lifted.
                case TouchPhase.Ended:

                    Vector3 touchLiftPosition = touch.position;
                    Vector2 swipeDelta = touchStartPosition - touchLiftPosition;

                    float timeDelta = currentTime - touchDownTime;

                    if (timeDelta <= MaximumTapTime && Mathf.Abs(swipeDelta.x) <= SwipeResistance.x)
                    {
                        tapCount += 1;
                        timeSinceLastTap = currentTime;
                        currentTouchActionState = UA11YTouchActionState.Tapping;
                    }
                    else if (currentTime - touchDownTime <= MaximumSwipeGestureTime)
                    {
                        if (swipeDelta.x > SwipeResistance.x)
                        {
                            currentTouchActionState = UA11YTouchActionState.SwipeLeft;
                        }
                        else if (swipeDelta.x < -SwipeResistance.x)
                        {
                            currentTouchActionState = UA11YTouchActionState.SwipeRight;
                        }
                    }
                    else if (currentTouchActionState == UA11YTouchActionState.Browsing)
                    {
                        currentTouchActionState = UA11YTouchActionState.None;
                    }

                    break;
            }

            if (inputReceiver != null)
            {
                switch (currentTouchActionState)
                {
                    case UA11YTouchActionState.None:
                        break;
                    case UA11YTouchActionState.SwipeLeft:
                        inputReceiver.FocusPreviousElement();
                        currentTouchActionState = UA11YTouchActionState.None;
                        break;
                    case UA11YTouchActionState.SwipeRight:
                        inputReceiver.FocusNextElement();
                        currentTouchActionState = UA11YTouchActionState.None;
                        break;
                    case UA11YTouchActionState.Browsing:
                        Vector2 point = touch.position;
                        point.y = Screen.height - point.y;
                        inputReceiver.FocusElementAtPosition(point);
                        break;
                    case UA11YTouchActionState.Tapping:
                        // This needs to be handled outside
                        // The tapping action will be triggered when the last touch is already over. 
                        // And when the touch is over, Input.touchCount > 0 fails.
                        break;
                }
            }
        }

        if (!touchActive && currentTouchActionState == UA11YTouchActionState.Tapping && currentTime - timeSinceLastTap >= MaximumTapCooldownTime)
        {
            if (tapCount == 2)
            {
                inputReceiver.SelectFocusedElement();
            }
            else
            {
                // Handle tripple tpuches and stuff
            }
            currentTouchActionState = UA11YTouchActionState.None;
        }

        if (currentTouchActionState != UA11YTouchActionState.Tapping)
        {
            tapCount = 0;
        }
    }

    override public string GetStatusText() 
    {
        return currentTouchActionState.ToString() +  " - " + tapCount.ToString();
    }
}
