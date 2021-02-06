using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

[AddComponentMenu("UA11Y/UI/UA11YText")]
public class UA11YText : UA11YElement
{
    /// The Text that might be attached to the same GameObject as this component
    private Text text;

    override protected List<UA11YTrait> defaultTraits
    {
        get
        {
            return new List<UA11YTrait> { UA11YTrait.StaticText };
        }
    }

    private void Awake()
    {
        this.text = gameObject.GetComponent<Text>();
    }

    override protected string ImplicitLabelValue()
    {
        string implicitTextValue;
        if (text != null)
        {
            implicitTextValue = text.text;
        }
        else 
        {
            implicitTextValue = "";
        }

        return implicitTextValue;
    }
}
