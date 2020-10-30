using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("UA11Y/UI/UA11YText")]
public class UA11YText : UA11YElement
{
    /// The Text that might be attached to the same GameObject as this component
    private Text text;

    override protected UA11YTrait defaultTraits
    {
        get
        {
            return UA11YTrait.StaticText;
        }
    }

    private void Awake()
    {
        this.text = gameObject.GetComponent<Text>();
        SetupLabel();
    }

    public void Reset()
    {
        isScreenReaderElement = true;
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
