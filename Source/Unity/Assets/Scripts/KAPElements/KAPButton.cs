using UnityEngine.UI;

public class KAPButton : KAPElement
{
    private Button button;
    private Text buttonText;

    public KAPButton() : base()
    {
        this.trait = KAPTrait.Button;
    }

    private void Start()
    {
        this.button = gameObject.GetComponent<Button>();

        if (button != null)
        {
            this.buttonText = button.GetComponentInChildren<Text>();
        }
    }

    override protected string ImplicitLabelValue()
    {
        string implicitTextValue;
        if (buttonText != null)
        {
            implicitTextValue = buttonText.text;
        }
        else
        {
            implicitTextValue = "";
        }

        return implicitTextValue;
    }
}
