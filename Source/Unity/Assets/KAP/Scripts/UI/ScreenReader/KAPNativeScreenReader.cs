using System;

/// <summary>
/// This needs to be a singleton to make the callback 
/// </summary>
public class KAPNativeScreenReader: IKAPScreenReader
{
    private KAPScreenReaderElement[] accessibilityElements;

    private KAPScreenReaderElement currentlyFocusedElement;

    public  KAPNativeScreenReader() 
    {
        currentlyFocusedElement = null;

        KAPNativeScreenReaderBridge.Instance.selectionCallback = InvokeSelectionOfElementWithID;
        KAPNativeScreenReaderBridge.Instance.valueChangeCallback = InvokeValueChangeOfElementWithID;
        KAPNativeScreenReaderBridge.Instance.focusCallback = SetFocusOnElementWithID;
    }

    #region IKAPScreenReader

    public void UpdateWithScreenReaderElements(KAPScreenReaderElement[] accessibilityElements, bool tryRetainingIndex = false)
    {
        this.accessibilityElements = accessibilityElements;
        KAPNativeScreenReaderBridge.Instance.UpdateWithScreenReaderElements(accessibilityElements);
    }


    /// <summary>
    /// Focuses the given element.
    /// </summary>
    /// <param name="elementToFocus">Element to focus.</param>
    /// 
    /// This needs to call the native plugin to change the focus of the native screen reader, which will trigger the InvokeFocuseCallback
    public void FocusElement(KAPScreenReaderElement elementToFocus)
    {
        // TODO: IMPLEMENT FULLY
        int targetInstanceID = elementToFocus.gameObject.GetInstanceID();
    }

    public void SetEnabled(bool enabled)
    { 
    }

    #endregion


    #region Callbacks

    /// <summary>
    /// Invokes the selection of the element with given identifier.
    /// </summary>
    /// <param name="instanceID">Instance id of the elements gameObject.</param>
    public void InvokeSelectionOfElementWithID(int instanceID)
    {
        KAPScreenReaderElement element = Array.Find(accessibilityElements, e => e.gameObject.GetInstanceID() == instanceID);

        if (element != null)
        {
            element.InvokeSelection();
        }
    }

    /// <summary>
    /// Invokes the value change of the element with the given identifier.
    /// </summary>
    /// <param name="instanceID">Instance id of the elements gameObject.</param>
    /// <param name="modifier">1 = Increment, -1 = decrement</param>
    public void InvokeValueChangeOfElementWithID(int instanceID, int modifier)
    {
        KAPScreenReaderElement element = Array.Find(accessibilityElements, e => e.gameObject.GetInstanceID() == instanceID);

        if (element != null)
        {
            if (modifier == -1)
            {
                element.InvokeDecrement();
            }
            else if (modifier == 1)
            {
                element.InvokeIncrement();
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
                currentlyFocusedElement.DidLoseFocus();
            }

            KAPScreenReaderElement newElement = accessibilityElements[index];
            newElement.DidBecomeFocused();

            currentlyFocusedElement = newElement;
        }
    }


    #endregion
}