﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class KAPUIManager : MonoBehaviour, IKAPInputReceiver
{
    private int selectedElementIndex;
    private KAPSpeechSynthesizer speechSynthesizer;
    private KAPUIVisualizer kapVisualizer;
    private KAPNativeScreenReaderBridge nativeScreenReaderBridge;

    private AudioSource soundEffectAudioSource;

    private AudioClip focusAudioClip;
    private AudioClip blockAudioClip;
    private AudioClip selectAudioClip;

    KAPInput input;

    /// <summary>
    /// Array of the KAPElements that are currently visible to the screen reader
    /// </summary>
    KAPElement[] accessibilityElements;

    /// <summary>
    /// Boolean indicating if the accessibility elements need to be updated
    /// </summary>
    private bool needsUpdateElements;

    /// <summary>
    /// Boolean indicating if the selectedElementIndex should be retained when elements are updated
    /// </summary>
    private bool retainSelectedElementIndex;

    private static KAPUIManager _instance;
    /// <summary>
    /// KAPUIManager Singleton
    /// Based on: https://gamedev.stackexchange.com/questions/116009/in-unity-how-do-i-correctly-implement-the-singleton-pattern
    /// </summary>
    public static KAPUIManager Instance 
    { 
        get 
        {
            if (_instance == null)
            {
                GameObject instanceObject = Resources.Load<GameObject>("Prefabs/UI/KAPUIManager");
                _instance = Instantiate<GameObject>(instanceObject).GetComponent<KAPUIManager>();
            } 

            return _instance;
        } 
    }

    private void Awake()
    {
        if(_instance != null && _instance != this) 
        {
            Destroy(this.gameObject);    
        }
        else
        {
            _instance = this;
        }
    }

    void Start()
    {
        // Create the correct Input depending on the available device
#if UNITY_STANDALONE || UNITY_EDITOR
        input = gameObject.AddComponent<KAPDesktopInput>();
        input.inputReceiver = this;
#elif UNITY_IOS || UNITY_ANDROID
        input = gameObject.AddComponent<KAPMobileInput>();
        input.inputReceiver = this;
#endif
        // Initialize Sounds
        soundEffectAudioSource = gameObject.AddComponent<AudioSource>();
        focusAudioClip = Resources.Load("Audio/kap_focus") as AudioClip;
        blockAudioClip = Resources.Load("Audio/kap_block") as AudioClip;
        selectAudioClip = Resources.Load("Audio/kap_select") as AudioClip;

        // TODO: Settings, like language, volume etc.
        speechSynthesizer = new KAPSpeechSynthesizer();

        // Initialize Visualizer
        GameObject visualizerObject = Resources.Load<GameObject>("Prefabs/UI/KAPUIVisualizer");
        visualizerObject = Instantiate<GameObject>(visualizerObject);
        visualizerObject.name = "KAPUIVisualizer";
        if(visualizerObject != null) 
        {
            kapVisualizer = visualizerObject.GetComponent<KAPUIVisualizer>();
        }

        nativeScreenReaderBridge = new KAPNativeScreenReaderBridge();

        if (!nativeScreenReaderBridge.Available())
        {
            // Fetch Elements
            LoadAccessibilityElements();
            selectedElementIndex = -1;

            // Annouce the very first one.
            if (accessibilityElements != null && accessibilityElements.Length > 0)
            {
                UpdateSelectedElementIndex(0);
                AnnouceElementAtSelectedIndex(true);
            }
        }
    }

    void LoadAccessibilityElements()
    {
        accessibilityElements = FindObjectsOfType<KAPElement>();

        KAPElement[] popovers = accessibilityElements.Where(c => c is KAPPopover).ToArray();
        if(popovers != null && popovers.Length > 0)
        {
            accessibilityElements = GetAccessibilityElementsFromPopover((KAPPopover)popovers[0]);
        }

        if (accessibilityElements != null) 
        {
            SortByFrame();
        }

        if (nativeScreenReaderBridge.Available())
        {
            nativeScreenReaderBridge.UpdateHooksForKAPElements(accessibilityElements);
        } 
    }


    /// <summary>
    /// Gets the accessibility elements from the top popover.
    /// </summary>
    /// <returns>The accessibility elements from the top popover.</returns>
    /// <param name="popover">The popover we start with</param>
    KAPElement[] GetAccessibilityElementsFromPopover(KAPPopover popover)
    {
        KAPElement[] topAccessibilityElements;

        if (popover != null)
        {

            KAPPopover topPopover = popover;

            // As described in KAPPopover, popovers should only appear in the same hierachy (See KAPPopover.cs for an example)
            // Thus, any popOver we find in the children of the topPopover should be at least one step up this hierachy
            // It could be multiple steps, depending on the order we get the popovers

            bool searchForChildPopOvers = true;
            KAPPopover[] childPopovers;
            do
            {
                // Get all child popOvers
                childPopovers = topPopover.gameObject.GetComponentsInChildren<KAPPopover>();
                // Filter out the current topPopover
                childPopovers = childPopovers.Where(go => go.gameObject != topPopover.gameObject).ToArray();

                if (childPopovers != null && childPopovers.Length > 0)
                {
                    // No matter which object we use, we get at least one step down the hierachy
                    topPopover = childPopovers[0];
                }
                else
                {
                    // We are at the top -> Break!
                    searchForChildPopOvers = false;
                }

            } while (searchForChildPopOvers);


            topAccessibilityElements = topPopover.gameObject.GetComponentsInChildren<KAPElement>();
            // Filter out the popover
            topAccessibilityElements = topAccessibilityElements.Where(go => go.gameObject != topPopover.gameObject).ToArray();
        } 
        else
        {
            topAccessibilityElements = new KAPElement[0];
        }

        return topAccessibilityElements;
    }

    void SortByFrame()
    {
        Array.Sort(accessibilityElements, delegate (KAPElement element1, KAPElement element2)
        {
            int comparrisonResult = element1.frame.y.CompareTo(element2.frame.y);
            if (comparrisonResult == 0)
            {
                comparrisonResult = element1.frame.x.CompareTo(element2.frame.x);
            }
            return comparrisonResult;
        });
    }

    #region Sounds

    private void AnnouceElementAtSelectedIndex(bool includeDescription)
    {
        if (speechSynthesizer != null)
        {
            KAPElement element = this.SelectedElement();

            if (element != null)
            {
                string text;
                if(includeDescription)
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
        KAPElement selectedElement = this.SelectedElement();
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

    #region Visualization

    private void OnGUI()
    {
        // Update visualizer
        KAPElement selectedElement = SelectedElement();

        if(selectedElement != null && kapVisualizer != null)
        {
            kapVisualizer.DrawIndicatorForElement(selectedElement);
        }
    }

    #endregion

    #region IKAPInputReceiver

    public void FocusNextElement()
    {
        if (selectedElementIndex + 1 < accessibilityElements.Length)
        {
            UpdateSelectedElementIndex(selectedElementIndex + 1);
            PlayFocusSound();
            AnnouceElementAtSelectedIndex(true);
        }
        else
        {
            PlayBlockingSound();
            AnnouceElementAtSelectedIndex(false);
        }
    }

    public void FocusPreviousElement()
    {
        if (selectedElementIndex > 0)
        {
            UpdateSelectedElementIndex(selectedElementIndex - 1);
            PlayFocusSound();
            AnnouceElementAtSelectedIndex(true);
        }
        else
        {
            PlayBlockingSound();
            AnnouceElementAtSelectedIndex(false);
        }
    }

    public void SelectFocusedElement()
    {
        KAPElement selectedElement = this.SelectedElement();
        if (selectedElement)
        {
            selectedElement.InvokeSelection();
            PlaySelectSound();
            AnnouceElementAtSelectedIndex(true);
        }
    }

    public void IncrementValueOfFocuedElement()
    {
        KAPElement selectedElement = this.SelectedElement();
        if (selectedElement)
        {
            string oldValue = selectedElement.value;
            selectedElement.InvokeIncrement();
            string newValue = selectedElement.value;

            if (oldValue == newValue)
            {
                PlayBlockingSound();
            }
            AnnouceElementAtSelectedIndex(true);
        }
    }

    public void DecrementValueOfFocuedElement()
    {
        KAPElement selectedElement = this.SelectedElement();
        if (selectedElement)
        {
            string oldValue = selectedElement.value;
            selectedElement.InvokeDecrement();
            string newValue = selectedElement.value;

            if (oldValue == newValue)
            {
                PlayBlockingSound();
            }
            AnnouceElementAtSelectedIndex(true);
        }
    }

    public void FocusElementAtPosition(Vector2 position)
    {
        int touchedElementIndex = IndexForTopElementAtPosition(position);

        if (touchedElementIndex != -1 && selectedElementIndex != touchedElementIndex)
        {
            UpdateSelectedElementIndex(touchedElementIndex);
            PlayFocusSound();
            AnnouceElementAtSelectedIndex(true);
        }
    }

    private int IndexForTopElementAtPosition(Vector2 position)
    {
        int index = -1;

        for(int i = 0; i < accessibilityElements.Length; i++) 
        {
            KAPElement element = accessibilityElements[i];
            if(element.frame.Contains(position))
            {
                if(index == -1)
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
            AnnouceElementAtSelectedIndex(true);
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
            AnnouceElementAtSelectedIndex(true);
        }
    }

    /// Should be called when a popover appears,level changes (but scene stays), ...
    public void VisibleElementsDidChange() 
    {
        SetNeedsUpdateElements(false);
    }

    public void SetNeedsUpdateElements(bool keepHighlightedElement = true)
    {
        needsUpdateElements = true;
        retainSelectedElementIndex = keepHighlightedElement;
    }

    #endregion

    #region Public Helpers (Native screenreader available)

    public void InvokeSelectionSilentlyOfElementWithID(int instanceID)
    {
        foreach (KAPElement element in accessibilityElements) 
        {
            if(element.gameObject.GetInstanceID() == instanceID)
            {
                element.InvokeSelection();
                break;    
            }
        }
    }

    /// Invokes the value change silently of element with identifier.
    /// <param name="instanceID">Instance identifier.</param>
    /// <param name="modifier">1 = Increment, -1 = decrement</param>
    public void InvokeValueChangeSilentlyOfElementWithID(int instanceID, int modifier)
    {
        foreach (KAPElement element in accessibilityElements)
        {
            if (element.gameObject.GetInstanceID() == instanceID)
            {
                // TOOD: Maybe move this to the bridge
                if(modifier == -1) 
                {
                    element.InvokeDecrement();
                }
                else if(modifier == 1)
                {
                    element.InvokeIncrement();
                }

                break;
            }
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

    private void Update()
    {
        if(needsUpdateElements)
        {
            LoadAccessibilityElements();
            needsUpdateElements = false;

            if (!retainSelectedElementIndex || selectedElementIndex >= accessibilityElements.Length)
            {
                selectedElementIndex = -1;

                // Annouce the very first one.
                if (accessibilityElements != null && accessibilityElements.Length > 0)
                {
                    UpdateSelectedElementIndex(0);
                    AnnouceElementAtSelectedIndex(true);
                }

                retainSelectedElementIndex = true;
            }
        }
    }
}
