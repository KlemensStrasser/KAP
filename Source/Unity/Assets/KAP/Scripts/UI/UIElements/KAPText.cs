﻿using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("KAP/UI/KAPText")]
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
