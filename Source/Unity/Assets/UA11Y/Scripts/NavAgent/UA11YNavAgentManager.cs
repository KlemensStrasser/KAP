using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

/// <summary>
/// UA11YNavAgent manager can guide the player to a given position.
/// (It's not really a NavAgent, more a inverse NavAgent I guess?)
/// </summary>
/// The automatic calculation of the path to a given point only works when a NavMesh is in the scene
/// 
/// Other preconditions are:
/// - A player that either has a camera or a audiolistener attached. (Don't forget to remove the audiolistener from the camera, if the audiolistener is directly attached)
/// - A player with a collider/rigidbody
/// - The player gameObject needs to be given here
public class UA11YNavAgentManager : MonoBehaviour, IUA11YNavAgentEventReceiver
{

    /// <summary>
    /// The Player gameObject
    /// </summary>
    /// We need it to extract the transform and the colider
    public GameObject player 
    { 
        set 
        {
            _player = value;
            ExtractValuesFromPlayer();
        }
        get
        {
            return _player;
        }
    }

    /// <summary>
    /// Needed in case the player gets assigned before the NavAgent was created
    /// </summary>

    [SerializeField]
    private GameObject _player;

    /// <summary>
    /// Decide if the algorithm to straigth the diagonals should be applied
    /// </summary>
    public bool shouldStraightenDiagonals = false;

    /// <summary>
    /// A threshold that indicates if a line between two points counts as a straight line or a diagonal
    /// The developer might want to adjust this value depending on the characters movement
    /// </summary>
    public float isValidDiagonalThreshhold = 0.35f;

    /// <summary>
    /// The deepest recursion we should allow for the straightening Algorigthm.
    /// That's already pretty deep, but what is needed depends on the game
    /// </summary>
    public float maxRecursionLevel = 10;

    /// <summary>
    /// The key used to change between manual and automatic NavAgent mode
    /// </summary>
    public KeyCode changeNavAgentModeKey = KeyCode.B;

    /// <summary>
    /// The key used for manually pinging the NavAgent when manual mode is activated
    /// </summary>
    public KeyCode manualNavAgentTriggerKey = KeyCode.N;

    /// <summary>
    /// The key used to force path recalculation, in case the player gets stuck
    /// </summary>
    public KeyCode forcePathRecalculationKey = KeyCode.R;

    /// <summary>
    /// If true, user can manually activate the NavAgent instead of hearing a constant ping
    /// </summary>
    private bool manuallyTriggerNavAgentSignal = false;

    /// <summary>
    /// Transform of the Player
    /// </summary>
    private Transform playerTransform;

    /// <summary>
    /// List of all points where the NavAgent will be placed
    /// </summary>
    private List<Vector3> pathPoints;
    /// The index of the corner in the path where the NavAgent is currenty positioned
    private int cornerIndex = -1;

    /// Gameobject of the NavAgent
    private GameObject NavAgent;

    /// The NavAgentController attached to the NavAgent
    private UA11YNavAgentController NavAgentController;

    private AudioSource soundEffectAudioSource;
    private AudioClip NavAgentReachedAudioClip;
    private AudioClip targetReachedAudioClip;


