using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
using AOT;

public delegate void KAPInvokeSelectionCallback(int instancdeID);

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
    public string value;
    public string hint;

    public ulong  trait;

    public KAPInvokeSelectionCallback selectionCallback;
}

public class KAPNativeScreenReaderBridge
{
#if UNITY_IOS && !UNITY_EDITOR

    [DllImport("__Internal")]
    private static extern bool KAPIsScreenReaderRunning();

    [DllImport("__Internal")]
    private static extern void KAPUpdateHook(KAPExternalAccessibilityHook hooks);

    [DllImport("__Internal")]
    private static extern void KAPUpdateHooks(KAPExternalAccessibilityHook[] hooks, int size);

    [DllImport("__Internal")]
    private static extern void KAPClearAllHooks();

    private bool NativeScreenReaderAvailable() { return true; }
#else

    // TODO: Return UNKOWN instead of false. 
    private bool KAPIsScreenReaderRunning() { return false; }

    private void KAPUpdateHook(KAPExternalAccessibilityHook hook) { }
    private void KAPUpdateHooks(KAPExternalAccessibilityHook[] hooks, int size) { }
    private void KAPClearAllHooks() { }

    private bool NativeScreenReaderAvailable() { return false; }
#endif

    public bool Available()
    {
        return NativeScreenReaderAvailable();
    }

    // This might be used in rare cases. 
    public void UpdateHookForKAPElements(KAPElement accessibilityElement)
    {
        KAPExternalAccessibilityHook hook = this.AccessibilityHookForElement(accessibilityElement);
        KAPUpdateHook(hook);
    }

    public void UpdateHooksForKAPElements(KAPElement[] accessibilityElements)
    {
        KAPExternalAccessibilityHook[] hooks = new KAPExternalAccessibilityHook[accessibilityElements.Length];

        // TODO: Error handling
        for(int i = 0; i < accessibilityElements.Length; i++)
        {
            KAPElement accessibilityElement = accessibilityElements[i];
            KAPExternalAccessibilityHook hook = this.AccessibilityHookForElement(accessibilityElement);

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

    #region Static Callbacks

    [MonoPInvokeCallback(typeof(KAPInvokeSelectionCallback))]
    public static void InvokeSelectionCallback(int instanceID)
    {
        KAPManager.Instance.InvokeSelectionSilentlyOfElementWithID(instanceID);
    }
    #endregion

    #region Private Helpers

    KAPExternalAccessibilityHook AccessibilityHookForElement(KAPElement accessibilityElement) {

        KAPExternalAccessibilityHook hook = new KAPExternalAccessibilityHook()
        {
            instanceID = accessibilityElement.gameObject.GetInstanceID(),
            x = accessibilityElement.frame.x,
            y = accessibilityElement.frame.y,
            width = accessibilityElement.frame.width,
            height = accessibilityElement.frame.height,
            label = accessibilityElement.label,
            value = accessibilityElement.value,
            hint = accessibilityElement.description,
            trait = accessibilityElement.traits.Value,

            selectionCallback = InvokeSelectionCallback,
        };

        return hook;
    }

    #endregion
}
