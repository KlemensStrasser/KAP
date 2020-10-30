using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
using AOT;
using System;

public delegate void UA11YInvokeSelectionCallback(int instanceID);
public delegate void UA11YInvokeFocusCallback(int instanceID);
public delegate void UA11YInvokeValueChangeCallback(int instanceID, int modifier);

[StructLayout(LayoutKind.Sequential)]
public struct UA11YExternalAccessibilityHook
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

    public UA11YInvokeSelectionCallback selectionCallback;
    public UA11YInvokeFocusCallback focusCallback;
    public UA11YInvokeValueChangeCallback valueChangeCallback;
}

public class UA11YNativeScreenReaderBridge
{
#if UNITY_IOS && !UNITY_EDITOR

    public static bool Available = true;

    [DllImport("__Internal")]
    private static extern bool UA11YIsScreenReaderRunning();

    [DllImport("__Internal")]
    private static extern void UA11YUpdateHook(UA11YExternalAccessibilityHook hooks);

    [DllImport("__Internal")]
    private static extern void UA11YUpdateHooks(UA11YExternalAccessibilityHook[] hooks, int size);

    [DllImport("__Internal")]
    private static extern void UA11YClearAllHooks();

    [DllImport("__Internal")]
    private static extern void UA11YAnnoucnceVoiceOverMessage(string cString);
#else

    public static bool Available = false;

    // TODO: Return UNKOWN instead of false. 
    private bool UA11YIsScreenReaderRunning() { return false; }

    private void UA11YUpdateHook(UA11YExternalAccessibilityHook hook) { }
    private void UA11YUpdateHooks(UA11YExternalAccessibilityHook[] hooks, int size) { }
    private void UA11YClearAllHooks() { }

    private void UA11YAnnoucnceVoiceOverMessage(string cString) { }

#endif

    [HideInInspector]
    public UA11YInvokeSelectionCallback selectionCallback;

    [HideInInspector]
    public UA11YInvokeFocusCallback focusCallback;

    [HideInInspector]
    public UA11YInvokeValueChangeCallback valueChangeCallback;


    private static UA11YNativeScreenReaderBridge _instance;
    /// <summary>
    /// UA11YNativeScreenReaderBridge Singleton
    /// </summary>
    public static UA11YNativeScreenReaderBridge Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new UA11YNativeScreenReaderBridge();
            }

            return _instance;
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:UA11YNativeScreenReaderBridge"/> class.
    /// Private, so that no second instance can be created
    /// </summary>
    private UA11YNativeScreenReaderBridge() { }

    public void UpdateWithScreenReaderElements(UA11YElement[] accessibilityElements)
    {
        UA11YExternalAccessibilityHook[] hooks = new UA11YExternalAccessibilityHook[accessibilityElements.Length];

        // TODO: Error handling
        for (int i = 0; i < accessibilityElements.Length; i++)
        {
            UA11YElement accessibilityElement = accessibilityElements[i];
            UA11YExternalAccessibilityHook hook = this.AccessibilityHookForElement(accessibilityElement);

            hooks[i] = hook;
        }

        UA11YUpdateHooks(hooks, hooks.Length);
    }

    public void ClearAllHooks()
    {
        UA11YClearAllHooks();
    }

    public void AnnounceMessage(string message)
    {
        if (message != null && message.Length > 0)
        {
            UA11YAnnoucnceVoiceOverMessage(message);
        }
    }

    #region Static Callbacks

    [MonoPInvokeCallback(typeof(UA11YInvokeSelectionCallback))]
    public static void InvokeSelectionCallback(int instanceID)
    {
        // Is this the best way to to that? I don't know...
        if(Instance.selectionCallback != null)
        {
            Instance.selectionCallback(instanceID);
        }
    }

    [MonoPInvokeCallback(typeof(UA11YInvokeValueChangeCallback))]
    public static void InvokeValueChangeCallback(int instanceID, int modifier)
    {
        // Is this the best way to to that? I don't know...
        if (Instance.valueChangeCallback != null)
        {
            Instance.valueChangeCallback(instanceID, modifier);
        }
    }

    [MonoPInvokeCallback(typeof(UA11YInvokeFocusCallback))]
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

    UA11YExternalAccessibilityHook AccessibilityHookForElement(UA11YElement accessibilityElement) {

        UA11YExternalAccessibilityHook hook = new UA11YExternalAccessibilityHook()
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