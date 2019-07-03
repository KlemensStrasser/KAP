using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System;

public class KAPCustomScreenReader : MonoBehaviour, IKAPInputReceiver, IKAPScreenReader
{
    private KAPSpeechSynthesizer speechSynthesizer;
    private KAPUIVisualizer kapVisualizer;
    KAPInput input;

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
        if (visualizerObject != null)
        {
            kapVisualizer = visualizerObject.GetComponent<KAPUIVisualizer>();
        }


        if(accessibilityElements != null && accessibilityElements.Length > 0)
        {
            UpdateFocusedElementIndex(0);
            AnnouceElementAtFocusedIndex(true);
        }
    }

    #region IKAPScreenReader

    public void UpdateWithScreenReaderElements(KAPScreenReaderElement[] accessibilityElements, bool tryRetainingIndex = false)
    {
        KAPScreenReaderElement oldSelectedElement = FocusedElement();

        this.accessibilityElements = accessibilityElements;

        // Annouce the very first one.
        if (accessibilityElements != null && accessibilityElements.Length > 0)
        {
            if(tryRetainingIndex && oldSelectedElement != null)
            {
                int oldInstanceID = oldSelectedElement.gameObject.GetInstanceID();
                int indexInNewArray = Array.FindIndex(accessibilityElements, element => element.gameObject.GetInstanceID() == oldInstanceID);

                if(indexInNewArray != -1)
                {
                    // Don't call UpdateSelectedElementIndex because the element is the same. No need to call the Focus methods 
                    focusedElementIndex = indexInNewArray;
                }
                else 
                {
                    UpdateFocusedElementIndex(0);
                    AnnouceElementAtFocusedIndex(true);
                }
            }
            else 
            {
                UpdateFocusedElementIndex(0);
                AnnouceElementAtFocusedIndex(true);
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
            UpdateFocusedElementIndex(instanceIndex);
            AnnouceElementAtFocusedIndex(true);
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
        KAPScreenReaderElement selectedElement = FocusedElement();

        if (selectedElement != null && kapVisualizer != null)
        {
            kapVisualizer.DrawIndicatorForElement(selectedElement);
        }
    }

    #endregion

    #region Sounds

    private void AnnouceElementAtFocusedIndex(bool includeDescription)
    {
        if (speechSynthesizer != null)
        {
            KAPScreenReaderElement element = this.FocusedElement();

            if (element != null)
            {
                string text;
                if (includeDescription)
                {
                    text = element.FullLabel();
                }
                else
                {
                    text = element.LabelWithTraitAndValue();
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
        KAPScreenReaderElement selectedElement = this.FocusedElement();
        if (speechSynthesizer != null && selectedElement != null && selectedElement.value != null)
        {
            string text = selectedElement.value;
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
            UpdateFocusedElementIndex(focusedElementIndex + 1);
            PlayFocusSound();
            AnnouceElementAtFocusedIndex(true);
        }
        else
        {
            PlayBlockingSound();
            AnnouceElementAtFocusedIndex(false);
        }
    }

    public void FocusPreviousElement()
    {
        if (focusedElementIndex > 0)
        {
            UpdateFocusedElementIndex(focusedElementIndex - 1);
            PlayFocusSound();
            AnnouceElementAtFocusedIndex(true);
        }
        else
        {
            PlayBlockingSound();
            AnnouceElementAtFocusedIndex(false);
        }
    }

    public void SelectFocusedElement()
    {
        KAPScreenReaderElement selectedElement = this.FocusedElement();
        if (selectedElement)
        {
            selectedElement.InvokeSelection();
            PlaySelectSound();
            AnnouceElementAtFocusedIndex(true);
        }
    }

    public void IncrementValueOfFocuedElement()
    {
        KAPScreenReaderElement selectedElement = this.FocusedElement();
        if (selectedElement)
        {
            string oldValue = selectedElement.value;
            selectedElement.InvokeIncrement();
            string newValue = selectedElement.value;

            if (oldValue == newValue)
            {
                PlayBlockingSound();
            }
            AnnouceElementAtFocusedIndex(true);
        }
    }

    public void DecrementValueOfFocuedElement()
    {
        KAPScreenReaderElement selectedElement = this.FocusedElement();
        if (selectedElement)
        {
            string oldValue = selectedElement.value;
            selectedElement.InvokeDecrement();
            string newValue = selectedElement.value;

            if (oldValue == newValue)
            {
                PlayBlockingSound();
            }
            AnnouceElementAtFocusedIndex(true);
        }
    }

    public void FocusElementAtPosition(Vector2 position)
    {
        int touchedElementIndex = IndexForTopElementAtPosition(position);

        if (touchedElementIndex != -1 && focusedElementIndex != touchedElementIndex)
        {
            UpdateFocusedElementIndex(touchedElementIndex);
            PlayFocusSound();
            AnnouceElementAtFocusedIndex(true);
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
    /// Safe way of accessibng the focused element
    /// </summary>
    /// <returns>Returns the currently focused element or null</returns>
    private KAPScreenReaderElement FocusedElement()
    {
        KAPScreenReaderElement element;
        if (focusedElementIndex >= 0 && accessibilityElements != null && focusedElementIndex < accessibilityElements.Length)
        {
            element = accessibilityElements[focusedElementIndex];
        }
        else
        {
            element = null;
        }
        return element;
    }

    private void UpdateFocusedElementIndex(int newFocusedElementIndex)
    {
        if (focusedElementIndex != newFocusedElementIndex)
        {
            KAPScreenReaderElement previousSelectedElement = FocusedElement();
            if (previousSelectedElement != null)
            {
                previousSelectedElement.DidLoseFocus();
            }

            focusedElementIndex = newFocusedElementIndex;

            KAPScreenReaderElement newSelectedElement = FocusedElement();
            if (newSelectedElement != null)
            {
                newSelectedElement.DidBecomeFocused();
            }
        }
    }

    #endregion
}
