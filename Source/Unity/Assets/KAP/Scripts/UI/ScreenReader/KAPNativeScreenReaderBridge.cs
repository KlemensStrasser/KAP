using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
using AOT;

public delegate void KAPInvokeSelectionCallback(int instancdeID);
public delegate void KAPInvokeValueChangeCallback(int instancdeID, int modifier);

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
    public KAPInvokeValueChangeCallback valueChangeCallback;
}

public class KAPNativeScreenReaderBridge : IKAPScreenReader
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

    [DllImport("__Internal")]
    private static extern void KAPAnnoucnceVoiceOverMessage(string cString);

    private bool NativeScreenReaderAvailable() { return true; }
#else

    // TODO: Return UNKOWN instead of false. 
    private bool KAPIsScreenReaderRunning() { return false; }

    private void KAPUpdateHook(KAPExternalAccessibilityHook hook) { }
    private void KAPUpdateHooks(KAPExternalAccessibilityHook[] hooks, int size) { }
    private void KAPClearAllHooks() { }

    private void KAPAnnoucnceVoiceOverMessage(string cString) { }

    private bool NativeScreenReaderAvailable() { return false; }
#endif

    public bool Available()
    {
        return NativeScreenReaderAvailable();
    }

    #region IKAPScreenReader

    public void UpdateWithScreenReaderElements(KAPScreenReaderElement[] accessibilityElements, bool tryRetainingIndex = false)
    {
        KAPExternalAccessibilityHook[] hooks = new KAPExternalAccessibilityHook[accessibilityElements.Length];

        // TODO: Error handling
        for(int i = 0; i < accessibilityElements.Length; i++)
        {
            KAPScreenReaderElement accessibilityElement = accessibilityElements[i];
            KAPExternalAccessibilityHook hook = this.AccessibilityHookForElement(accessibilityElement);

            hooks[i] = hook;
        }

        KAPUpdateHooks(hooks, hooks.Length);
    }

    public void AnnounceMessage(string message)
    {
        if (message != null && message.Length > 0)
        {
            KAPAnnoucnceVoiceOverMessage(message);
        }
    }

    public void FocusElementWithID(int instanceID)
    { 
        // TODO: Implement
    }

    #endregion

    public void ClearAllHooks()
    {
        KAPClearAllHooks();
    }

    #region Static Callbacks

    [MonoPInvokeCallback(typeof(KAPInvokeSelectionCallback))]
    public static void InvokeSelectionCallback(int instanceID)
    {
        KAPUIManager.Instance.InvokeSelectionSilentlyOfElementWithID(instanceID);
    }

    [MonoPInvokeCallback(typeof(KAPInvokeValueChangeCallback))]
    public static void InvokeValueChangeCallback(int instanceID, int modifier)
    {
        KAPUIManager.Instance.InvokeValueChangeSilentlyOfElementWithID(instanceID, modifier);
    }

    #endregion

    #region Private Helpers

    KAPExternalAccessibilityHook AccessibilityHookForElement(KAPScreenReaderElement accessibilityElement) {

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
            valueChangeCallback = InvokeValueChangeCallback,
        };

        return hook;
    }

    #endregion
}
