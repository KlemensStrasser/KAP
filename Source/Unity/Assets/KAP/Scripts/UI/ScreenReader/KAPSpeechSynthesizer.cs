﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;

public class KAPSpeechSynthesizer {

#if UNITY_STANDALONE_OSX || UNITY_EDITOR_OSX
    [DllImport("KAPMac")]
    private static extern void VoiceSetup();

    [DllImport("KAPMac")]
    private static extern void VoiceStartSpeaking(string cString);

    [DllImport("KAPMac")]
    private static extern void VoicePauseSpeaking();

    [DllImport("KAPMac")]
    private static extern void VoiceContinueSpeaking();

    [DllImport("KAPMac")]
    private static extern void VoiceStopSpeaking();

    [DllImport("KAPMac")]
    private static extern bool VoiceIsSpeaking();

    [DllImport("KAPMac")]
    private static extern bool VoiceIsPaused();
#elif UNITY_IOS
    [DllImport ("__Internal")]
    private static extern void VoiceSetup();

    [DllImport ("__Internal")]
    private static extern void VoiceStartSpeaking(string cString);

    [DllImport ("__Internal")]
    private static extern void VoicePauseSpeaking();

    [DllImport ("__Internal")]
    private static extern void VoiceContinueSpeaking();

    [DllImport ("__Internal")]
    private static extern void VoiceStopSpeaking();

    [DllImport ("__Internal")]
    private static extern bool VoiceIsSpeaking();

    [DllImport ("__Internal")]
    private static extern bool VoiceIsPaused();
#else
    // Dummy Implementations for non supported platforms
    private void VoiceSetup() { }

    private void VoiceStartSpeaking(string cString) { }

    private void VoicePauseSpeaking() { }

    private void VoiceContinueSpeaking() { }

    private void VoicecStopSpeaking() { }

    private bool VoiceIsSpeaking() { return false; }

    private bool VoiceIsPaused() { return false; }
#endif

    private static KAPSpeechSynthesizer _instance;
    /// <summary>
    /// KAPSpeechSynthesizer Singleton
    /// Based on: https://gamedev.stackexchange.com/questions/116009/in-unity-how-do-i-correctly-implement-the-singleton-pattern
    /// </summary>
    public static KAPSpeechSynthesizer Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new KAPSpeechSynthesizer();
            }

            return _instance;
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:KAPSpeechSynthesizer"/> class.
    /// Private, so that no second instance can be created
    /// </summary>
    private KAPSpeechSynthesizer() 
    {
        VoiceSetup();
    }

    /// <summary>
    /// Starts speaking a text.
    /// If anything is currently spoken, the spoken text will be stopped and the queue will be cleared
    /// </summary>
    /// <param name="text">The text that needs to be spoken.</param>
    public void StartSpeakingImmediately(string text)
    {
        if (text != null && text.Length > 0)
        {
            VoiceStartSpeaking(text);
        }
        else
        {
            Debug.LogWarning("KAPSpeechSynthesizer: Message cannot be spoken.");
        }
    }

    /// <summary>
    /// Starts speaking a text.
    /// If anything is currently spoken, the given text will be added to the queue and spoken later
    /// </summary>
    /// <param name="text">The text that needs to be spoken.</param>
    public void StartSpeaking(string text)
    {
        if(text != null && text.Length > 0)
        {
            VoiceStartSpeaking(text);    
        } 
        else 
        {
            Debug.LogWarning("KAPSpeechSynthesizer: Message cannot be spoken.");
        }
    }

    /// <summary>
    /// Pauses the output from speech synthesizer
    /// Does not clear the queue of text that still needs to be spoken
    /// </summary>
    public void PauseSpeaking()
    {
        VoicePauseSpeaking();
    }

    /// <summary>
    /// Continue speaking any intterrupted text and text that is in the queue, if speaking was paused before
    /// </summary>
    public void ContinueSpeaking() 
    {
        VoiceContinueSpeaking();
    }

    /// <summary>
    /// Stops the output from speech synthesizer
    /// Also clears the queue of text that should be spoken
    /// </summary>
    public void StopSpeaking() 
    {
        VoiceStopSpeaking();
    }

    /// <summary>
    /// Check if there is currently a output by the speech synthesizer
    /// </summary>
    /// <returns><c>true</c>, if the synthesizer currently speaks, <c>false</c> otherwise.</returns>
    public bool IsSpeaking()
    {
        return VoiceIsSpeaking();
    }

    /// <summary>
    /// Check if speaking is currently paused
    /// </summary>
    /// <returns><c>true</c>, if speaking is currently paused, <c>false</c> otherwise.</returns>
    public bool IsPaused() 
    {
        return VoiceIsPaused();
    }
}
