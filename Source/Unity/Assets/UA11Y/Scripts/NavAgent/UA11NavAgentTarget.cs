using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UA11NavAgentTarget : UA11YElement
{
    override protected List<UA11YTrait> defaultTraits
    {
        get
        {
            return new List<UA11YTrait> { UA11YTrait.HideFromScreenReader };
        }
    }
}
