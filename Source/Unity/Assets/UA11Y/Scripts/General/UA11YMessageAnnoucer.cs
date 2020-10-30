using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Responsible to annouce a message
/// </summary>
public class UA11YMessageAnnoucer
{
    private static UA11YMessageAnnoucer _instance;
    /// <summary>
    /// UA11YMessageAnnoucer Singleton
    /// </summary>
    public static UA11YMessageAnnoucer Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new UA11YMessageAnnoucer();
            }

            return _instance;
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:UA11YMessageAnnoucer"/> class.
    /// Private, so that no second instance can be created
    /// </summary>
    private UA11YMessageAnnoucer() { }

    public void AnnounceMessage(string message)
    {
        // Use the native screenreader if possible for annoucements
        // Fall back on the speech synthesizer
        if(UA11YNativeScreenReaderBridge.Available)
        {
            UA11YNativeScreenReaderBridge.Instance.AnnounceMessage(message);
        } 
        else
        {
            // TODO: Maybe add a priority setting
            UA11YSpeechSynthesizer.Instance.StartSpeaking(message);
        }
    }
}
