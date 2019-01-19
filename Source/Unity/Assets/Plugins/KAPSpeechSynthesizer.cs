using System.Collections;
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

    public void Setup()
    {
        VoiceSetup();
    }

    public void StartSpeaking(string text)
    {
        if(text != null && text.Length > 0)
        {
            VoiceStartSpeaking(text);    
        } 
        else 
        {
            StopSpeaking();
        }
    }

    public void PauseSpeaking()
    {
        VoicePauseSpeaking();
    }

    public void ContinueSpeaking() 
    {
        VoiceContinueSpeaking();
    }

    public void StopSpeaking() 
    {
        VoiceStopSpeaking();
    }

    public bool IsSpeaking()
    {
        return VoiceIsSpeaking();
    }

    public bool IsPaused() 
    {
        return VoiceIsPaused();
    }
}
