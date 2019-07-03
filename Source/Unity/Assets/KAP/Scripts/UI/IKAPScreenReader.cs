using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IKAPScreenReader
{
    void UpdateWithKAPElements(KAPElement[] accessibilityElements, bool tryRetainingIndex = false);

    void AnnounceMessage(string Message);

    void FocusElementWithID(int instanceID);
}
