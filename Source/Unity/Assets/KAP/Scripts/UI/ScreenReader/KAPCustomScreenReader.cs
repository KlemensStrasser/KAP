﻿using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System;

public class KAPCustomScreenReader : MonoBehaviour, IKAPInputReceiver, IKAPScreenReader
{
    private KAPSpeechSynthesizer speechSynthesizer;
    private KAPUIVisualizer kapVisualizer;
    KAPInput input;

    private KAPScreenReaderElement focusedElement;
    private int focusedElementIndex;

    private AudioSource soundEffectAudioSource;

    private AudioClip focusAudioClip;
    private AudioClip blockAudioClip;
    private AudioClip selectAudioClip;

    KAPScreenReaderElement[] accessibilityElements;

    private void Awake()
    {
        // Create the correct Input depending on the available device
#if UNITY_STANDALONE || UNITY_EDITOR
        input = gameObject.AddComponent<KAPDesktopInput>();
        input.inputReceiver = this;
#elif UNITY_IOS || UNITY_ANDROID
        input = gameObject.AddComponent<KAPMobileInput>();
        input.inputReceiver = this;
#endif

        speechSynthesizer = new KAPSpeechSynthesizer();

        // Initialize Sounds
        soundEffectAudioSource = gameObject.AddComponent<AudioSource>();
        focusAudioClip = Resources.Load("Audio/kap_focus") as AudioClip;
        blockAudioClip = Resources.Load("Audio/kap_block") as AudioClip;
        selectAudioClip = Resources.Load("Audio/kap_select") as AudioClip;

        // Initialize Visualizer
        GameObject visualizerObject = Resources.Load<GameObject>("Prefabs/UI/KAPUIVisualizer");
        visualizerObject = Instantiate<GameObject>(visualizerObject);
        visualizerObject.name = "KAPUIVisualizer";
        visualizerObject.gameObject.transform.SetParent(gameObject.transform);
        if (visualizerObject != null)
        {
            kapVisualizer = visualizerObject.GetComponent<KAPUIVisualizer>();
        }

        if (accessibilityElements != null && accessibilityElements.Length > 0 && focusedElement == null)
        {
            UpdateFocusedElement(0);
            AnnouceFocusedElement(true);
        }
    }

    #region IKAPScreenReader

    public void UpdateWithScreenReaderElements(KAPScreenReaderElement[] accessibilityElements, bool tryRetainingIndex = false)
    {
        this.accessibilityElements = accessibilityElements;

        if (accessibilityElements != null && accessibilityElements.Length > 0)
        {
            if(tryRetainingIndex && focusedElement != null)
            {
                int oldInstanceID = focusedElement.gameObject.GetInstanceID();
                int indexInNewArray = Array.FindIndex(accessibilityElements, element => element.gameObject.GetInstanceID() == oldInstanceID);

                // Focused element is not in the new array, so we have to highlight something else
                if(indexInNewArray == -1)
                {
                    UpdateFocusedElement(0);
                    AnnouceFocusedElement(true);
                } else
                {
                    // Update the index to the one in the new array
                    focusedElementIndex = indexInNewArray;
                }
            }
            else 
            {
                UpdateFocusedElement(0);
                AnnouceFocusedElement(true);
            }
        }
    }

    public void AnnounceMessage(string message)
    {
        if (message != null && speechSynthesizer != null)
        {
            // This might not work
            // TODO: Add queue to speechSynthesizer
            speechSynthesizer.StartSpeaking(message);
        }
    }

    public void FocusElement(KAPScreenReaderElement elementToFocus)
    {
        int targetInstanceID = elementToFocus.gameObject.GetInstanceID();
        int instanceIndex = Array.FindIndex(accessibilityElements, element => element.gameObject.GetInstanceID() == targetInstanceID);

        if (instanceIndex != -1)
        {
            UpdateFocusedElement(instanceIndex);
            AnnouceFocusedElement(true);
        } 
        else
        {
            Debug.LogWarning("KAPCustomScreenReader: Accessibility element with instanceID could not be found!");
        }
    }

    #endregion

