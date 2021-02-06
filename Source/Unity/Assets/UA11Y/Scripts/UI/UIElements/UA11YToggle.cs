using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

[AddComponentMenu("UA11Y/UI/UA11YToggle")]
public class UA11YToggle : UA11YElement
{
    /// The Text that might be attached to the same GameObject as this component
    private Toggle toggle;
    private Text title;

    public override string value
    {
        get
        {
            if (toggle != null)
            {
                // TODO: Localize correctly, only works on iOS right now
                return (toggle.isOn ? "1" : "0");
            }
            else
            {
                return base.value;
            }
        }

        set
        {
            base.value = value;
        }
    }

    override protected List<UA11YTrait> defaultTraits
    {
        get
        {
            return new List<UA11YTrait> { UA11YTrait.Toggle };
        }
    }

    private void Awake()
    {
        toggle = gameObject.GetComponent<Toggle>();

        if (toggle != null)
        {
            title = toggle.GetComponentInChildren<Text>();
        }
    }

    override protected string ImplicitLabelValue()
    {
        string implicitTextValue;
        if (title != null)
        {
            implicitTextValue = title.text;
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
        else if (toggle != null)
        {
            toggle.onValueChanged.Invoke(!toggle.isOn);
        }

        if (this.onClick != null)
        {
            this.onClick.Invoke();
        }

        // TODO: This is not the right place for the update call because its only needed when we have a native plugin. But not sure where the right place is.
        UA11YScreenReaderManager.Instance.SetNeedsUpdateElements();
    }
}
