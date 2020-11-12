using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System;

public class UA11YCustomScreenReader : MonoBehaviour, IUA11YInputReceiver, IUA11YScreenReader
{
    private UA11YUIVisualizer UA11YVisualizer;
    UA11YInput input;

    private UA11YElement focusedElement;
    private int focusedElementIndex;

    private AudioSource soundEffectAudioSource;

    private AudioClip focusAudioClip;
    private AudioClip blockAudioClip;
    private AudioClip selectAudioClip;

    UA11YElement[] accessibilityElements;

    private void Awake()
    {
        // Create the correct Input depending on the available device
#if UNITY_STANDALONE || UNITY_EDITOR
        input = gameObject.AddComponent<UA11YDesktopInput>();
        input.inputReceiver = this;
#elif UNITY_IOS || UNITY_ANDROID
        input = gameObject.AddComponent<UA11YMobileInput>();
        input.inputReceiver = this;
#endif

        // Initialize Sounds
        soundEffectAudioSource = gameObject.AddComponent<AudioSource>();
        focusAudioClip = Resources.Load("Audio/UA11Y_focus") as AudioClip;
        blockAudioClip = Resources.Load("Audio/UA11Y_block") as AudioClip;
        selectAudioClip = Resources.Load("Audio/UA11Y_select") as AudioClip;

        // Initialize Visualizer
        GameObject visualizerObject = Resources.Load<GameObject>("Prefabs/UI/UA11YUIVisualizer");
        visualizerObject = Instantiate<GameObject>(visualizerObject);
        visualizerObject.name = "UA11YUIVisualizer";
        visualizerObject.gameObject.transform.SetParent(gameObject.transform);
        if (visualizerObject != null)
        {
            UA11YVisualizer = visualizerObject.GetComponent<UA11YUIVisualizer>();
        }

        if (accessibilityElements != null && accessibilityElements.Length > 0 && focusedElement == null)
        {
            UpdateFocusedElement(0);
            AnnouceFocusedElement(true);
        }
    }

    #region IUA11YScreenReader

    public void UpdateWithScreenReaderElements(UA11YElement[] accessibilityElements, bool tryRetainingIndex = false)
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

    public void FocusElement(UA11YElement elementToFocus)
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
            Debug.LogWarning("UA11YCustomScreenReader: Accessibility element with instanceID could not be found!");
        }
    }

    public void SetEnabled(bool enabled)
    {
        // TODO: Maybe handle that differrently?
        this.gameObject.SetActive(enabled);
    }

    #endregion

    #region Visualization

    private void OnGUI()
    {
        // Update visualizer
        if (focusedElement != null && UA11YVisualizer != null)
        {
            UA11YVisualizer.DrawIndicatorForElement(focusedElement);
        }
    }

    #endregion

    #region Sounds

    private void AnnouceFocusedElement(bool includeDescription)
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

            UA11YSpeechSynthesizer.Instance.StartSpeakingImmediately(text);
        }
        else
        {
            Debug.LogWarning("UA11Y Element at the selectedElementIndex is null!");
        }
    }

    private void AnnouceValueOfFocusedElement()
    {
        if (focusedElement != null && focusedElement.value != null)
        {
            string text = focusedElement.value;
            UA11YSpeechSynthesizer.Instance.StartSpeakingImmediately(text);
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


    #region IUA11YInputReceiver

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
            focusedElement.InvokeEventOfType(UA11YElementInteractionEventType.Click);
            PlaySelectSound();
            AnnouceFocusedElement(true);
        }
    }

    public void IncrementValueOfFocuedElement()
    {
        if (focusedElement)
        {
            string oldValue = focusedElement.value;
            focusedElement.InvokeEventOfType(UA11YElementInteractionEventType.Increment);
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
            focusedElement.InvokeEventOfType(UA11YElementInteractionEventType.Decrement);
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
            UA11YElement element = accessibilityElements[i];
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
        Debug.LogWarning("UA11YUIManager: Escape Gesture is not implemented yet");
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
                focusedElement.InvokeEventOfType(UA11YElementInteractionEventType.LoseFocus);
            }

            UA11YElement newFocusedElement = accessibilityElements[newFocusedElementIndex];
            newFocusedElement.InvokeEventOfType(UA11YElementInteractionEventType.BecomeFocused);

            focusedElement = newFocusedElement;
            focusedElementIndex = newFocusedElementIndex;
        }
    }

    #endregion
}
