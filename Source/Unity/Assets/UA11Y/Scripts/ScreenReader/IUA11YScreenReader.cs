using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IUA11YScreenReader
{
    void UpdateWithScreenReaderElements(UA11YElement[] accessibilityElements, bool tryRetainingIndex = false);

    void FocusElement(UA11YElement elementToFocus);

    void SetEnabled(bool enabled);
}
