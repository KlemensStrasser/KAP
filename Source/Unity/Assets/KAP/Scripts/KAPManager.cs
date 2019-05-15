using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// TODO: Make this a singleton?
public class KAPManager : MonoBehaviour, IKAPInputReceiver
{
    private int selectedElementIndex;
    private KAPSpeechSynthesizer speechSynthesizer;
    private KAPVisualizer kapVisualizer;
    private KAPNativeScreenReaderBridge nativeScreenReaderBridge;

    private AudioSource soundEffectAudioSource;

    private AudioClip focusAudioClip;
    private AudioClip blockAudioClip;
    private AudioClip selectAudioClip;

    KAPInput input;
    KAPElement[] accessibilityElements;

    // Boolean indicating if the elements need to be updated
    private bool needsUpdateElements;

    // Singleton
    // Based on: https://gamedev.stackexchange.com/questions/116009/in-unity-how-do-i-correctly-implement-the-singleton-pattern
    private static KAPManager _instance;
    public static KAPManager Instance { get { return _instance; } }

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
        GameObject visualizerObject = Resources.Load<GameObject>("Prefabs/KAPVisualizer");
        visualizerObject = Instantiate<GameObject>(visualizerObject);
        visualizerObject.name = "KAPVisualizer";
        if(visualizerObject != null) 
        {
            kapVisualizer = visualizerObject.GetComponent<KAPVisualizer>();
        }

        nativeScreenReaderBridge = new KAPNativeScreenReaderBridge();

        if (nativeScreenReaderBridge.Available())
        {
            // TODO: If a native screen reader is available, we shouldn't create our own stuff. For testing reasons, we still do
        }

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

    void LoadAccessibilityElements()
    {
        accessibilityElements = FindObjectsOfType<KAPElement>();

        // TODO: This should do more in the future, like:
        //       - Respect shouldGroupAccessibilityChildren
        //       - Handle Popovers

        if(accessibilityElements != null) 
        {
            SortByFrame();
        }

        if (nativeScreenReaderBridge.Available())
        {
            nativeScreenReaderBridge.UpdateHooksForKAPElements(accessibilityElements);
        } 
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
                else
                {
                    // TODO: If both elments Overlap, get the one that is in front!
                }
            }
        }

        return index;
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

    /// Should be called when a popover appears,
    /// level changes (but scene stays), ...
    public void VisibleElementsDidChange() 
    {
        SetNeedsUpdateElements();
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

    #endregion

    #region Public Helpers (Overall)

    // TODO: Maybe move into notifications section
    public void SetNeedsUpdateElements() 
    {
        needsUpdateElements = true;
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
        }
    }
}
