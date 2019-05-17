using UnityEngine.UI;

public class KAPText : KAPElement
{
    /// The Text that might be attached to the same GameObject as this component
    private Text text;

    override protected KAPTrait defaultTraits
    {
        get
        {
            return KAPTrait.StaticText;
        }
    }

    public KAPText() : base()
    {
    }

    private void Awake()
    {
        this.text = gameObject.GetComponent<Text>();
        SetupLabel();
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
