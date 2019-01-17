using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KAPManager : MonoBehaviour, IKAPInputReceiver
{
    private int selectedElementIndex;
    private KAPSpeechSynthesizer speechSynthesizer;

    private AudioSource soundEffectAudioSource;

    private AudioClip focusAudioClip;
    private AudioClip blockAudioClip;
    private AudioClip selectAudioClip;

    KAPInput input;
    KAPElement[] accessibilityElements;

    // TODO: Language

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
        selectedElementIndex = -1;

        if (accessibilityElements != null && accessibilityElements.Length > 0)
        {
            SortByFrame();

            UpdateSelectedElementIndex(0);
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
    }

    #region Sounds

    private void AnnouceElementAtSelectedIndex()
    {
        if (speechSynthesizer != null)
        {
            KAPElement element = this.SelectedElement();

            if (element != null)
            {
                speechSynthesizer.StartSpeaking(element.LabelWithTrait());
            }
            else
            {
                Debug.LogWarning("KAP Element at the selectedElementIndex is null!");
            }
        }
        else
        {
            Debug.LogWarning("The speechSynthesizer is null!");
        }
    }

    private void PlayFocusSound() 
    {
        soundEffectAudioSource.Stop();
        soundEffectAudioSource.PlayOneShot(focusAudioClip);
    }

    private void PlayBlockingSound()
    {
        soundEffectAudioSource.Stop();
        soundEffectAudioSource.PlayOneShot(blockAudioClip);
    }

    private void PlaySelectSound()
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
            UpdateSelectedElementIndex(selectedElementIndex + 1);
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
            UpdateSelectedElementIndex(selectedElementIndex - 1);
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

    #region "Notifications"

    /// Stops any text that is currently spoken and annouces the given message
    public void AnnouceMessage(string message) 
    {
        if (message != null && speechSynthesizer != null)
        {
            speechSynthesizer.StartSpeaking(message);
        }
    }

    /// Tries to set the focus on a given KAPElement
    public void FocusElement(KAPElement element) 
    {
        int index = Array.IndexOf(accessibilityElements, element);
        if(index != -1) {
            UpdateSelectedElementIndex(index);
            AnnouceElementAtSelectedIndex();
        }
    }

    /// Tries to set the focus on a KAPElement that is attached to the given
    /// GameObject
    public void FocusGameObject(GameObject gObject)
    {
        int index = Array.FindIndex(accessibilityElements, element => element.gameObject == gObject);
        if (index != -1)
        {
            UpdateSelectedElementIndex(index);
            AnnouceElementAtSelectedIndex();
        }
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

    private void UpdateSelectedElementIndex(int newSelectedElementIndex) 
    {
        if(selectedElementIndex != newSelectedElementIndex) 
        {
            KAPElement previousSelectedElement = SelectedElement();
            if(previousSelectedElement != null) 
            {
                previousSelectedElement.DidLoseFocus();
            }

            selectedElementIndex = newSelectedElementIndex;

            KAPElement newSelectedElement = SelectedElement();
            if (newSelectedElement != null)
            {
                newSelectedElement.DidBecomeFocused();
            }
        }
    }

    #endregion
}
