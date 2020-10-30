using UnityEngine;

[AddComponentMenu("UA11Y/UI/UA11YImage")]
public class UA11YImage : UA11YElement
{
    override protected UA11YTrait defaultTraits
    {
        get
        {
            return UA11YTrait.Image;
        }
    }

    public void Reset()
    {
        isScreenReaderElement = true;
    }
}
