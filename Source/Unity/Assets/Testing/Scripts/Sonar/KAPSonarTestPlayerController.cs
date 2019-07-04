using UnityEngine;

public class KAPSonarTestPlayerController : MonoBehaviour
{
    [HideInInspector]
    public bool movementBlocked;

    private float playerSpeed = 10.0f;

    void Update()
    {
        if(!movementBlocked)
        {
            Vector3 position = transform.position;

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
            transform.position = position;
        }
    }
}
