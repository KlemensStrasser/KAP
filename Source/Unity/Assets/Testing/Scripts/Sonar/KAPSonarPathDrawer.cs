using UnityEngine;
using UnityEngine.AI;

public class KAPSonarPathDrawer : MonoBehaviour
{
    public Camera cam;
    public KAPSonarManager lighthouseManager;
    public Transform playerTransform;
    private NavMeshPath path;
    private float elapsed = 0.0f;

    bool clickedOnce = false;
    private Vector3 targetPosition;

    void Start()
    {
        path = new NavMeshPath();
        elapsed = 0.0f;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                targetPosition = hit.point;

                if(lighthouseManager != null)
                {
                    lighthouseManager.SetTargetPosition(targetPosition);
                    NavMesh.CalculatePath(playerTransform.position, targetPosition, NavMesh.AllAreas, path);
                }

                clickedOnce = true;
            }
        }

        // Update the way to the goal every second.
        elapsed += Time.deltaTime;
        if (elapsed > 1.0f && clickedOnce == true)
        {
            elapsed -= 1.0f;

        }

        if (path != null)
        {
            for (int i = 0; i < path.corners.Length - 1; i++)
            {
                Debug.DrawLine(path.corners[i], path.corners[i + 1], Color.red);
            }
        }
    }
}