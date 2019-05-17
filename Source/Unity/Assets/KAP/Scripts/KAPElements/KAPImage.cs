using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KAPImage : KAPElement
{
    override protected KAPTrait defaultTraits
    {
        get
        {
            return KAPTrait.Image;
        }
    }

    public KAPImage() : base()
    {
    }
}
