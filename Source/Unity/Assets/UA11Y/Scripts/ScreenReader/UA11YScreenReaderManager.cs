using System;
using UnityEngine;
using System.Linq;

public class UA11YScreenReaderManager : MonoBehaviour
{
    private IUA11YScreenReader screenReader;

    /// <summary>
    /// Boolean indicating if the accessibility elements need to be updated
    /// </summary>
    private bool needsUpdateElements;

    /// <summary>
    /// Boolean indicating if the selectedElementIndex should be retained when elements are updated
    /// </summary>
    private bool retainSelectedElementIndex;

    private static UA11YScreenReaderManager _instance;
    /// <summary>
    /// UA11YUIManager Singleton
    /// Based on: https://gamedev.stackexchange.com/questions/116009/in-unity-how-do-i-correctly-implement-the-singleton-pattern
    /// </summary>
    public static UA11YScreenReaderManager Instance 
    { 
        get 
        {
            if (_instance == null)
            {
                GameObject instanceObject = Resources.Load<GameObject>("Prefabs/UI/UA11YScreenReaderManager");
                _instance = Instantiate<GameObject>(instanceObject).GetComponent<UA11YScreenReaderManager>();
            } 

            return _instance;
        } 
    }

    /// <summary>
    /// Makes sure that there is only one instance of the UA11YUIManager
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

        // Make sure that the transform is reset, otherwise the screenreader highlighting might be messed up
        this.transform.position = Vector3.zero;
    }

    /// <summary>
    /// Creates the screenreader and triggers fetching the accessibility elements
    /// </summary>
    void Start()
    {
        // TODO: Add settings to the screen readers like a prefered language

        if(UA11YNativeScreenReaderBridge.Available)
        {
            screenReader = new UA11YNativeScreenReader();
        }
        else
        {
            GameObject screenReaderObject = Resources.Load<GameObject>("Prefabs/UI/UA11YScreenReader");
            screenReaderObject = Instantiate<GameObject>(screenReaderObject);
            screenReaderObject.name = "UA11YScreenReader";
            screenReaderObject.gameObject.transform.SetParent(gameObject.transform);
            if (screenReaderObject != null)
            {
                screenReader = screenReaderObject.GetComponent<IUA11YScreenReader>();
            }
        }

        // Fetch Elements and update screen reader
        UA11YElement[] screenReaderElements = LoadScreenReaderElements();
        screenReader.UpdateWithScreenReaderElements(screenReaderElements);
    }

    /// <summary>
    /// Fetches the accessibility elements and filters out all elements hidden from the screen reader
    /// </summary>
    private UA11YElement[] LoadScreenReaderElements()
    {
        UA11YElement[] accessibilityElements = FindObjectsOfType<UA11YElement>();

        UA11YElement[] popovers = accessibilityElements.Where(c => c is UA11YPopover).ToArray();
        if (popovers != null && popovers.Length > 0)
        {
            accessibilityElements = GetAccessibilityElementsFromPopover((UA11YPopover)popovers[0]);
        }

        if (accessibilityElements != null)
        {
            // Sort by frame
            Array.Sort(accessibilityElements, delegate (UA11YElement element1, UA11YElement element2)
            {
                int comparrisonResult = element1.frame.y.CompareTo(element2.frame.y);
                if (comparrisonResult == 0)
                {
                    comparrisonResult = element1.frame.x.CompareTo(element2.frame.x);
                }
                return comparrisonResult;
            });
        }

        accessibilityElements = accessibilityElements.Where(e => e.traits.Contains(UA11YTrait.HideFromScreenReader) == false).ToArray();

        return accessibilityElements;
    }

    /// <summary>
    /// Gets the accessibility elements from the top popover.
    /// </summary>
    /// <returns>The accessibility elements from the top popover.</returns>
    /// <param name="popover">The popover we start with</param>
    private UA11YElement[] GetAccessibilityElementsFromPopover(UA11YPopover popover)
    {
        UA11YElement[] topAccessibilityElements;

        if (popover != null)
        {

            UA11YPopover topPopover = popover;

            // As described in UA11YPopover, popovers should only appear in the same hierachy (See UA11YPopover.cs for an example)
            // Thus, any popOver we find in the children of the topPopover should be at least one step up this hierachy
            // It could be multiple steps, depending on the order we get the popovers

            bool searchForChildPopOvers = true;
            UA11YPopover[] childPopovers;
            do
            {
                // Get all child popOvers
                childPopovers = topPopover.gameObject.GetComponentsInChildren<UA11YPopover>();
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


            topAccessibilityElements = topPopover.gameObject.GetComponentsInChildren<UA11YElement>();
            // Filter out the popover
            topAccessibilityElements = topAccessibilityElements.Where(go => go.gameObject != topPopover.gameObject).ToArray();
        } 
        else
        {
            topAccessibilityElements = new UA11YElement[0];
        }

        return topAccessibilityElements;
    }


    #region "Notifications"

    /// <summary>
    /// Stops any text that is currently spoken and annouces the given message
    /// </summary>
    public void AnnounceMessage(string message) 
    {
        screenReader.AnnounceMessage(message);
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

    /// <summary>
    /// Disables/Enables screenreder navigation, so that the game can capture the current input
    /// </summary>
    /// 
    /// This will not disable the screenreader fully, because it is also used for the annoucements
    public void SetScreenReaderEnabled(bool enabled)
    {
        if (screenReader != null)
        {
            screenReader.SetEnabled(enabled);
        }
    }

    #endregion

    /// <summary>
    /// If needsUpdateElements is set, reloads the accessibility elements and updates the screen readers
    /// </summary>
    private void Update()
    {
        if(needsUpdateElements)
        {
            UA11YElement[] screenReaderElements = LoadScreenReaderElements();
            screenReader.UpdateWithScreenReaderElements(screenReaderElements, retainSelectedElementIndex);

            needsUpdateElements = false;
            retainSelectedElementIndex = true;
        }
    }
}