    private static UA11YNavAgentManager _instance;
    /// <summary>
    /// UA11YNavAgentManager Singleton
    /// Based on: https://gamedev.stackexchange.com/questions/116009/in-unity-how-do-i-correctly-implement-the-singleton-pattern
    /// </summary>
    public static UA11YNavAgentManager Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject instanceObject = Resources.Load<GameObject>("Prefabs/NavAgent/UA11YNavAgentManager");
                _instance = Instantiate<GameObject>(instanceObject).GetComponent<UA11YNavAgentManager>();
            }

            return _instance;
        }
    }

    void OnValidate()
    {
        if(_player != null) 
        {
            ExtractValuesFromPlayer();
        }
    }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;

            // Create and setup the NavAgent
            NavAgent = Resources.Load<GameObject>("Prefabs/NavAgent/UA11YNavAgent");
            NavAgent = Instantiate<GameObject>(NavAgent);
            NavAgent.name = "UA11YNavAgent";
            NavAgent.transform.SetParent(this.transform);
            NavAgent.SetActive(false);

            NavAgentController = NavAgent.GetComponent<UA11YNavAgentController>();

            if (NavAgentController != null)
            {
                NavAgentController.eventReceiver = this;
                NavAgentController.ShouldLoop(!manuallyTriggerNavAgentSignal);

                ExtractValuesFromPlayer();
            }

            // Setup the sound stuff
            soundEffectAudioSource = gameObject.AddComponent<AudioSource>();
            soundEffectAudioSource.volume = 0.75f;

            NavAgentReachedAudioClip = Resources.Load("Audio/NavAgent/UA11Y_NavAgentReached") as AudioClip;
            targetReachedAudioClip = Resources.Load("Audio/NavAgent/UA11Y_NavAgentGoal") as AudioClip;
        }
    }

    /// <summary>
    /// Helper method to extract needed values from _player
    /// </summary>
    private void ExtractValuesFromPlayer()
    {
        if (_player != null)
        {
            Collider pCollider = _player.GetComponent<Collider>();
            Transform pTransform = _player.GetComponent<Transform>();

            if (pCollider != null && NavAgentController != null)
            {
                NavAgentController.SetPlayerCollider(pCollider);
            }

            if (pTransform != null)
            {
                playerTransform = pTransform;
            }
        }
    }

    /// <summary>
    /// Creates the path to the given target position and starts guide to that
    /// IMPORTANT: There is no check if these points are even reachable! So use this method carfully
    /// </summary>
    public void StartGuideToTargetPosition(Vector3 targetPosition)
    {
        List<Vector3> tempPoints = RecalculatePath(targetPosition);

        if (tempPoints.Count > 0)
        {
            pathPoints = tempPoints;

            this.NavAgent.SetActive(true);
            RepositionNavAgent();
            NavAgentController.StartSignal();
        }
    }

    /// <summary>
    /// Uses predefined points as positions for the NavAgent and starts the guide
    /// </summary>
    public void StartGuideWithPoints(List<Vector3> points)
    {
        if (points != null && points.Count > 0)
        {
            pathPoints = points.ConvertAll(point => new Vector3(point.x, point.y, point.z));
            cornerIndex = 0;

            this.NavAgent.SetActive(true);
            RepositionNavAgent();
            NavAgentController.StartSignal();
        }
    }

    public List<Vector3> PathPoints()
    {
        return pathPoints ?? new List<Vector3>();
    }

    #region Private helpers for creating the path & repositioning the NavAgent

    /// <summary>
    /// Calculate the path from the current position to the given target 
    /// </summary>
    private List<Vector3> RecalculatePath(Vector3 targetPosition)
    {
        List<Vector3> points = new List<Vector3>();

        NavMeshPath path = new NavMeshPath();

        if (playerTransform != null)
        {
            if (NavMesh.CalculatePath(playerTransform.position, targetPosition, NavMesh.AllAreas, path))
            {
                if (shouldStraightenDiagonals)
                {
                    points = StraightenDiagonalsInPath(path);

                }
                else
                {
                    for (int i = 1; i < path.corners.Length; i++)
                    {
                        points.Add(path.corners[i]);
                    }
                }
            }
        } 
        else
        {
            Debug.LogError("UA11YNavAgentManager: PlayerTransform is null, cannot recalculate path!");
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
    /// 4. Call RecursiveSplitDiagonal on previous and current point
    /// 4.1 Add all these points to the straight path points
    /// 5. Add the current point to the straight path points
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
                straightPathPoints.AddRange(RecursiveSplitDiagonal(previousCorner, currentCorner, 0));
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
    /// 1. Calculate the path from the source to the split
    /// 2. If a direct path exsits (only points are source and split) -> Continue, Else FALSE
    /// 3. Calculate the path from the split to the target
    /// 4. If a direct path exsits (only points are source and split) -> TRUE, Else FALSE
    /// 
    /// This method is only called from RecursiveSplitDiagonal with a split point that is either:
    /// - horizontal to the source and vertial to the target
    /// - vertial to the source and horizontal to the target
    private bool IsSplitValid(Vector3 source, Vector3 target, Vector3 split)
    {
        bool splitIsOK = false;

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

        return splitIsOK;
    }

    private void RepositionNavAgent()
    {
        if (cornerIndex >= 0 && pathPoints != null && pathPoints.Count > cornerIndex && NavAgentController != null)
        {
            Vector3 currentCornerPosition = pathPoints[cornerIndex];

            // Take the y position from the player so that it floats right in front of him/her
            Vector3 NavAgentPosition = new Vector3(currentCornerPosition.x, playerTransform.position.y, currentCornerPosition.z);

            // Distance is needed to make sure the sound can be heard
            float distance = Vector3.Distance(NavAgentPosition, playerTransform.position);

            NavAgentController.UpdatePosition(NavAgentPosition, distance);
        }
    }

    private float CurrentDistanceToNavAgent()
    {
        Vector3 currentCornerPosition = pathPoints [cornerIndex];
        Vector3 NavAgentPosition = new Vector3(currentCornerPosition.x, playerTransform.position.y, currentCornerPosition.z);

        float distance = Vector3.Distance(NavAgentPosition, playerTransform.position);
        return distance;
    }

    #endregion

    #region User Triggered changes at the NavAgent

    private void Update()
    {
        if (Input.GetKeyDown(changeNavAgentModeKey))
        {
            manuallyTriggerNavAgentSignal = !manuallyTriggerNavAgentSignal;
            NavAgentController.ShouldLoop(!manuallyTriggerNavAgentSignal);
        } 
        else if (Input.GetKeyDown(forcePathRecalculationKey))
        {
            ForcePathRecalculation();
        }
        else if (manuallyTriggerNavAgentSignal && Input.GetKeyDown(manualNavAgentTriggerKey))
        {
            ManualTriggerNavAgent();
        } 
    }

    /// <summary>
    /// Manuals the trigger NavAgent.
    /// </summary>
    private void ManualTriggerNavAgent()
    {
        if (pathPoints != null && pathPoints.Count > 0)
        {
            // If the player wandered too far, this will ensure that the player 
            NavAgentController.EnsureThatSignalCanBeHeard(CurrentDistanceToNavAgent());
            NavAgentController.StartSignal();
        }
    }

    private void ForcePathRecalculation()
    {
        // Goal is last position in the current path
        if(pathPoints != null && pathPoints.Count > 0)
        {
            Vector3 targetPosition = pathPoints[pathPoints.Count - 1];
            StartGuideToTargetPosition(targetPosition);
        }
    }

    #endregion

    #region IUA11YNavAgentEventReceiver
    /// <summary>
    /// Plays a sound to indicate that the NavAgent is reached and repositions it (if needed)
    /// </summary>
    void IUA11YNavAgentEventReceiver.NavAgentReached()
    {
        if (cornerIndex == pathPoints.Count - 1)
        {
            this.NavAgent.SetActive(false);
            cornerIndex = -1;
            pathPoints.Clear();

            if (soundEffectAudioSource != null && targetReachedAudioClip != null)
            {
                soundEffectAudioSource.PlayOneShot(targetReachedAudioClip);
            }
        }
        else
        {
            if (soundEffectAudioSource != null && NavAgentReachedAudioClip != null)
            {
                soundEffectAudioSource.PlayOneShot(NavAgentReachedAudioClip);
            }

            cornerIndex += 1;

            // Deactivate so that if the NavAgent is placed within the character, the collider triggers again

            NavAgent.SetActive(false);
            RepositionNavAgent();
            NavAgent.SetActive(true);
            // TODO: This method makes the NavAgent sound stop, so we start it again. BUT this leads to the sound beign weird, so we need to find a different method!
            if (!manuallyTriggerNavAgentSignal) 
            {
                NavAgentController.StartSignal();
            }
        }
    }
    #endregion
}
