using UnityEngine;

public class KAPKeyboardInput : KAPInput {
    
    /// Keycode of the key used to move to the next element
    public KeyCode nextElementKey;
    /// Keycode of the key used to move to the previous element
    public KeyCode previousElementKey;
    /// Keycode of the key used to trigger the escape gesture
    public KeyCode escapeKey;

    void Start ()
    {
        nextElementKey = KeyCode.RightArrow;
        previousElementKey = KeyCode.LeftArrow;
        escapeKey = KeyCode.Escape;
    }

	void Update ()
    {
        if (inputReceiver != null)
        {
            if (Input.GetKeyDown(nextElementKey))
            {
                inputReceiver.FocusNextElement();
            }
            else if (Input.GetKeyDown(previousElementKey))
            {
                inputReceiver.FocusPreviousElement();
            }
            else if (Input.GetKeyDown(escapeKey))
            {
                inputReceiver.HandleEscapeGesture();
            }
            // TODO: Implement via mouse down.
        } else {
            // TODO: LOG ERROR
        }
	}
}
