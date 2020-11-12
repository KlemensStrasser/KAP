using UnityEngine;
using UnityEngine.Events;
using System;
using System.Collections.Generic;
using System.Linq;

public enum UA11YElementInteractionEventType
{
    Click,
    Increment,
    Decrement,
    BecomeFocused,
    LoseFocus
}

/// Basic Element
/// Used for custom GameObject that should be made accessible
[AddComponentMenu("UA11Y/UA11YScreenReaderElement")]
public class UA11YElement : MonoBehaviour
{
    /// <summary>
    /// A short but concise label of the element
    /// </summary>
    public string label = "";

    /// <summary>
    /// Value of the element
    /// </summary>
    public virtual string value { get; set; }

    /// <summary>
    /// Detailed description of the function of the element.
    /// Only used if the functioning isn't obvious through label, value and traits.
    /// </summary>
    public string description = "";

    /// <summary>
    /// Default indication on how the accessibilityElement should be treated by the screen reader.
    /// Set but the subclasses
    /// </summary>
    protected virtual List<UA11YTrait> defaultTraits
    {
        get
        {
            return new List<UA11YTrait>();
        }
    }

    private List<UA11YTrait> _traits;

    /// <summary>
    /// Indication on how the element should be treated by the accessibility technology
    /// Returns the defaultTraits if not explicitily set
    /// Only set for standard elements if you know what you're doing.
    /// </summary>
    [HideInInspector]
    public List<UA11YTrait> traits
    { 
        get 
        { 
            if(_traits != null) 
            {
                return _traits;
            }
            else
            {
                return defaultTraits;
            }
        }

        set
        {
            _traits = value;
        }
    }

    /// <summary>
    /// Standardized frame of the element in screen coordinates.
    /// </summary>
    public virtual Rect frame
    {
        get
        {
            return ScreenRectForGameObject(this.gameObject);
        }
    }

    /// <summary>
    /// Indicates if it can be seen by the screen reader or not.
    /// </summary>
    public bool isScreenReaderElement;

    /// <summary>
    /// Event that gets invoked when the element was selected
    /// </summary>
    protected UnityEvent onClick = new UnityEvent();

    /// <summary>
    /// Event that gets invoked when the value of the element is incremented
    /// </summary>
    protected UnityEvent onIncrement = new UnityEvent();

    /// <summary>
    /// Event that gets invoked when the value of the element is decremented
    /// </summary>
    protected UnityEvent onDecrement = new UnityEvent();

    /// <summary>
    /// Event that gets invoked when the element was focused by the manager
    /// </summary>
    protected UnityEvent onBecomeFocused = new UnityEvent();

    /// <summary>
    /// Event that gets invoked when the element loses focus
    /// </summary>
    protected UnityEvent onLoseFocus = new UnityEvent();

    void Awake()
    {
        SetupLabel();
    }

    /// <summary>
    /// Setup the label 
    /// </summary>
    protected void SetupLabel()
    {
        if (label == null || label.Length == 0)
        {
            this.label = ImplicitLabelValue();
        }
    }

    /// <summary>
    /// Label value if none was explicitly set. This would be the text of a button for example.
    /// Should be overridden by the subclasses.
    /// </summary>
    protected virtual string ImplicitLabelValue()
    {
        return "";
    }

    #region Subscribing or Unsubscribe to Events

    public void AddListenerForEventOfType(UA11YElementInteractionEventType eventType, UnityAction action)
    {
        UnityEvent unityEvent = EventForType(eventType);

        if (unityEvent != null)
        {
            unityEvent.AddListener(action);
        }
    }

    public void RemoveListenerForEventOfType(UA11YElementInteractionEventType eventType, UnityAction action)
    {
        UnityEvent unityEvent = EventForType(eventType);

        if (unityEvent != null)
        {
            unityEvent.RemoveListener(action);
        }
    }

    public void RemoveAllListenersForEventOfType(UA11YElementInteractionEventType eventType)
    {
        UnityEvent unityEvent = EventForType(eventType);

        if (unityEvent != null)
        {
            unityEvent.RemoveAllListeners();
        }
    }   

    #endregion

    #region Triggering Events

