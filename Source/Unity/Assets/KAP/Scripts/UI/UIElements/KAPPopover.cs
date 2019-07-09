using UnityEngine;


/// <summary>
/// KAPPopover is used to temporarily limit the accessible elements to only children of the top most popover
/// </summary>
/// 
/// Multiple popovers should only appear in the same hierachy
/// So for example if you have the popovers p1, p2 and p3:
/// - p1 can have anything as parent,
/// - p2 has to have p1 or one of p1's children as parent 
/// - p3 has to have p2 or one of p2's children as parent 
/// If this rule is broken, the current active popover might not be the one the user sees on top of the screen!
[AddComponentMenu("KAP/UI/KAPPopOver")]
public class KAPPopover : KAPElement
{
    override protected KAPTrait defaultTraits
    {
        get
        {
            return KAPTrait.None;
        }
    }

    public void Reset()
    {
        isScreenReaderElement = true;
    }
}
