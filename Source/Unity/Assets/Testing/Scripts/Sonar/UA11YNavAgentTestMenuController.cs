using System;
using UnityEngine;
using UnityEngine.UI;

public class UA11YNavAgentTestMenuController : MonoBehaviour, IPickupCallbackObject
{
    [Header("UI")]
    public GameObject mainMenu;
    public GameObject winningMenu;

    // TODO: (If I got some time) dynamically create these buttons for every pickup in the scene
    public Button buttonPickupA;
    public Button buttonPickupB;
    public Button buttonPickupC;

    [Header("Pickups")]
    public UA11YNavAgentTestPickupController pickupA;
    public UA11YNavAgentTestPickupController pickupB;
    public UA11YNavAgentTestPickupController pickupC;

    [Header("Player")]
    public UA11YNavAgentTestPlayerController playerController;

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

        if (pickupA != null)
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

        UA11YScreenReaderManager.Instance.SetScreenReaderEnabled(false);
    }

    void Update()
    {
        if (winningMenu.activeSelf == false)
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
        UA11YScreenReaderManager.Instance.SetScreenReaderEnabled(true);
        UA11YScreenReaderManager.Instance.VisibleElementsDidChange();

        if (playerController != null)
        {
            playerController.movementBlocked = true;
        }

        UA11YNavAgentManager.Instance.gameObject.SetActive(false);
    }

    void ChangeMenuVisibility(bool showMenu)
    {
        if (mainMenu != null)
        {
            UA11YScreenReaderManager.Instance.SetScreenReaderEnabled(showMenu);
            mainMenu.SetActive(showMenu);

            if (showMenu)
            {
                UA11YScreenReaderManager.Instance.VisibleElementsDidChange();
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
            UA11YNavAgentManager.Instance.StartGuideToTargetPosition(pickupA.transform.position);
            ChangeMenuVisibility(false);
        }
    }

    private void ButtonPickupBClicked()
    {
        if (pickupB != null)
        {
            UA11YNavAgentManager.Instance.StartGuideToTargetPosition(pickupB.transform.position);
            ChangeMenuVisibility(false);
        }
    }

    private void ButtonPickupCClicked()
    {
        if (pickupC != null)
        {
            UA11YNavAgentManager.Instance.StartGuideToTargetPosition(pickupC.transform.position);
            ChangeMenuVisibility(false);
        }
    }

    #endregion

    #region IPickupCallbackObject

    public bool CanBePickedUpByCollider(Collider other, int pickupID)
    {
        bool shouldPickup = false;
        Collider playerCollider = playerController.gameObject.GetComponent<Collider>();

        if (playerCollider != null && playerCollider == other && pickupID == currentTargetPickupID)
        {
            shouldPickup = true;

            UA11YNavAgentTestPickupController pickup = PickupForID(pickupID);

            if (pickup != null && pickup.GetComponent<UA11NavAgentTarget>() != null)
            {
                UA11NavAgentTarget target = pickup.GetComponent<UA11NavAgentTarget>();
                UA11YScreenReaderManager.Instance.AnnounceMessage("Picked up " + target.label);

                // Disable button if pickup was picked up
                Button button = ButtonForPickup(pickup);
                if (button != null)
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

    private UA11YNavAgentTestPickupController PickupForID(int id)
    {
        UA11YNavAgentTestPickupController[] pickupControllers = { pickupA, pickupB, pickupC };

        return Array.Find(pickupControllers, p => p.PickupID() == id);
    }

    private Button ButtonForPickup(UA11YNavAgentTestPickupController pickup)
    {
        Button button = null;
        // This is horrible manual code, but I don't care right now. It just needs to work now
        if (pickup == pickupA)
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
