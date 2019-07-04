using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPickupCallbackObject
{ 
    bool CanBePickedUpByCollider(Collider other, int pickupID);
}

public class KAPSonarTestPickupController : MonoBehaviour
{
    private bool pickedUp = false;

    private int pickupID = -1;

    [HideInInspector]
    public IPickupCallbackObject callbackObject;

    public void AssignPickupID(int id)
    {
        pickupID = id;
    }

    public int PickupID()
    {
        return pickupID;
    }

    public bool WasPickedUp()
    {
        return pickedUp;
    }

    public void SetPickedUp()
    {
        pickedUp = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(callbackObject != null && callbackObject.CanBePickedUpByCollider(other, pickupID))
        {
            SetPickedUp();
            gameObject.SetActive(false);
        }
    }
}
