using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    // TODO: Think if we really need the trait
    public KAPTrait trait;

    // TODO: shouldGroupAccessibilityChildren
    // TODO: Implement UIAccessibilityFocus Protocol https://developer.apple.com/documentation/uikit/accessibility/uiaccessibilityfocus


    protected bool isFocused;

    public KAPElement() 
    {
        this._label = null;

        this.label = null;
        this.description = "";
        this.value = "";
        this.trait = KAPTrait.None;
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
    /// If
    public virtual void InvokeSelection() 
    {
        // TODO: Search trough the gameObject if there is a gameObject that receives a trigger call
    }


    /// Static Helper Methods, not sure yet where to put them
    protected static Rect ScreenRectForRectTransform(RectTransform rectTransform) 
    {
        Vector3 screenPosition = Camera.main.WorldToScreenPoint(rectTransform.position);
        // TODO: Height & Width
        return new Rect(screenPosition.x, screenPosition.y, 0, 0);
    }
}
