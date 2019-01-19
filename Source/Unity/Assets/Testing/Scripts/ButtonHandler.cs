using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonHandler : MonoBehaviour {

    private Button button;

	// Use this for initialization
	void Start ()
    {
        this.button = gameObject.GetComponent<Button>();
        this.button.onClick.AddListener(this.Click);
	}

    void Click() 
    {
        Debug.Log("Click");
    }
}
