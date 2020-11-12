using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UITestController : MonoBehaviour {

    public Button popupButton;
    public Button alertButton;

    public GameObject mainMenu;
    public GameObject popover;


	void Awake ()
    {
        if (popupButton != null)
        {
            popupButton.onClick.AddListener(ShowPopover);
        }

        if(alertButton != null)
        {
            alertButton.onClick.AddListener(SpeakAlert);
        }

        if (popover != null)
        {
            popover.SetActive(false);
        }

        if (mainMenu != null)
        {
            mainMenu.SetActive(true);
        }
        UA11YScreenReaderManager.Instance.VisibleElementsDidChange();
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

                UA11YScreenReaderManager.Instance.VisibleElementsDidChange();
            }
        }
    }

    void ShowPopover() 
    {
        if (popover != null)
        {
            popover.SetActive(true);
            UA11YScreenReaderManager.Instance.VisibleElementsDidChange();
        }
    }

    void SpeakAlert()
    {
        UA11YScreenReaderManager.Instance.AnnounceMessage("This an important announcement");
    }
}
