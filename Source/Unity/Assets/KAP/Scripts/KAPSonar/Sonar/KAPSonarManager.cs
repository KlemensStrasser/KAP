using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;


/// <summary>
/// KAPSonar manager can guide the player to a given position.
/// (It's not really a sonar, more a inverse sonar I guess?)
/// </summary>
/// The automatic calculation of the path to a given point only works when a NavMesh is in the scene
/// 
/// Other preconditions are:
/// - A player that either has a camera or a audiolistener attached. (Don't forget to remove the audiolistener from the camera, if the audiolistener is directly attached)
/// - A player with a rigidbody attached
/// - The transform of this player needs to be given here.
public class KAPSonarManager : MonoBehaviour, IKAPSonarEventReceiver
{
    /// Transform of the Player
    public Transform playerTransform;

    // Decide if the algorithm to straigth the diagonals should be applied
    public bool shouldStraightenDiagonals = false;

    /// A threshold that indicates if a line between two points counts as a straight line or a diagonal
    /// The developer might want to adjust this value depending on the characters movement
    public float isValidDiagonalThreshhold = 0.35f;

    /// The deepest recursion we should allow for the straightening Algorigthm.
    /// That's already pretty deep, but what is needed depends on the game
    public float maxRecursionLevel = 10;

    // TODO: Make the points private again
    // TODO: Add method to set points instead of a target position
    public List<Vector3> pathPoints;
    /// The index of the corner in the path where the lighhouse is currenty positioned
    private int cornerIndex = -1;

    /// Gameobject of the sonar
    private GameObject sonar;

    /// The sonarController attached to the sonar
    private KAPSonarController sonarController;

    private AudioSource soundEffectAudioSource;
    private AudioClip sonarReachedAudioClip;
    private AudioClip targetReachedAudioClip;

    // TODO: Setting to disable auto sonar

    private void Awake()
    {
        // Create and setup the sonar
        sonar = Resources.Load<GameObject>("Prefabs/Sonar/KAPSonar");
        sonar = Instantiate<GameObject>(sonar);
        sonar.name = "KAPSonar";
        sonar.transform.SetParent(this.transform);
        sonar.SetActive(false);

        sonarController = sonar.GetComponent<KAPSonarController>();
        sonarController.eventReceiver = this;

        // Setup the sound stuff
        soundEffectAudioSource = gameObject.AddComponent<AudioSource>();
        soundEffectAudioSource.volume = 0.75f;

        sonarReachedAudioClip = Resources.Load("Audio/Sonar/kap_SonarReached") as AudioClip;
        targetReachedAudioClip = Resources.Load("Audio/Sonar/kap_SonarGoal") as AudioClip;
    }

    /// <summary>
    /// Creates the path to the given target position and starts guide to that
    /// </summary>
    public void StartGuideToTargetPosition(Vector3 targetPosition)
    {
        List<Vector3> tempPoints = RecalculatePath(targetPosition);

        if (tempPoints.Count > 0)
        {
            pathPoints = tempPoints;

            this.sonar.SetActive(true);
            RepositionSonar();
            sonarController.StartSignal();
        }
    }

    /// <summary>
    /// Calculate the path from the current position to the given target 
    /// </summary>
    private List<Vector3> RecalculatePath(Vector3 targetPosition)
    {
        List<Vector3> points;

        NavMeshPath path = new NavMeshPath();
        if (NavMesh.CalculatePath(playerTransform.position, targetPosition, NavMesh.AllAreas, path))
        {
            if (shouldStraightenDiagonals)
            {
                points = StraightenDiagonalsInPath(path);

            } else 
            {
                points = new List<Vector3>();
                for (int i = 1; i < path.corners.Length; i++)
                {
                    points.Add(path.corners[i]);
                }
            }
        } else 
        {
            // Empty List
            points = new List<Vector3>();
        }

        cornerIndex = 0;

        return points;
    }

