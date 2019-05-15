using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class KAPToggle : KAPElement
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

    public KAPToggle() : base()
    {
        this.trait = KAPTrait.Toggle;
    }

    private void Awake()
    {
        this.toggle = gameObject.GetComponent<Toggle>();

        if (toggle != null)
        {
            this.title = toggle.GetComponentInChildren<Text>();
        }

        SetupLabel();
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

    override public void InvokeSelection()
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

        KAPManager.Instance.SetNeedsUpdateElements();
    }
}
