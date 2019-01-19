using UnityEngine;
using UnityEngine.Events;
using System;
using System.Linq;

/// Basic Element
/// Used for custom GameObject that should be made accessible
public class KAPElement : MonoBehaviour
{
    public string label = "";

    public virtual Rect frame
    {
        get
        {
            return ScreenRectForGameObject(this.gameObject);
        }
    }

    /// Value
    public string value = "";

    /// Detailed description of the function of the element
    public string description = "";

    /// Indication on how the accessibilityElement should be treated
    [HideInInspector]
    public KAPTrait trait = KAPTrait.None;

    // TODO: shouldGroupAccessibilityChildren

    /// Event that gets invoked when the element was focused by the manager
    public UnityEvent becomeFocusedEvent = new UnityEvent();
    /// Event that gets invoked when the element loses focus
    public UnityEvent loseFocusEvent = new UnityEvent();
    /// Indicates if the element currently has focus.
    bool isFocused;

    public UnityEvent onClick = new UnityEvent();

    public KAPElement()
    {
        this.isFocused = false;
    }

    void Start()
    {
        SetupLabel();
    }

    protected void SetupLabel()
    {
        if (label == null || label.Length == 0)
        {
            this.label = ImplicitLabelValue();
        }
    }

    /// Label value if none was set.
    /// This would be the text of a button for example.
    /// Should be overridden by the subclasses.
    protected virtual string ImplicitLabelValue() 
    {
        return "";
    }


    /// Selecting the element
    /// This could trigger button press, switch change...
    public virtual void InvokeSelection() 
    {
        if(this.onClick != null) 
        {
            onClick.Invoke();
        }
    }

    #region Focus Events and indication

    public void DidBecomeFocused()
    {
        this.isFocused = true;
        if(this.becomeFocusedEvent != null) 
        {
            becomeFocusedEvent.Invoke();
        }
    }

    public void DidLoseFocus()
    {
        this.isFocused = false;
        if (this.loseFocusEvent != null)
        {
            loseFocusEvent.Invoke();
        }
    }

    /// Indicates if the element currently has focus.
    public bool IsFocused()
    {
        return this.isFocused;
    }

    #endregion

    #region Public Helpers

    public string FullLabel()
    {
        string fullString;

        if(description != null && description.Length > 0)
        {
            string lableWithTrait = LabelWithTraitAndValue();

            if(lableWithTrait != null && lableWithTrait.Length > 0)
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

    public string LabelWithTraitAndValue()
    {
        string fullString;
        string[] strings;

        if(trait != null)
        {
            strings = (new[] { label, trait.Value, value });
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

    /// Static Helper Methods, not sure yet where to put them

    // Based on: https://answers.unity.com/questions/292031/how-to-display-a-rectangle-around-a-player.html
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
            // TODO: Size.
            Vector2 screenPosition = RectTransformUtility.WorldToScreenPoint(null, rectTransform.position);
            screenPosition.y = Screen.height - screenPosition.y;
            rect = new Rect(screenPosition.x, screenPosition.y, 20, 20);
        }
        else 
        {
            rect = new Rect(0, 0, 0, 0);
        }

        return rect;
    }
}
