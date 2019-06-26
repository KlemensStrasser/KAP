﻿using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[AddComponentMenu("KAP/UI/KAPToggle")]
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

    override protected KAPTrait defaultTraits
    {
        get
        {
            return KAPTrait.Toggle;
        }
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

        // TODO: This is not the right place for the update call because its only needed when we have a native plugin. But not sure where the right place is.
        KAPUIManager.Instance.SetNeedsUpdateElements();
    }
}