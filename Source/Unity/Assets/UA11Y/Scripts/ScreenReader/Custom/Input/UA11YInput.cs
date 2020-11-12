﻿using UnityEngine;

public abstract class UA11YInput : MonoBehaviour
{
    // TODO: Check if Circular reference needs to be prevented (between Input and InputReceiver)!
    public IUA11YInputReceiver inputReceiver;

    public abstract string GetStatusText();
}