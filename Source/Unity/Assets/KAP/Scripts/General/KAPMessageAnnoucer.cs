using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Responsible to annouce a message
/// </summary>
public class KAPMessageAnnoucer
{
    private static KAPMessageAnnoucer _instance;
    /// <summary>
    /// KAPMessageAnnoucer Singleton
    /// </summary>
    public static KAPMessageAnnoucer Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new KAPMessageAnnoucer();
            }

            return _instance;
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:KAPMessageAnnoucer"/> class.
    /// Private, so that no second instance can be created
    /// </summary>
    private KAPMessageAnnoucer() { }

    public void AnnounceMessage(string message)
    {
        // Use the native screenreader if possible for annoucements
        // Fall back on the speech synthesizer
        if(KAPNativeScreenReaderBridge.Available)
        {
            KAPNativeScreenReaderBridge.Instance.AnnounceMessage(message);
        } 
        else
        {
            // TODO: Maybe add a priority setting
            KAPSpeechSynthesizer.Instance.StartSpeaking(message);
        }
    }
}
