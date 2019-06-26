using UnityEngine;

public class KAPDesktopInput : KAPInput {
    
    /// Keycode of the key used to move to the next element
    public KeyCode nextElementKey;
    /// Keycode of the key used to move to the previous element
    public KeyCode previousElementKey;
    /// Keycode of the key used to trigger the escape gesture
    public KeyCode escapeKey;

    /// Keycode for the key used to trigger a selection
    public KeyCode selectKey;
    /// Keycode for the key used to increment the value of the focused element
    public KeyCode incrementValueKey;
    /// Keycode for the key used to decrement the value of the focused element
    public KeyCode decrementValueKey;

    void Start ()
    {
        // TODO: Config
        nextElementKey = KeyCode.RightArrow;
        previousElementKey = KeyCode.LeftArrow;
        escapeKey = KeyCode.Escape;

        selectKey = KeyCode.Space;
        incrementValueKey = KeyCode.UpArrow;
        decrementValueKey = KeyCode.DownArrow;
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
            else if (Input.GetKeyDown(incrementValueKey))
            {
                inputReceiver.IncrementValueOfFocuedElement();
            }
            else if (Input.GetKeyDown(decrementValueKey))
            {
                inputReceiver.DecrementValueOfFocuedElement();
            }
        } else {
            Debug.LogWarning("The KAPKeyboardInput has no input receiver and thus, cannot forward any input.");
        }
	}

    override public string GetStatusText()
    {
        return "Nothing";
    }
}
