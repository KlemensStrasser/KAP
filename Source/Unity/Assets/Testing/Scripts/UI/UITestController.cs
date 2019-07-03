using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UITestController : MonoBehaviour {

    public Button alertButton;

    public GameObject mainMenu;
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
        }

        if (mainMenu != null)
        {
            mainMenu.SetActive(true);
        }
        KAPUIManager.Instance.VisibleElementsDidChange();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if (mainMenu != null)
            {
                mainMenu.SetActive(!mainMenu.activeSelf);

                if (popover != null)
                {
                    popover.SetActive(false);
                }

                KAPUIManager.Instance.VisibleElementsDidChange();
            }
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
