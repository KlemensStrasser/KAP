using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KAPManager : MonoBehaviour, IKAPInputReceiver
{
    private uint selectedElementIndex;
    private KAPSpeechSynthesizer speechSynthesizer;

    private AudioSource soundEffectAudioSource;

    private AudioClip focusAudioClip;
    private AudioClip blockAudioClip;
    private AudioClip selectAudioClip;

    KAPInput input;
    KAPElement[] accessibilityElements;
    // TODO: No Next/Previous Element sound

    // TODO: Language

    // TODO: Implement Notifications for:
    //       - Announcing specific string 
    //       - Changing Focus on specific element

    // Use this for initialization
    void Start()
    {
        // TODO: Add the correct input for the current device
        input = gameObject.AddComponent<KAPKeyboardInput>();
        input.inputReceiver = this;

        soundEffectAudioSource = gameObject.AddComponent<AudioSource>();
        focusAudioClip = Resources.Load("Audio/kap_focus") as AudioClip;
        blockAudioClip = Resources.Load("Audio/kap_block") as AudioClip;
        selectAudioClip = Resources.Load("Audio/kap_select") as AudioClip;

        accessibilityElements = FindObjectsOfType<KAPElement>();

        // TODO: Settings
        speechSynthesizer = new KAPSpeechSynthesizer();

        selectedElementIndex = 0;
        if (accessibilityElements != null && accessibilityElements.Length > 0)
        {
            SortByFrame();
            AnnouceElementAtSelectedIndex();
        }
    }

    void SortByFrame()
    {
        Array.Sort(accessibilityElements, delegate (KAPElement element1, KAPElement element2)
        {
            int comparrisonResult = element2.frame.y.CompareTo(element1.frame.y);
            if (comparrisonResult == 0)
            {
                comparrisonResult = element1.frame.x.CompareTo(element2.frame.x);
            }
            return comparrisonResult;
        });

        foreach (KAPElement element in accessibilityElements)
        {
            Debug.Log(element.label + " Pos" + element.frame.position);
        }
    }

    #region Sounds

    void AnnouceElementAtSelectedIndex()
    {
        if (speechSynthesizer != null)
        {
            KAPElement element = this.SelectedElement();

            if (element != null)
            {
                speechSynthesizer.StartSpeaking(element.label);
            }
            else
            {
                // TODO: LOG ERROR
            }
        }
        else
        {
            // TODO: Log Error!
        }
    }

    void PlayFocusSound() 
    {
        soundEffectAudioSource.Stop();
        soundEffectAudioSource.PlayOneShot(focusAudioClip);
    }

    void PlayBlockingSound()
    {
        soundEffectAudioSource.Stop();
        soundEffectAudioSource.PlayOneShot(blockAudioClip);
    }

    void PlaySelectSound()
    {
        soundEffectAudioSource.Stop();
        soundEffectAudioSource.PlayOneShot(selectAudioClip);
    }

    #endregion

    #region IKAPInputReceiver

    public void FocusNextElement()
    {
        if (selectedElementIndex + 1 < accessibilityElements.Length)
        {
            selectedElementIndex += 1;
            PlayFocusSound();
            AnnouceElementAtSelectedIndex();
        }
        else
        {
            PlayBlockingSound();
            AnnouceElementAtSelectedIndex();
        }
    }

    public void FocusPreviousElement()
    {
        if (selectedElementIndex > 0)
        {
            selectedElementIndex -= 1;
            PlayFocusSound();
            AnnouceElementAtSelectedIndex();
        }
        else
        {
            PlayBlockingSound();
            AnnouceElementAtSelectedIndex();
        }
    }

    public void SelectFocusedElement()
    {
        // TODO: Play Slection sound
        KAPElement selectedElement = this.SelectedElement();
        if (selectedElement)
        {
            selectedElement.InvokeSelection();
            PlaySelectSound();
            AnnouceElementAtSelectedIndex();
        }
    }


    public void FocusElementAtPosition(Vector2 position)
    {
        throw new System.NotImplementedException();
    }

    public void HandleEscapeGesture()
    {
        throw new System.NotImplementedException();
    }

    #endregion

    #region Private Helpers

    private KAPElement SelectedElement() 
    {
        KAPElement element;
        if(selectedElementIndex >= 0 && selectedElementIndex < accessibilityElements.Length) 
        {
            element = accessibilityElements[selectedElementIndex];
        } 
        else 
        {
            element = null;
        }
        return element;
    }

    #endregion
}
