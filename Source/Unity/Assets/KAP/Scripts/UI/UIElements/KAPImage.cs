using UnityEngine;

[AddComponentMenu("KAP/UI/KAPImage")]
public class KAPImage : KAPScreenReaderElement
{
    override protected KAPTrait defaultTraits
    {
        get
        {
            return KAPTrait.Image;
        }
    }
}
