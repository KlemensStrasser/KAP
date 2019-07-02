using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UITestController : MonoBehaviour {

    public Button alertButton;

    public GameObject popover;

	void Awake ()
    {
        if (alertButton != null)
        {
            alertButton.onClick.AddListener(ShowPopover);
        }

        if (popover != null)
        {
            popover.SetActive(false);
            KAPUIManager.Instance.VisibleElementsDidChange();
        }
    }

    void ShowPopover() 
    {
        if (popover != null)
        {
            popover.SetActive(true);
            KAPUIManager.Instance.VisibleElementsDidChange();
        }
    }
}
