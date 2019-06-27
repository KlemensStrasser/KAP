using UnityEngine;

public class KAPSonarPlayerController : MonoBehaviour
{
    private float playerSpeed = 10.0f;

    void Update()
    {
        Vector3 position = this.transform.position;

        if (Input.GetKey(KeyCode.DownArrow)) 
        {
            position.z -= Time.deltaTime * playerSpeed;
        }
        else if (Input.GetKey(KeyCode.UpArrow))
        {
            position.z += Time.deltaTime * playerSpeed;
        }
        else if (Input.GetKey(KeyCode.LeftArrow))
        {
            position.x -= Time.deltaTime * playerSpeed;
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            position.x += Time.deltaTime * playerSpeed;
        }
        this.transform.position = position;
    }
}
