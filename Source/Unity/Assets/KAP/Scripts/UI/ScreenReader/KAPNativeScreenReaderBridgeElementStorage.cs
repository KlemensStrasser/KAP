using System;

/// <summary>
/// Storage for the accessibility elements
/// </summary>
/// We need this because we cannot access the elements from the static callback methods
public class KAPNativeScreenReaderBridgeElementStorage
{
    private static KAPNativeScreenReaderBridgeElementStorage _instance;
    /// <summary>
    /// KAPNativeScreenReaderBridgeElementStorage Singleton
    /// Based on: https://gamedev.stackexchange.com/questions/116009/in-unity-how-do-i-correctly-implement-the-singleton-pattern
    /// </summary>
    public static KAPNativeScreenReaderBridgeElementStorage Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new KAPNativeScreenReaderBridgeElementStorage();
            }

            return _instance;
        }
    }

    /// <summary>
    /// Array of the KAPScreenReaderElement that are currently visible to the screen reader
    /// </summary>
    private KAPScreenReaderElement[] accessibilityElements;

    private KAPScreenReaderElement currentlyFocusedElement;

    private KAPNativeScreenReaderBridgeElementStorage() 
    {
        currentlyFocusedElement = null;
    }

    public void SetAccessibilityElements(KAPScreenReaderElement[] accessibilityElements)
    {
        this.accessibilityElements = accessibilityElements;
    }

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