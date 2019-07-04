using UnityEngine;
using System.Collections.Generic;

public class KAPSonarPathDrawer : MonoBehaviour
{
    public Camera cam;
    public KAPSonarManager sonarManager;
    public Transform playerTransform;
    private float elapsed = 0.0f;

    bool clickedOnce = false;
    private Vector3 targetPosition;

    void Start()
    {
        elapsed = 0.0f;
    }

    void Update()
    {
        //if (Input.GetMouseButtonDown(0))
        //{
        //    Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        //    RaycastHit hit;

        //    if (Physics.Raycast(ray, out hit))
        //    {
        //        targetPosition = hit.point;

        //        if (sonarManager != null)
        //        {
        //            sonarManager.StartGuideToTargetPosition(targetPosition);
        //            //NavMesh.CalculatePath(playerTransform.position, targetPosition, NavMesh.AllAreas, path);
        //        }

        //        clickedOnce = true;
        //    }
        //}

        // Update the way to the goal every second.
        elapsed += Time.deltaTime;
        if (elapsed > 1.0f && clickedOnce == true)
        {
            elapsed -= 1.0f;

        }

        List<Vector3> pathPoints = sonarManager.PathPoints();
        for (int i = 0; i < pathPoints.Count - 1; i++)
        {
            Debug.DrawLine(pathPoints[i], pathPoints[i + 1], Color.red);
        }
        
    }
}