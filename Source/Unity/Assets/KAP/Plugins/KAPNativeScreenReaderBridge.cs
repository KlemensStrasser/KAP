﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;

[StructLayout(LayoutKind.Sequential)]
public struct KAPExternalAccessibilityHook
{
    public int instanceID;
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
    private static extern void KAPAddHook(KAPExternalAccessibilityHook hook);

    [DllImport("__Internal")]
    private static extern void KAPUpdateHooks(KAPExternalAccessibilityHook[] hooks, int size);

    [DllImport("__Internal")]
    private static extern void KAPClearAllHooks();

    private bool NativeScreenReaderAvailable() { return true; }
#else

    // TODO: Return UNKOWN instead of false. 
    private bool KAPIsScreenReaderRunning() { return false; }

    private void KAPAddHook(KAPExternalAccessibilityHook hook) { }

    private void KAPUpdateHooks(KAPExternalAccessibilityHook[] hooks, int size) { }

    private void KAPClearAllHooks() { }

    private bool NativeScreenReaderAvailable() { return false; }
#endif

    public bool Available()
    {
        return NativeScreenReaderAvailable();
    }

    public void AddHook(KAPElement accessibilityElement)
    {
        if (accessibilityElement.label != null && accessibilityElement.label.Length > 0)
        {
            KAPExternalAccessibilityHook hook = new KAPExternalAccessibilityHook() {
                instanceID = accessibilityElement.gameObject.GetInstanceID(),
                x = accessibilityElement.frame.x,
                y = accessibilityElement.frame.y,
                width = accessibilityElement.frame.width,
                height = accessibilityElement.frame.height,
                label = accessibilityElement.label,
            };
            KAPAddHook(hook);
        }
        else 
        {
            Debug.LogError("Label for hook is invalid");
        }

    }

    public void AddHooksForKAPElements(KAPElement[] accessibilityElements)
    {
        KAPExternalAccessibilityHook[] hooks = new KAPExternalAccessibilityHook[accessibilityElements.Length];

        // TODO: Error handling
        for(int i = 0; i < accessibilityElements.Length; i++)
        {
            KAPElement element = accessibilityElements[i];
            KAPExternalAccessibilityHook hook = new KAPExternalAccessibilityHook()
            {
                instanceID = element.gameObject.GetInstanceID(),
                x = element.frame.x,
                y = element.frame.y,
                width = element.frame.width,
                height = element.frame.height,
                label = element.label,
            };

            hooks[i] = hook;
        }

        //// Not possible because: Object contains non - primitive or non-blittable data
        // GCHandle arrayHandle = GCHandle.Alloc(hooks, GCHandleType.Pinned); ;
        KAPUpdateHooks(hooks, hooks.Length);
    }

    public void ClearAllHooks()
    {
        KAPClearAllHooks();
    }
}
