using UnityEngine;
using System.Collections.Generic;

public class UA11YNavAgentPathDrawer : MonoBehaviour
{
    public Camera cam;
    public UA11YNavAgentManager NavAgentManager;
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
        List<Vector3> pathPoints = NavAgentManager.PathPoints();
        for (int i = 0; i < pathPoints.Count - 1; i++)
        {
            Debug.DrawLine(pathPoints[i], pathPoints[i + 1], Color.red);
        }
        
    }
}