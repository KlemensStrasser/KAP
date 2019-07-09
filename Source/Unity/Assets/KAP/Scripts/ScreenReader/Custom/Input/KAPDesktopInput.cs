using UnityEngine;

public class KAPDesktopInput : KAPInput {
    
    /// Keycode of the key used to move to the next element
    public KeyCode nextElementKey = KeyCode.RightArrow;
    /// Keycode of the key used to move to the previous element
    public KeyCode previousElementKey = KeyCode.LeftArrow;
    /// Keycode of the key used to trigger the escape gesture
    public KeyCode escapeKey = KeyCode.E;

    /// Keycode for the key used to trigger a selection
    public KeyCode selectKey = KeyCode.Space;
    /// Keycode for the key used to increment the value of the focused element
    public KeyCode incrementValueKey = KeyCode.UpArrow;
    /// Keycode for the key used to decrement the value of the focused element
    public KeyCode decrementValueKey = KeyCode.DownArrow;

    public KeyCode browseElementsKey = KeyCode.Caret;

    void Update ()
    {
        if (inputReceiver != null)
        {
            if (Input.GetKey(browseElementsKey))
            {
                Vector2 invertedPosition = new Vector2(Input.mousePosition.x, Screen.height - Input.mousePosition.y);
                inputReceiver.FocusElementAtPosition(invertedPosition);
            }
            else if (Input.GetKeyDown(nextElementKey))
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