    /// <summary>
    /// Invokes the given type of event
    /// <param name="eventType">Type of event that should be invoked</param>
    /// </summary>
    public void InvokeEventOfType(UA11YElementInteractionEventType eventType)
    {
        switch (eventType)
        {
            case UA11YElementInteractionEventType.Click:
                InvokeSelection();
                break;
            case UA11YElementInteractionEventType.Increment:
                InvokeIncrement();
                break;
            case UA11YElementInteractionEventType.Decrement:
                InvokeDecrement();
                break;
            case UA11YElementInteractionEventType.BecomeFocused:
                InvokeBecomeFocused();
                break;
            case UA11YElementInteractionEventType.LoseFocus:
                InvokeLoseFocus();
                break;
        }
    }

    /// <summary>
    /// Invokes selecting the element (Button Click, toggle change...)
    /// </summary>
    protected virtual void InvokeSelection()
    {
        onClick.Invoke();
    }

    /// <summary>
    /// Invokes incrementing the value of the element
    /// </summary>
    protected virtual void InvokeIncrement()
    {
        onIncrement.Invoke();
    }

    /// <summary>
    /// Invokes decrementing the value of the element
    /// </summary>
    protected virtual void InvokeDecrement()
    {
        onDecrement.Invoke();
    }

    /// <summary>
    /// Called by the screenreader if this element gets focused
    /// </summary>
    protected void InvokeBecomeFocused()
    {
        onBecomeFocused.Invoke();
    }

    /// <summary>
    /// Called by the screenreader if this element is focused right now, but the focus will move to another element
    /// </summary>
    protected void InvokeLoseFocus()
    {
        onLoseFocus.Invoke();
    }

    #endregion

    #region Public Helpers (No native screenreader available)

    /// <summary>
    /// Composes a string from the label, trait and description of the element
    /// </summary>
    /// <returns>The composed string</returns>
    public string FullLabel()
    {
        string fullString;

        if (description != null && description.Length > 0)
        {
            string lableWithTrait = LabelWithTraitAndValue();

            if (lableWithTrait != null && lableWithTrait.Length > 0)
            {
                fullString = LabelWithTraitAndValue() + ". " + description;
            }
            else
            {
                fullString = description;
            }
        }
        else
        {
            fullString = LabelWithTraitAndValue();
        }

        if (fullString == null)
        {
            fullString = "";
        }

        return fullString;
    }

    /// <summary>
    /// Composes a string from the label and the element
    /// </summary>
    /// <returns>The composed string</returns>
    public string LabelWithTraitAndValue()
    {
        string fullString;
        string[] strings;

        if (traits.Count > 0)
        {
            strings = (new[] { label, traitsValue(), value });
        }
        else
        {
            strings = (new[] { label, value });
        }

        fullString = String.Join(". ", strings.Where(str => str != null && str.Length > 0).ToArray());

        if (fullString == null)
        {
            fullString = "";
        }

        return fullString;
    }

    #endregion