    /// <summary>
    /// Straighten out most diagonals to make the path easier navigable
    /// </summary>
    /// 
    /// Here is the basic idea of the Algorithm:
    /// Start at i = 1
    /// 1. Take a look at the line from the previous to the current point
    /// 2. If the line is (almost) horizontal or (almost) vertical -> continue
    /// 3. If the line is diagonal, split it at the center.
    /// 4. Call RecursiveSplitDiagonal on the first segment
    /// 4.1 Add all these points to the straight path points
    /// 5. Add the center point to the straight path points
    /// 6. Call RecursiveSplitDiagonal on the second segment
    /// 6.1 Add all these points to the straight path points
    /// 7. Continue with the next point 
    /// 
    /// End when all corners of the path have been looked at.
    /// Return straight path points
    private List<Vector3> StraightenDiagonalsInPath(NavMeshPath path) 
    {
        List<Vector3> straightPathPoints = new List<Vector3>();

        // First corner is the start position
        // Second corner is the first place to position the lighthouse
        // -> i = 1
        for (int i = 1; i < path.corners.Length; i++)
        {
            Vector3 previousCorner = path.corners[i - 1];
            Vector3 currentCorner = path.corners[i];

            // If X or Z coordinate is almost the same, 
            if (IsAValidLine(previousCorner, currentCorner))
            {
                straightPathPoints.Add(currentCorner);
            }
            else
            {
                Vector3 center = new Vector3((previousCorner.x + currentCorner.x) / 2.0f, previousCorner.y, (previousCorner.z + currentCorner.z) / 2.0f);


                straightPathPoints.AddRange(RecursiveSplitDiagonal(previousCorner, center, 0));
                straightPathPoints.Add(center);
                straightPathPoints.AddRange(RecursiveSplitDiagonal(center, currentCorner, 0));

                straightPathPoints.Add(currentCorner);
            }
        }

        return straightPathPoints;
    }

    /// <summary>
    /// Checks if the line between two points is (almost) horizontal or (almost) vertical
    /// </summary>
    private bool IsAValidLine(Vector3 pointA, Vector3 pointB) 
    {
        bool cornerIsValid = false;

        float valA = Mathf.Abs(Mathf.Sqrt(Mathf.Pow(pointA.x, 2)) - Mathf.Sqrt(Mathf.Pow(pointB.x, 2)));
        float valB = Mathf.Abs(Mathf.Sqrt(Mathf.Pow(pointA.z, 2)) - Mathf.Sqrt(Mathf.Pow(pointB.z, 2)));

        if (valA < isValidDiagonalThreshhold || valB < isValidDiagonalThreshhold)
        {
            cornerIsValid = true;
        }

        return cornerIsValid;
    }

    /// <summary>
    /// Recursive algorithm to split the diagonal in almost horizontal/vertical lines
    /// </summary>
    /// 
    /// The algorithm works as follows:
    /// 1. If the line between the two given points is not straight -> Continue ELSE return EMPTY
    /// 2. Create possibleAB (x coordinate of A, z coordinate of B)
    /// 3. Create possibleBA (x coordinate of B, z coordinate of A)
    /// 4. Check if possibleAB is a valid split point (see IsSplitValid for more info)
    /// 4.1 If Valid -> RETURN [possibleAB]
    /// 5. (Else) check if possibleBA is a valid split point
    /// 5.1 If Valid -> RETURN [possibleBA]
    /// 6. (Else) Split the line between A and B at the center
    /// 7. Call RecursiveSplitDiagonal with A and center
    /// 7.1 Add result to points
    /// 8. Add center to points
    /// 9. Call RecursiveSplitDiagonal with center and B
    /// 9.1 Add result to points
    /// 10. Return points
    private List<Vector3> RecursiveSplitDiagonal(Vector3 pointA, Vector3 pointB, int deepness) 
    {
        List<Vector3> points = new List<Vector3>();

        if(!IsAValidLine(pointA, pointB))
        {
            Vector3 possibleAB = new Vector3(pointA.x, pointA.y, pointB.z);
            Vector3 possibleBA = new Vector3(pointB.x, pointA.y, pointA.z);

            if (IsSplitValid(pointA, pointB, possibleAB))
            {
                points.Add(possibleAB);
            }
            else if (IsSplitValid(pointA, pointB, possibleBA))
            {
                points.Add(possibleBA);
            }
            else if (deepness < maxRecursionLevel)
            {
                // Recurse again!
                Vector3 center = new Vector3((pointA.x + pointB.x) / 2.0f, pointA.y, (pointA.z + pointB.z) / 2.0f);

                points.AddRange(RecursiveSplitDiagonal(pointA, center, deepness + 1));
                points.Add(center);
                points.AddRange(RecursiveSplitDiagonal(center, pointB, deepness + 1));
            }
        }

        return points;
    }

