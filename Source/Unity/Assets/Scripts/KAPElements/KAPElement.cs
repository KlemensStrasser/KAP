using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

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
    private bool isFocused;

    public KAPElement() 
    {
        this._label = null;

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
        // TODO: Search trough the gameObject if there is a gameObject that receives a trigger call
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
    protected static Rect ScreenRectForRectTransform(RectTransform rectTransform) 
    {
        Vector3 screenPosition = Camera.main.WorldToScreenPoint(rectTransform.position);
        // TODO: Height & Width
        return new Rect(screenPosition.x, screenPosition.y, 0, 0);
    }
}
