using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KAPSlider : KAPElement
{
    /// The slider that should be attached to the same gameObject
    private Slider slider;

    /// The amount of decimals we can handle for increasing/decreasing the slider
    private static int accuracy = 4;

    override protected KAPTrait defaultTraits
    {
        get
        {
            return KAPTrait.Adjustable;
        }
    }

    public override string value
    {
        get
        {
            if (slider != null)
            {
                return slider.value.ToString();
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

    public KAPSlider() : base()
    {
    }

    private void Awake()
    {
        this.slider = gameObject.GetComponent<Slider>();
    }

    /// Increment value
    override public void InvokeIncrement()
    {
        if (this.onIncrement != null)
        {
            onIncrement.Invoke();
        }

        if (slider != null)
        {
            float newValue = slider.value + (slider.maxValue - slider.minValue) * 0.1f;

            if (newValue >= slider.maxValue)
            {
                newValue = slider.maxValue;
            }

            slider.value = newValue;

            double roundedValue = System.Math.Round((double)newValue, accuracy);

            slider.value = (float)roundedValue;

            Debug.Log(newValue + " " + roundedValue);
            // TODO: This is not the right place for the update call because its only needed when we have a native plugin. But not sure where the right place is.
            KAPManager.Instance.SetNeedsUpdateElements();
        }
    }

    /// Decrement value
    override public void InvokeDecrement()
    {
        if (this.onDecrement != null)
        {
            onDecrement.Invoke();
        }

        if(slider != null)
        {
            float newValue = slider.value - (slider.maxValue - slider.minValue) * 0.1f;

            if(newValue <= slider.minValue)
            {
                newValue = slider.minValue;
            }


            double roundedValue = System.Math.Round((double)newValue, accuracy);

            Debug.Log(newValue + " " + roundedValue);

            slider.value = (float)roundedValue;

            // TODO: This is not the right place for the update call because its only needed when we have a native plugin. But not sure where the right place is.
            KAPManager.Instance.SetNeedsUpdateElements();
        }
    }
}
