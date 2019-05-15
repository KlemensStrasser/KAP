using UnityEngine.UI;

public class KAPText : KAPElement
{
    /// The Text that might be attached to the same GameObject as this component
    private Text text;

    public KAPText() : base()
    {
        this.trait = KAPTrait.StaticText;
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
