using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IKAPScreenReader
{
    void UpdateWithScreenReaderElements(KAPScreenReaderElement[] accessibilityElements, bool tryRetainingIndex = false);

    void AnnounceMessage(string Message);

    void FocusElement(KAPScreenReaderElement elementToFocus);
}
