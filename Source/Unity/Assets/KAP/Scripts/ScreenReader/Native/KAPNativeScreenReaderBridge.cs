using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
using AOT;
using System;

public delegate void KAPInvokeSelectionCallback(int instanceID);
public delegate void KAPInvokeFocusCallback(int instanceID);
public delegate void KAPInvokeValueChangeCallback(int instanceID, int modifier);

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
    public KAPInvokeFocusCallback focusCallback;
    public KAPInvokeValueChangeCallback valueChangeCallback;
}

public class KAPNativeScreenReaderBridge
{
#if UNITY_IOS && !UNITY_EDITOR

    public static bool Available = true;

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
#else

    public static bool Available = false;

    // TODO: Return UNKOWN instead of false. 
    private bool KAPIsScreenReaderRunning() { return false; }

    private void KAPUpdateHook(KAPExternalAccessibilityHook hook) { }
    private void KAPUpdateHooks(KAPExternalAccessibilityHook[] hooks, int size) { }
    private void KAPClearAllHooks() { }

    private void KAPAnnoucnceVoiceOverMessage(string cString) { }

#endif

    [HideInInspector]
    public KAPInvokeSelectionCallback selectionCallback;

    [HideInInspector]
    public KAPInvokeFocusCallback focusCallback;

    [HideInInspector]
    public KAPInvokeValueChangeCallback valueChangeCallback;


    private static KAPNativeScreenReaderBridge _instance;
    /// <summary>
    /// KAPNativeScreenReaderBridge Singleton
    /// </summary>
    public static KAPNativeScreenReaderBridge Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new KAPNativeScreenReaderBridge();
            }

            return _instance;
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:KAPNativeScreenReaderBridge"/> class.
    /// Private, so that no second instance can be created
    /// </summary>
    private KAPNativeScreenReaderBridge() { }

    public void UpdateWithScreenReaderElements(KAPElement[] accessibilityElements)
    {
        KAPExternalAccessibilityHook[] hooks = new KAPExternalAccessibilityHook[accessibilityElements.Length];

        // TODO: Error handling
        for (int i = 0; i < accessibilityElements.Length; i++)
        {
            KAPElement accessibilityElement = accessibilityElements[i];
            KAPExternalAccessibilityHook hook = this.AccessibilityHookForElement(accessibilityElement);

            hooks[i] = hook;
        }

        KAPUpdateHooks(hooks, hooks.Length);
    }

    public void ClearAllHooks()
    {
        KAPClearAllHooks();
    }

    public void AnnounceMessage(string message)
    {
        if (message != null && message.Length > 0)
        {
            KAPAnnoucnceVoiceOverMessage(message);
        }
    }

    #region Static Callbacks

    [MonoPInvokeCallback(typeof(KAPInvokeSelectionCallback))]
    public static void InvokeSelectionCallback(int instanceID)
    {
        // Is this the best way to to that? I don't know...
        if(Instance.selectionCallback != null)
        {
            Instance.selectionCallback(instanceID);
        }
    }

    [MonoPInvokeCallback(typeof(KAPInvokeValueChangeCallback))]
    public static void InvokeValueChangeCallback(int instanceID, int modifier)
    {
        // Is this the best way to to that? I don't know...
        if (Instance.valueChangeCallback != null)
        {
            Instance.valueChangeCallback(instanceID, modifier);
        }
    }

    [MonoPInvokeCallback(typeof(KAPInvokeFocusCallback))]
    public static void InvokeFocuseCallback(int instanceID)
    {
        // Is this the best way to to that? I don't know...
        if (Instance.focusCallback != null)
        {
            Instance.focusCallback(instanceID);
        }
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
            focusCallback = InvokeFocuseCallback,
            valueChangeCallback = InvokeValueChangeCallback,
        };

        return hook;
    }

    #endregion
}