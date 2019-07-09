using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IKAPScreenReader
{
    void UpdateWithScreenReaderElements(KAPElement[] accessibilityElements, bool tryRetainingIndex = false);

    void FocusElement(KAPElement elementToFocus);

    void SetEnabled(bool enabled);
}
