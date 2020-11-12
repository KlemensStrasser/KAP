using UnityEngine;
using UnityEngine.UI;

public class PopupTestController : MonoBehaviour
{
    public Button closeButton;

    void Awake()
    {
        if (closeButton != null)
        {
            closeButton.onClick.AddListener(ClosePopover);
        }
    }

    void ClosePopover()
    {
        gameObject.SetActive(false);
        UA11YScreenReaderManager.Instance.VisibleElementsDidChange();
    }
}
