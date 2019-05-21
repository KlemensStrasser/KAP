﻿using UnityEngine;

public interface IKAPInputReceiver
{
    /// Set the focus at the next element
    void FocusNextElement();

    /// Set the focus at the previous element
    void FocusPreviousElement();

    /// Try to set the focus at a specified point
    /// Usually triggered by a click or a touch
    /// If there is no element at that position, the currently highlighted 
    /// element will stay highlighted
    void FocusElementAtPosition(Vector2 position);

    /// Select the focused element (aka click a button...)
    void SelectFocusedElement();

    /// Increment the value of the focused element (aka change a slider value)
    void IncrementValueOfFocuedElement();

    /// Decrement the value of the focused element (aka change a slider value)
    void DecrementValueOfFocuedElement();

    /// Escpae gesture is 
    void HandleEscapeGesture();

}