    /// <summary>
    /// Checks if a split at a given point is valid
    /// </summary>
    /// 
    /// The valid check works as follows:
    /// 1. Check if the point is even reachable (SamplePoint)
    /// 2. If it is, calculate a path from the source to the split, Else FALSE
    /// 3. Calculate the path from the source to the split
    /// 4. If a direct path exsits (only points are source and split) -> Continue, Else FALSE
    /// 5. Calculate the path from the split to the target
    /// 6. If a direct path exsits (only points are source and split) -> TRUE, Else FALSE
    /// 
    /// This method is only called from RecursiveSplitDiagonal with a split point that is either:
    /// - horizontal to the source and vertial to the target
    /// - vertial to the source and horizontal to the target
    private bool IsSplitValid(Vector3 source, Vector3 target, Vector3 split)
    {
        bool splitIsOK = false;
        NavMeshHit hit = new NavMeshHit();

        // TODO: Think again if we should even do the sample
        //       We could also just calculate the path to both points directly (one after another) and check that
        //if (NavMesh.SamplePosition(split, out hit, 0.5f, NavMesh.AllAreas))
        //{
            NavMeshPath pathA = new NavMeshPath();
            NavMesh.CalculatePath(source, split, NavMesh.AllAreas, pathA);

            if(pathA.status == NavMeshPathStatus.PathComplete && pathA.corners.Length == 2)
            {
                NavMeshPath pathB = new NavMeshPath();
                NavMesh.CalculatePath(split, target, NavMesh.AllAreas, pathB);

                // Split seems to be reachable from both locations in a straight line.
                // The split is definitley not diagonal. So we found something that is actually valid!
                if (pathB.status == NavMeshPathStatus.PathComplete && pathB.corners.Length == 2)
                {
                    splitIsOK = true;
                }
            }
        //}

        return splitIsOK;
    }

    private void RepositionSonar()
    {
        if (cornerIndex >= 0 && pathPoints != null && pathPoints.Count > cornerIndex && sonarController != null)
        {
            Vector3 currentCornerPosition = pathPoints[cornerIndex];

            // Take the y position from the player so that it floats right in front of him/her
            Vector3 lightHousePosition = new Vector3(currentCornerPosition.x, playerTransform.position.y, currentCornerPosition.z);

            // Distance is needed to make sure the sound can be heard
            float distance = Vector3.Distance(lightHousePosition, playerTransform.position);
            sonarController.UpdatePosition(lightHousePosition, distance);

            // TODO: Maybe we should get rid of the playerPosition here and use the first positon of the lightHouse to calculate the distance
            //       In that case, we need the very first point in the points too!!!
        }
    }

    #region IKAPSonarEventReceiver

    void IKAPSonarEventReceiver.LighthouseReached()
    {
        if (cornerIndex == pathPoints.Count - 1)
        {
            this.sonar.SetActive(false);

            if (soundEffectAudioSource != null && targetReachedAudioClip != null)
            {
                soundEffectAudioSource.PlayOneShot(targetReachedAudioClip);
            }
        }
        else
        {
            cornerIndex += 1;
            RepositionSonar();

            if (soundEffectAudioSource != null && sonarReachedAudioClip != null)
            {
                soundEffectAudioSource.PlayOneShot(sonarReachedAudioClip);
            }
        }
    }
    #endregion
}
