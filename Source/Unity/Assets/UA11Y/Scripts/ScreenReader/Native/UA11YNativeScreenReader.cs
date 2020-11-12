using System;

/// <summary>
/// This needs to be a singleton to make the callback 
/// </summary>
public class UA11YNativeScreenReader: IUA11YScreenReader
{
    private UA11YElement[] accessibilityElements;

    private UA11YElement currentlyFocusedElement;

    public UA11YNativeScreenReader() 
    {
        currentlyFocusedElement = null;

        UA11YNativeScreenReaderBridge.Instance.selectionCallback = InvokeSelectionOfElementWithID;
        UA11YNativeScreenReaderBridge.Instance.valueChangeCallback = InvokeValueChangeOfElementWithID;
        UA11YNativeScreenReaderBridge.Instance.focusCallback = SetFocusOnElementWithID;
    }

    #region IUA11YScreenReader

    public void UpdateWithScreenReaderElements(UA11YElement[] accessibilityElements, bool tryRetainingIndex = false)
    {
        this.accessibilityElements = accessibilityElements;
        UA11YNativeScreenReaderBridge.Instance.UpdateWithScreenReaderElements(accessibilityElements);
    }


    /// <summary>
    /// Focuses the given element.
    /// </summary>
    /// <param name="elementToFocus">Element to focus.</param>
    /// 
    /// This needs to call the native plugin to change the focus of the native screen reader, which will trigger the InvokeFocuseCallback
    public void FocusElement(UA11YElement elementToFocus)
    {
        // TODO: IMPLEMENT FULLY
        int targetInstanceID = elementToFocus.gameObject.GetInstanceID();
    }

    public void SetEnabled(bool enabled)
    { 
    }

    public void AnnounceMessage(string message)
    {
        UA11YNativeScreenReaderBridge.Instance.AnnounceMessage(message);
    }

    #endregion


    #region Callbacks

    /// <summary>
    /// Invokes the selection of the element with given identifier.
    /// </summary>
    /// <param name="instanceID">Instance id of the elements gameObject.</param>
    public void InvokeSelectionOfElementWithID(int instanceID)
    {
        UA11YElement element = Array.Find(accessibilityElements, e => e.gameObject.GetInstanceID() == instanceID);

        if (element != null)
        {
            element.InvokeEventOfType(UA11YElementInteractionEventType.Click);
        }
    }

    /// <summary>
    /// Invokes the value change of the element with the given identifier.
    /// </summary>
    /// <param name="instanceID">Instance id of the elements gameObject.</param>
    /// <param name="modifier">1 = Increment, -1 = decrement</param>
    public void InvokeValueChangeOfElementWithID(int instanceID, int modifier)
    {
        UA11YElement element = Array.Find(accessibilityElements, e => e.gameObject.GetInstanceID() == instanceID);

        if (element != null)
        {
            if (modifier == -1)
            {
                element.InvokeEventOfType(UA11YElementInteractionEventType.Decrement);
            }
            else if (modifier == 1)
            {
                element.InvokeEventOfType(UA11YElementInteractionEventType.Increment);
            }
        }
    }

    /// <summary>
    /// Sets the focus on the element with the given identifier. 
    /// Also calls DidLoseFocus on the previous focused element (if available)
    /// </summary>
    /// <param name="instanceID">Instance id of the elements gameObject.</param>
    public void SetFocusOnElementWithID(int instanceID)
    {
        int index = Array.FindIndex(accessibilityElements, e => e.gameObject.GetInstanceID() == instanceID);

        if(index != -1)
        { 
            if(currentlyFocusedElement != null)
            {
                currentlyFocusedElement.InvokeEventOfType(UA11YElementInteractionEventType.LoseFocus);
            }

            UA11YElement newElement = accessibilityElements[index];
            newElement.InvokeEventOfType(UA11YElementInteractionEventType.BecomeFocused);

            currentlyFocusedElement = newElement;
        }
    }


    #endregion
}