    /// <summary>
    /// Static helper method to calculate the frame of gameObject
    /// </summary>
    /// <returns>The frame for gameObject.</returns>
    /// <param name="gObject">GameObject for which to calculate the screen rect</param>
    /// Based on: https://answers.unity.com/questions/292031/how-to-display-a-rectangle-around-a-player.html
    /// Not sure where to put this code
    protected static Rect ScreenRectForGameObject(GameObject gObject) 
    {
        Rect rect;
        Renderer renderer = gObject.GetComponent<Renderer>();
        RectTransform rectTransform = gObject.GetComponent<RectTransform>();
        Camera mainCamera = Camera.main;

        if(renderer != null && mainCamera != null)
        {
            Bounds bounds = renderer.bounds;

            // Check if the object is behind the camera and thus, not visible
            if (mainCamera.WorldToScreenPoint(bounds.center).z < 0) {};
            Vector3[] cornerPoints = new Vector3[8];

            if(renderer is SpriteRenderer)
            {
                cornerPoints = new Vector3[4];

                cornerPoints[0] = mainCamera.WorldToScreenPoint(new Vector3(bounds.center.x + bounds.extents.x, bounds.center.y + bounds.extents.y, bounds.center.z));
                cornerPoints[1] = mainCamera.WorldToScreenPoint(new Vector3(bounds.center.x + bounds.extents.x, bounds.center.y - bounds.extents.y, bounds.center.z));
                cornerPoints[2] = mainCamera.WorldToScreenPoint(new Vector3(bounds.center.x - bounds.extents.x, bounds.center.y + bounds.extents.y, bounds.center.z));
                cornerPoints[3] = mainCamera.WorldToScreenPoint(new Vector3(bounds.center.x - bounds.extents.x, bounds.center.y - bounds.extents.y, bounds.center.z));
            }
            else 
            {
                cornerPoints = new Vector3[8];
                cornerPoints[0] = mainCamera.WorldToScreenPoint(new Vector3(bounds.center.x + bounds.extents.x, bounds.center.y + bounds.extents.y, bounds.center.z + bounds.extents.z));
                cornerPoints[1] = mainCamera.WorldToScreenPoint(new Vector3(bounds.center.x + bounds.extents.x, bounds.center.y + bounds.extents.y, bounds.center.z - bounds.extents.z));
                cornerPoints[2] = mainCamera.WorldToScreenPoint(new Vector3(bounds.center.x + bounds.extents.x, bounds.center.y - bounds.extents.y, bounds.center.z + bounds.extents.z));
                cornerPoints[3] = mainCamera.WorldToScreenPoint(new Vector3(bounds.center.x + bounds.extents.x, bounds.center.y - bounds.extents.y, bounds.center.z - bounds.extents.z));
                cornerPoints[4] = mainCamera.WorldToScreenPoint(new Vector3(bounds.center.x - bounds.extents.x, bounds.center.y + bounds.extents.y, bounds.center.z + bounds.extents.z));
                cornerPoints[5] = mainCamera.WorldToScreenPoint(new Vector3(bounds.center.x - bounds.extents.x, bounds.center.y + bounds.extents.y, bounds.center.z - bounds.extents.z));
                cornerPoints[6] = mainCamera.WorldToScreenPoint(new Vector3(bounds.center.x - bounds.extents.x, bounds.center.y - bounds.extents.y, bounds.center.z + bounds.extents.z));
                cornerPoints[7] = mainCamera.WorldToScreenPoint(new Vector3(bounds.center.x - bounds.extents.x, bounds.center.y - bounds.extents.y, bounds.center.z - bounds.extents.z));
            }

            // Calculate real y position in GUI space
            for (int i = 0; i < cornerPoints.Length; i++)
            {
                cornerPoints[i].y = Screen.height - cornerPoints[i].y;
            }

            Vector3 minPoint = cornerPoints[0];
            Vector3 maxPoint = cornerPoints[0];
            for (int i = 1; i < cornerPoints.Length; i++)
            {
                minPoint = Vector3.Min(minPoint, cornerPoints[i]);
                maxPoint = Vector3.Max(maxPoint, cornerPoints[i]);
            }

            rect = Rect.MinMaxRect(minPoint.x, minPoint.y, maxPoint.x, maxPoint.y);
        }
        else if(rectTransform != null)
        {
            Vector2 screenPosition = RectTransformUtility.WorldToScreenPoint(null, rectTransform.position);
            screenPosition.y = Screen.height - screenPosition.y - rectTransform.rect.height / 2;
            screenPosition.x = screenPosition.x - rectTransform.rect.width/ 2;
            rect = new Rect(screenPosition.x, screenPosition.y, rectTransform.rect.width, rectTransform.rect.height);
        }
        else 
        {
            rect = new Rect(0, 0, 0, 0);
        }

        return rect;
    }

    private string traitsValue()
    {
        string traitsValue = "";
        foreach (UA11YTrait trait in traits)
        {
            if (traitsValue.Equals(""))
            {
                traitsValue = trait.ToString();
            }
            else
            {
                traitsValue += ", " + trait.ToString();
            }
        }

        return traitsValue;
    }

    private UnityEvent EventForType(UA11YElementInteractionEventType eventType)
    {
        UnityEvent eventForType = null;

        switch (eventType)
        {
            case UA11YElementInteractionEventType.Click:
                eventForType = onClick;
                break;
            case UA11YElementInteractionEventType.Increment:
                eventForType = onIncrement;
                break;
            case UA11YElementInteractionEventType.Decrement:
                eventForType = onDecrement;
                break;
            case UA11YElementInteractionEventType.BecomeFocused:
                eventForType = onBecomeFocused;
                break;
            case UA11YElementInteractionEventType.LoseFocus:
                eventForType = onLoseFocus;
                break;
        }

        return eventForType;
    }

}
