using UnityEngine;

public class KAPKeyboardInput : KAPInput {
    
    /// Keycode of the key used to move to the next element
    public KeyCode nextElementKey;
    /// Keycode of the key used to move to the previous element
    public KeyCode previousElementKey;
    /// Keycode of the key used to trigger the escape gesture
    public KeyCode escapeKey;
    /// Keycode for the key used to trigger a selection
    public KeyCode selectKey;

    void Start ()
    {
        // TODO: Config
        nextElementKey = KeyCode.RightArrow;
        previousElementKey = KeyCode.LeftArrow;
        escapeKey = KeyCode.Escape;
        selectKey = KeyCode.Space;
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
            else if(Input.GetKeyDown(selectKey)) 
            {
                inputReceiver.SelectFocusedElement();
            }
            // TODO: Implement the rest?
        } else {
            Debug.LogError("The KAPKeyboardInput has no input receiver and thus, cannot forward any input.");
        }
	}
}
