using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

[AddComponentMenu("UA11Y/UI/UA11YButton")]
public class UA11YButton : UA11YElement
{
    private Button button;
    private Text buttonText;

    override protected List<UA11YTrait> defaultTraits
    {
        get
        {
            return new List<UA11YTrait> { UA11YTrait.Button };
        }
    }

    private void Awake()
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

    override protected void InvokeSelection()
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
