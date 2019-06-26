using UnityEngine;

[AddComponentMenu("KAP/UI/KAPImage")]
public class KAPImage : KAPElement
{
    override protected KAPTrait defaultTraits
    {
        get
        {
            return KAPTrait.Image;
        }
    }
}
