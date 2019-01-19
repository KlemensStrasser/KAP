using UnityEngine;
using UnityEngine.Events;
using System;
using System.Linq;

/// Basic Element
/// Used for custom GameObject that should be made accessible
public class KAPElement : MonoBehaviour
{
    private string _label;
    public string label 
    { 
        get 
        {
            if(_label != null) 
            {
                return _label;
            } 
            else 
            {
                return ImplicitLabelValue();
            }
        } 
        set 
        {
            _label = value;
        }
    }

    public virtual Rect frame
    {
        get
        {
            RectTransform rectTransform = gameObject.GetComponent<RectTransform>();

            if (rectTransform != null) 
            {
                return ScreenRectForRectTransform(rectTransform);   
            } 
            else 
            {
                return new Rect(0, 0, 0, 0);   
            }
        }
    }

    /// Detailed description of the function of the element
    public string description;

    /// Value
    public string value;

    /// Indication on how the accessibilityElement should be treated
    public KAPTrait trait;

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
        this._label = null;
        this.isFocused = false;

        this.label = null;
        this.description = "";
        this.value = "";
        this.trait = KAPTrait.None;

        this.isFocused = false;
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

    public string LabelWithTrait()
    {
        string labelWithTrait;
        if(this.trait != null && this.trait.Value != null && this.trait.Value != "") 
        {
            labelWithTrait = label + ". " + trait.Value;
        } 
        else 
        {
            labelWithTrait = label;
        }

        return labelWithTrait;
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
