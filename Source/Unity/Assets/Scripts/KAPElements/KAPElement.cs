using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    /// Detailed description of the function of the element
    public string description;

    /// Value
    public string value;

    // TODO: Think if we really need the trait
    public KAPTrait trait;

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
}
