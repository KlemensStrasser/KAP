using UnityEngine;

public abstract class KAPInput : MonoBehaviour
{
    // TODO: Check if Circular reference needs to be prevented (between Input and InputReceiver)!
    public IKAPInputReceiver inputReceiver;

    public abstract string GetStatusText();
}