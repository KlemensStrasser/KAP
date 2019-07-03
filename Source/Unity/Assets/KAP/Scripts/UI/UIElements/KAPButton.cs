using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[AddComponentMenu("KAP/UI/KAPButton")]
public class KAPButton : KAPScreenReaderElement
{
    private Button button;
    private Text buttonText;

    override protected KAPTrait defaultTraits
    {
        get
        {
            return KAPTrait.Button;
        }
    }

    private void Awake()
    {
        this.button = gameObject.GetComponent<Button>();

        if (button != null)
        {
            this.buttonText = button.GetComponentInChildren<Text>();
        }

        SetupLabel();
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

    override public void InvokeSelection()
    {
        if (EventSystem.current != null)
        {
            ExecuteEvents.Execute(gameObject, new BaseEventData(EventSystem.current), ExecuteEvents.submitHandler);
        }
        else if (button != null)
        {
            button.onClick.Invoke();
        }

        if(this.onClick != null) 
        {
            this.onClick.Invoke();
        }
    }
}
