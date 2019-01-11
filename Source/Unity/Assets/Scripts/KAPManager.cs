using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KAPManager : MonoBehaviour, IKAPInputReceiver
{
    private uint selectedElementIndex;
    private KAPSpeechSynthesizer speechSynthesizer;

    KAPInput input;
    KAPElement[] accessibilityElements;
    // TODO: No Next/Previous Element sound

    // Use this for initialization
    void Start()
    {
        // TODO: Add the correct input for the current device
        input = gameObject.AddComponent<KAPKeyboardInput>();
        input.inputReceiver = this;

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
        Array.Sort(accessibilityElements, delegate(KAPElement element1, KAPElement element2) {
            int comparrisonResult = element2.frame.y.CompareTo(element1.frame.y);
            if (comparrisonResult == 0)
            {
                comparrisonResult = element1.frame.x.CompareTo(element2.frame.x);
            }
            return comparrisonResult;
        });

        foreach(KAPElement element in accessibilityElements) {
            Debug.Log(element.label + " Pos" + element.frame.position);
        }
    } 

    // Update is called once per frame
    void Update()
    {
        
    }

    void AnnouceElementAtSelectedIndex()
    {
        if (speechSynthesizer != null)
        {
            if (selectedElementIndex < accessibilityElements.Length)
            {
                KAPElement element = accessibilityElements[selectedElementIndex];
                //Debug.Log("Element: " + element.frame);
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

    void PlayBlockingSound() 
    {
        // TODO: Implement & Rename
        Debug.Log("Block");

        // TODO: Play annoucement after blocking sound was played
        AnnouceElementAtSelectedIndex();
    }

    #region IKAPInputReceiver

    public void FocusNextElement()
    {
        if (selectedElementIndex + 1 < accessibilityElements.Length)
        {
            selectedElementIndex += 1;
            AnnouceElementAtSelectedIndex();
        }
        else
        {
            PlayBlockingSound();
        }
    }

    public void FocusPreviousElement()
    {
        if (selectedElementIndex > 0)
        {
            selectedElementIndex -= 1;
            AnnouceElementAtSelectedIndex();
        }
        else
        {
            PlayBlockingSound();
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
}
