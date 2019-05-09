using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;

public class KAPNativeScreenReaderBridge
{
#if UNITY_IOS && !UNITY_EDITOR

    [DllImport("__Internal")]
    private static extern bool KAPIsScreenReaderRunning();

    [DllImport("__Internal")]
    private static extern void KAPAddHookAtPosition(float x, float y, float width, float height, string label);

    [DllImport("__Internal")]
    private static extern void KAPClearAllHooks();

    private bool NativeScreenReaderAvailable() { return true; }
#else

    // TODO: Return UNKOWN instead of false. 
    private bool KAPIsScreenReaderRunning() { return false; }

    private void KAPAddHookAtPosition(float x, float y, float width, float height, string label) { }

    private void KAPClearAllHooks() { }

    private bool NativeScreenReaderAvailable() { return false; }
#endif

    public bool Available()
    {
        return NativeScreenReaderAvailable();
    }

    public void AddHook(Rect frame, string label)
    {
        if (label != null && label.Length > 0) 
        {
            // TODO: Frame conversion?
            KAPAddHookAtPosition(frame.x, frame.y, frame.width, frame.height, label);
            Debug.Log("The function was called");
        }
        else 
        {
            // TODO: Log Error
            Debug.LogError("Label is incorrect");
        }

    }

    public void ClearAllHooks()
    {
        KAPClearAllHooks();
    }
}
