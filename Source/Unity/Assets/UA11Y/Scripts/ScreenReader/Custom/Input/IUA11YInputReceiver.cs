using UnityEngine;

public interface IUA11YInputReceiver
{
    /// Set the focus at the next Element
    void FocusNextElement();

    /// Set the focus at the previous Element
    void FocusPreviousElement();

    /// Try to set the focus at a specified point
    /// Usually triggered by a click or a touch
    /// If there is no Element at that position, the currently highlighted 
    /// Element will stay highlighted
    void FocusElementAtPosition(Vector2 position);

    /// Select the focused Element (aka click a button...)
    void SelectFocusedElement();

    /// Increment the value of the focused Element (aka change a slider value)
    void IncrementValueOfFocuedElement();

    /// Decrement the value of the focused Element (aka change a slider value)
    void DecrementValueOfFocuedElement();

    /// Escpae gesture is 
    void HandleEscapeGesture();

}
