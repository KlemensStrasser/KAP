using System;
using UnityEngine;
using UnityEngine.UI;

public class KAPSonarTestMenuController : MonoBehaviour, IPickupCallbackObject
{
    [Header("UI")]
    public GameObject mainMenu;
    public GameObject winningMenu;

    // TODO: (If I got some time) dynamically create these buttons for every pickup in the scene
    public Button buttonPickupA;
    public Button buttonPickupB;
    public Button buttonPickupC;

    [Header("Pickups")]
    public KAPSonarTestPickupController pickupA;
    public KAPSonarTestPickupController pickupB;
    public KAPSonarTestPickupController pickupC;

    [Header("Player")]
    public KAPSonarTestPlayerController playerController;

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

        KAPUIManager.Instance.SetScreenReaderEnabled(false);
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
        KAPUIManager.Instance.SetScreenReaderEnabled(true);
        KAPUIManager.Instance.VisibleElementsDidChange();

        if (playerController != null)
        {
            playerController.movementBlocked = true;
        }

        KAPSonarManager.Instance.gameObject.SetActive(false);
    }

    void ChangeMenuVisibility(bool showMenu)
    {
        if (mainMenu != null)
        {
            KAPUIManager.Instance.SetScreenReaderEnabled(showMenu);
            mainMenu.SetActive(showMenu);

            if (showMenu)
            {
                KAPUIManager.Instance.VisibleElementsDidChange();
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
            KAPSonarManager.Instance.StartGuideToTargetPosition(pickupA.transform.position);
            ChangeMenuVisibility(false);
        }
    }

    private void ButtonPickupBClicked()
    {
        if (pickupB != null)
        {
            KAPSonarManager.Instance.StartGuideToTargetPosition(pickupB.transform.position);
            ChangeMenuVisibility(false);
        }
    }

    private void ButtonPickupCClicked()
    {
        if (pickupC != null)
        {
            KAPSonarManager.Instance.StartGuideToTargetPosition(pickupC.transform.position);
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

            KAPSonarTestPickupController pickup = PickupForID(pickupID);

            if (pickup != null && pickup.GetComponent<KAPSonarTarget>() != null)
            {
                KAPSonarTarget target = pickup.GetComponent<KAPSonarTarget>();
                KAPUIManager.Instance.AnnouceMessage("Picked up " + target.label);

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

    private KAPSonarTestPickupController PickupForID(int id)
    {
        KAPSonarTestPickupController[] pickupControllers = new[] {pickupA, pickupB, pickupC};

        return Array.Find(pickupControllers, p => p.PickupID() == id);
    }

    private Button ButtonForPickup(KAPSonarTestPickupController pickup)
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
