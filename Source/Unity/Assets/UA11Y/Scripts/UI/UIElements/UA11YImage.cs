using UnityEngine;
using System.Collections.Generic;

[AddComponentMenu("UA11Y/UI/UA11YImage")]
public class UA11YImage : UA11YElement
{
    override protected List<UA11YTrait> defaultTraits
    {
        get
        {
            return new List<UA11YTrait> { UA11YTrait.Image };
        }
    }

    public void Reset()
    {
        isScreenReaderElement = true;
    }
}
