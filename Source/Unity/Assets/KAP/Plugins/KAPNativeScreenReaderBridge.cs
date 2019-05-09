using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;

[StructLayout(LayoutKind.Sequential)]
public struct KAPAccessibilityHook
{
    // TODO: Maybe we can wrap this in another struct
    public float x;
    public float y;
    public float width;
    public float height;

    public string label;
}

public class KAPNativeScreenReaderBridge
{
#if UNITY_IOS && !UNITY_EDITOR

    [DllImport("__Internal")]
    private static extern bool KAPIsScreenReaderRunning();

    [DllImport("__Internal")]
    private static extern void KAPAddHook(KAPAccessibilityHook hook);

    [DllImport("__Internal")]
    private static extern void KAPClearAllHooks();

    private bool NativeScreenReaderAvailable() { return true; }
#else

    // TODO: Return UNKOWN instead of false. 
    private bool KAPIsScreenReaderRunning() { return false; }

    private void KAPAddHook(KAPAccessibilityHook hook) { }

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
            KAPAccessibilityHook hook = new KAPAccessibilityHook() {
                x = frame.x,
                y = frame.y,
                width = frame.width,
                height = frame.height,
                label = label,
            };
            KAPAddHook(hook);
        }
        else 
        {
            Debug.LogError("Label for hook is invalid");
        }

    }

    public void ClearAllHooks()
    {
        KAPClearAllHooks();
    }
}