    #region Visualization

    private void OnGUI()
    {
        // Update visualizer
        if (focusedElement != null && kapVisualizer != null)
        {
            kapVisualizer.DrawIndicatorForElement(focusedElement);
        }
    }

    #endregion

    #region Sounds

    private void AnnouceFocusedElement(bool includeDescription)
    {
        if (speechSynthesizer != null)
        {
            if (focusedElement != null)
            {
                string text;
                if (includeDescription)
                {
                    text = focusedElement.FullLabel();
                }
                else
                {
                    text = focusedElement.LabelWithTraitAndValue();
                }

                speechSynthesizer.StartSpeaking(text);
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

    private void AnnouceValueOfSelectedElement()
    {
        if (speechSynthesizer != null && focusedElement != null && focusedElement.value != null)
        {
            string text = focusedElement.value;
            speechSynthesizer.StartSpeaking(text);
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
        if (focusedElementIndex + 1 < accessibilityElements.Length)
        {
            UpdateFocusedElement(focusedElementIndex + 1);
            PlayFocusSound();
            AnnouceFocusedElement(true);
        }
        else
        {
            PlayBlockingSound();
            AnnouceFocusedElement(false);
        }
    }

    public void FocusPreviousElement()
    {
        if (focusedElementIndex > 0)
        {
            UpdateFocusedElement(focusedElementIndex - 1);
            PlayFocusSound();
            AnnouceFocusedElement(true);
        }
        else
        {
            PlayBlockingSound();
            AnnouceFocusedElement(false);
        }
    }

    public void SelectFocusedElement()
    {
        if (focusedElement)
        {
            focusedElement.InvokeSelection();
            PlaySelectSound();
            AnnouceFocusedElement(true);
        }
    }

    public void IncrementValueOfFocuedElement()
    {
        if (focusedElement)
        {
            string oldValue = focusedElement.value;
            focusedElement.InvokeIncrement();
            string newValue = focusedElement.value;

            if (oldValue == newValue)
            {
                PlayBlockingSound();
            }
            AnnouceFocusedElement(true);
        }
    }

    public void DecrementValueOfFocuedElement()
    {
        if (focusedElement)
        {
            string oldValue = focusedElement.value;
            focusedElement.InvokeDecrement();
            string newValue = focusedElement.value;

            if (oldValue == newValue)
            {
                PlayBlockingSound();
            }
            AnnouceFocusedElement(true);
        }
    }

    public void FocusElementAtPosition(Vector2 position)
    {
        int touchedElementIndex = IndexForTopElementAtPosition(position);

        if (touchedElementIndex != -1 && focusedElementIndex != touchedElementIndex)
        {
            UpdateFocusedElement(touchedElementIndex);
            PlayFocusSound();
            AnnouceFocusedElement(true);
        }
    }

    private int IndexForTopElementAtPosition(Vector2 position)
    {
        int index = -1;

        for (int i = 0; i < accessibilityElements.Length; i++)
        {
            KAPScreenReaderElement element = accessibilityElements[i];
            if (element.frame.Contains(position))
            {
                if (index == -1)
                {
                    index = i;
                }
            }
        }

        return index;
    }

    public void HandleEscapeGesture()
    {
        Debug.LogWarning("KAPUIManager: Escape Gesture is not implemented yet");
    }

    #endregion

    #region Private Helpers

    /// <summary>
    /// Updates the focused element.
    /// </summary>
    /// <param name="newFocusedElementIndex">Index of the new focused element in the accessibilityElements array.</param>
    private void UpdateFocusedElement(int newFocusedElementIndex)
    {
        if (newFocusedElementIndex >= 0 && newFocusedElementIndex < accessibilityElements.Length)
        {
            if (focusedElement != null)
            {
                focusedElement.DidLoseFocus();
            }

            KAPScreenReaderElement newFocusedElement = accessibilityElements[newFocusedElementIndex];
            newFocusedElement.DidBecomeFocused();

            focusedElement = newFocusedElement;
            focusedElementIndex = newFocusedElementIndex;
        }
    }

    #endregion
}