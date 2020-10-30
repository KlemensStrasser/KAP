using System;
using UnityEngine;
using UnityEngine.UI;

public class UA11YSonarTestMenuController : MonoBehaviour, IPickupCallbackObject
{
    [Header("UI")]
    public GameObject mainMenu;
    public GameObject winningMenu;

    // TODO: (If I got some time) dynamically create these buttons for every pickup in the scene
    public Button buttonPickupA;
    public Button buttonPickupB;
    public Button buttonPickupC;

    [Header("Pickups")]
    public UA11YSonarTestPickupController pickupA;
    public UA11YSonarTestPickupController pickupB;
    public UA11YSonarTestPickupController pickupC;

    [Header("Player")]
    public UA11YSonarTestPlayerController playerController;

    private int currentTargetPickupID = -1;

    private void Awake()
    {
        if (buttonPickupA != null)
        {
            buttonPickupA.onClick.AddListener(ButtonPickupAClicked);
        }

        if (buttonPickupB != null)
        {
            buttonPickupB.onClick.AddListener(ButtonPickupBClicked);
        }

        if (buttonPickupC != null)
        {
            buttonPickupC.onClick.AddListener(ButtonPickupCClicked);
        }

        if(pickupA != null)
        {
            pickupA.callbackObject = this;
            pickupA.AssignPickupID(0);

            currentTargetPickupID = pickupA.PickupID();
        }

        if (pickupB != null)
        {
            pickupB.callbackObject = this;
            pickupB.AssignPickupID(pickupA.PickupID() + 1);
        }

        if (pickupC != null)
        {
            pickupC.callbackObject = this;
            pickupC.AssignPickupID(pickupB.PickupID() + 1);
        }
    }

    private void Start()
    {
        if (mainMenu != null)
        {
            mainMenu.SetActive(false);
        }

        if (winningMenu != null)
        {
            winningMenu.SetActive(false);
        }

        UA11YUIManager.Instance.SetScreenReaderEnabled(false);
    }

    void Update()
    {
        if(winningMenu.activeSelf == false)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                bool showMenu = !mainMenu.activeSelf;
                ChangeMenuVisibility(showMenu);
            }

            if (currentTargetPickupID > pickupC.PickupID())
            {
                TriggerGameFinished();
            }
        }
    }

    void TriggerGameFinished()
    {
        winningMenu.SetActive(true);
        UA11YUIManager.Instance.SetScreenReaderEnabled(true);
        UA11YUIManager.Instance.VisibleElementsDidChange();

        if (playerController != null)
        {
            playerController.movementBlocked = true;
        }

        UA11YSonarManager.Instance.gameObject.SetActive(false);
    }

    void ChangeMenuVisibility(bool showMenu)
    {
        if (mainMenu != null)
        {
            UA11YUIManager.Instance.SetScreenReaderEnabled(showMenu);
            mainMenu.SetActive(showMenu);

            if (showMenu)
            {
                UA11YUIManager.Instance.VisibleElementsDidChange();
            }

            if (playerController != null)
            {
                playerController.movementBlocked = showMenu;
            }
        }
    }

    #region Button interaction

    private void ButtonPickupAClicked()
    {
        if (pickupA != null)
        {
            UA11YSonarManager.Instance.StartGuideToTargetPosition(pickupA.transform.position);
            ChangeMenuVisibility(false);
        }
    }

    private void ButtonPickupBClicked()
    {
        if (pickupB != null)
        {
            UA11YSonarManager.Instance.StartGuideToTargetPosition(pickupB.transform.position);
            ChangeMenuVisibility(false);
        }
    }

    private void ButtonPickupCClicked()
    {
        if (pickupC != null)
        {
            UA11YSonarManager.Instance.StartGuideToTargetPosition(pickupC.transform.position);
            ChangeMenuVisibility(false);
        }
    }

    #endregion

    #region IPickupCallbackObject

    public bool CanBePickedUpByCollider(Collider other, int pickupID)
    {
        bool shouldPickup = false;
        Collider playerCollider = playerController.gameObject.GetComponent<Collider>();

        if(playerCollider != null && playerCollider == other && pickupID == currentTargetPickupID)
        {
            shouldPickup = true;

            UA11YSonarTestPickupController pickup = PickupForID(pickupID);

            if (pickup != null && pickup.GetComponent<UA11YSonarTarget>() != null)
            {
                UA11YSonarTarget target = pickup.GetComponent<UA11YSonarTarget>();
                UA11YUIManager.Instance.AnnouceMessage("Picked up " + target.label);

                // Disable button if pickup was picked up
                Button button = ButtonForPickup(pickup);
                if(button != null)
                {
                    button.gameObject.SetActive(false);
                }
            }

            currentTargetPickupID += 1;
        }

        return shouldPickup;
    }

    #endregion
    #region Private Helpers

    private UA11YSonarTestPickupController PickupForID(int id)
    {
        UA11YSonarTestPickupController[] pickupControllers = {pickupA, pickupB, pickupC};

        return Array.Find(pickupControllers, p => p.PickupID() == id);
    }

    private Button ButtonForPickup(UA11YSonarTestPickupController pickup)
    {
        Button button = null;
        // This is horrible manual code, but I don't care right now. It just needs to work now
        if(pickup == pickupA)
        {
            button = buttonPickupA; 
        }
        else if (pickup == pickupB)
        {
            button = buttonPickupB;
        }
        else if (pickup == pickupC)
        {
            button = buttonPickupC;
        }

        return button;
    }

    #endregion
}
