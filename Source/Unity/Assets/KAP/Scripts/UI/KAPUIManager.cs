using System;
using UnityEngine;
using System.Linq;

public class KAPUIManager : MonoBehaviour
{
    private IKAPScreenReader screenReader;

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

    /// <summary>
    /// Makes sure that there is only one instance of the KAPUIManager
    /// </summary>
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

    /// <summary>
    /// Creates the screenreader and triggers fetching the accessibility elements
    /// </summary>
    void Start()
    {
        // TODO: Add settings to the screen readers like a prefered language
        KAPNativeScreenReaderBridge nativeScreenReaderBridge = new KAPNativeScreenReaderBridge();

        if(nativeScreenReaderBridge.Available())
        {
            screenReader = nativeScreenReaderBridge;
        }
        else
        {
            GameObject screenReaderObject = Resources.Load<GameObject>("Prefabs/UI/KAPScreenReader");
            screenReaderObject = Instantiate<GameObject>(screenReaderObject);
            screenReaderObject.name = "KAPScreenReader";
            if (screenReaderObject != null)
            {
                screenReader = screenReaderObject.GetComponent<IKAPScreenReader>();
            }
        }

        // Fetch Elements
        KAPScreenReaderElement[] accessibilityElements = LoadAccessibilityElements();

        screenReader.UpdateWithScreenReaderElements(accessibilityElements);
    }

    /// <summary>
    /// Fetches the accessibility elements in the scene and sorts them by frame
    /// </summary>
    KAPScreenReaderElement[] LoadAccessibilityElements()
    {
        KAPScreenReaderElement[] elements = FindObjectsOfType<KAPScreenReaderElement>();

        KAPScreenReaderElement[] popovers = elements.Where(c => c is KAPPopover).ToArray();
        if(popovers != null && popovers.Length > 0)
        {
            elements = GetAccessibilityElementsFromPopover((KAPPopover)popovers[0]);
        }

        if (elements != null) 
        {
            // Sort by frame
            Array.Sort(elements, delegate (KAPScreenReaderElement element1, KAPScreenReaderElement element2)
            {
                int comparrisonResult = element1.frame.y.CompareTo(element2.frame.y);
                if (comparrisonResult == 0)
                {
                    comparrisonResult = element1.frame.x.CompareTo(element2.frame.x);
                }
                return comparrisonResult;
            });
        }

        return elements;
    }


    /// <summary>
    /// Gets the accessibility elements from the top popover.
    /// </summary>
    /// <returns>The accessibility elements from the top popover.</returns>
    /// <param name="popover">The popover we start with</param>
    KAPScreenReaderElement[] GetAccessibilityElementsFromPopover(KAPPopover popover)
    {
        KAPScreenReaderElement[] topAccessibilityElements;

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


            topAccessibilityElements = topPopover.gameObject.GetComponentsInChildren<KAPScreenReaderElement>();
            // Filter out the popover
            topAccessibilityElements = topAccessibilityElements.Where(go => go.gameObject != topPopover.gameObject).ToArray();
        } 
        else
        {
            topAccessibilityElements = new KAPScreenReaderElement[0];
        }

        return topAccessibilityElements;
    }


    #region "Notifications"

    /// <summary>
    /// Stops any text that is currently spoken and annouces the given message
    /// </summary>
    public void AnnouceMessage(string message) 
    {
        screenReader.AnnounceMessage(message);
    }

    /// <summary>
    /// Tries to set the focus on a given KAPScreenReaderElement
    /// </summary>
    public void FocusElement(KAPScreenReaderElement element) 
    {
        screenReader.FocusElement(element);
    }

    /// <summary>
    /// Tries to set the focus on a KAPScreenReaderElement that is attached to the given GameObject
    /// </summary>
    public void FocusGameObject(GameObject gObject)
    {
        KAPScreenReaderElement element = gObject.GetComponent<KAPScreenReaderElement>();

        if (element != null)
        {
            this.FocusElement(element);
        }
        else
        {
            Debug.LogWarning("KAPUIManager: Given GameObject has no KAPScreenReaderElement attached!");
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

    /// <summary>
    /// If needsUpdateElements is set, reloads the accessibility elements and updates the screen readers
    /// </summary>
    private void Update()
    {
        if(needsUpdateElements)
        {
            KAPScreenReaderElement[] accessibilityElements = LoadAccessibilityElements();
            screenReader.UpdateWithScreenReaderElements(accessibilityElements, retainSelectedElementIndex);

            needsUpdateElements = false;
            retainSelectedElementIndex = true;
        }
    }
}
