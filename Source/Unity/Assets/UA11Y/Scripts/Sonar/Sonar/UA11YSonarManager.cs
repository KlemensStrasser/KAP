using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

/// <summary>
/// UA11YSonar manager can guide the player to a given position.
/// (It's not really a sonar, more a inverse sonar I guess?)
/// </summary>
/// The automatic calculation of the path to a given point only works when a NavMesh is in the scene
/// 
/// Other preconditions are:
/// - A player that either has a camera or a audiolistener attached. (Don't forget to remove the audiolistener from the camera, if the audiolistener is directly attached)
/// - A player with a collider/rigidbody
/// - The player gameObject needs to be given here
public class UA11YSonarManager : MonoBehaviour, IUA11YSonarEventReceiver
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
    /// Needed in case the player gets assigned before the sonar was created
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
    /// The key used to change between manual and automatic sonar mode
    /// </summary>
    public KeyCode changeSonarModeKey = KeyCode.B;

    /// <summary>
    /// The key used for manually pinging the sonar when manual mode is activated
    /// </summary>
    public KeyCode manualSonarTriggerKey = KeyCode.N;

    /// <summary>
    /// The key used to force path recalculation, in case the player gets stuck
    /// </summary>
    public KeyCode forcePathRecalculationKey = KeyCode.R;

    /// <summary>
    /// If true, user can manually activate the sonar instead of hearing a constant ping
    /// </summary>
    private bool manuallyTriggerSonarSignal = false;

    /// <summary>
    /// Transform of the Player
    /// </summary>
    private Transform playerTransform;

    /// <summary>
    /// List of all points where the sonar will be placed
    /// </summary>
    private List<Vector3> pathPoints;
    /// The index of the corner in the path where the sonar is currenty positioned
    private int cornerIndex = -1;

    /// Gameobject of the sonar
    private GameObject sonar;

    /// The sonarController attached to the sonar
    private UA11YSonarController sonarController;

    private AudioSource soundEffectAudioSource;
    private AudioClip sonarReachedAudioClip;
    private AudioClip targetReachedAudioClip;


    private static UA11YSonarManager _instance;
    /// <summary>
    /// UA11YSonarManager Singleton
    /// Based on: https://gamedev.stackexchange.com/questions/116009/in-unity-how-do-i-correctly-implement-the-singleton-pattern
    /// </summary>
    public static UA11YSonarManager Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject instanceObject = Resources.Load<GameObject>("Prefabs/Sonar/UA11YSonarManager");
                _instance = Instantiate<GameObject>(instanceObject).GetComponent<UA11YSonarManager>();
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

            // Create and setup the sonar
            sonar = Resources.Load<GameObject>("Prefabs/Sonar/UA11YSonar");
            sonar = Instantiate<GameObject>(sonar);
            sonar.name = "UA11YSonar";
            sonar.transform.SetParent(this.transform);
            sonar.SetActive(false);

            sonarController = sonar.GetComponent<UA11YSonarController>();

            if (sonarController != null)
            {
                sonarController.eventReceiver = this;
                sonarController.ShouldLoop(!manuallyTriggerSonarSignal);

                ExtractValuesFromPlayer();
            }

            // Setup the sound stuff
            soundEffectAudioSource = gameObject.AddComponent<AudioSource>();
            soundEffectAudioSource.volume = 0.75f;

            sonarReachedAudioClip = Resources.Load("Audio/Sonar/UA11Y_SonarReached") as AudioClip;
            targetReachedAudioClip = Resources.Load("Audio/Sonar/UA11Y_SonarGoal") as AudioClip;
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

            if (pCollider != null && sonarController != null)
            {
                sonarController.SetPlayerCollider(pCollider);
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

            this.sonar.SetActive(true);
            RepositionSonar();
            sonarController.StartSignal();
        }
    }

    /// <summary>
    /// Uses predefined points as positions for the sonar and starts the guide
    /// </summary>
    public void StartGuideWithPoints(List<Vector3> points)
    {
        if (points != null && points.Count > 0)
        {
            pathPoints = points.ConvertAll(point => new Vector3(point.x, point.y, point.z));
            cornerIndex = 0;

            this.sonar.SetActive(true);
            RepositionSonar();
            sonarController.StartSignal();
        }
    }

    public List<Vector3> PathPoints()
    {
        return pathPoints ?? new List<Vector3>();
    }

    #region Private helpers for creating the path & repositioning the sonar

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
            Debug.LogError("UA11YSonarManager: PlayerTransform is null, cannot recalculate path!");
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

    private void RepositionSonar()
    {
        if (cornerIndex >= 0 && pathPoints != null && pathPoints.Count > cornerIndex && sonarController != null)
        {
            Vector3 currentCornerPosition = pathPoints[cornerIndex];

            // Take the y position from the player so that it floats right in front of him/her
            Vector3 sonarPosition = new Vector3(currentCornerPosition.x, playerTransform.position.y, currentCornerPosition.z);

            // Distance is needed to make sure the sound can be heard
            float distance = Vector3.Distance(sonarPosition, playerTransform.position);

            sonarController.UpdatePosition(sonarPosition, distance);
        }
    }

    private float CurrentDistanceToSonar()
    {
        Vector3 currentCornerPosition = pathPoints [cornerIndex];
        Vector3 sonarPosition = new Vector3(currentCornerPosition.x, playerTransform.position.y, currentCornerPosition.z);

        float distance = Vector3.Distance(sonarPosition, playerTransform.position);
        return distance;
    }

    #endregion

    #region User Triggered changes at the sonar

    private void Update()
    {
        if (Input.GetKeyDown(changeSonarModeKey))
        {
            manuallyTriggerSonarSignal = !manuallyTriggerSonarSignal;
            sonarController.ShouldLoop(!manuallyTriggerSonarSignal);
        } 
        else if (Input.GetKeyDown(forcePathRecalculationKey))
        {
            ForcePathRecalculation();
        }
        else if (manuallyTriggerSonarSignal && Input.GetKeyDown(manualSonarTriggerKey))
        {
            ManualTriggerSonar();
        } 
    }

    /// <summary>
    /// Manuals the trigger sonar.
    /// </summary>
    private void ManualTriggerSonar()
    {
        if (pathPoints != null && pathPoints.Count > 0)
        {
            // If the player wandered too far, this will ensure that the player 
            sonarController.EnsureThatSignalCanBeHeard(CurrentDistanceToSonar());
            sonarController.StartSignal();
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

    #region IUA11YSonarEventReceiver
    /// <summary>
    /// Plays a sound to indicate that the sonar is reached and repositions it (if needed)
    /// </summary>
    void IUA11YSonarEventReceiver.SonarReached()
    {
        if (cornerIndex == pathPoints.Count - 1)
        {
            this.sonar.SetActive(false);
            cornerIndex = -1;
            pathPoints.Clear();

            if (soundEffectAudioSource != null && targetReachedAudioClip != null)
            {
                soundEffectAudioSource.PlayOneShot(targetReachedAudioClip);
            }
        }
        else
        {
            if (soundEffectAudioSource != null && sonarReachedAudioClip != null)
            {
                soundEffectAudioSource.PlayOneShot(sonarReachedAudioClip);
            }

            cornerIndex += 1;

            // Deactivate so that if the sonar is placed within the character, the collider triggers again

            sonar.SetActive(false);
            RepositionSonar();
            sonar.SetActive(true);
            // TODO: This method makes the sonar sound stop, so we start it again. BUT this leads to the sound beign weird, so we need to find a different method!
            if (!manuallyTriggerSonarSignal) 
            {
                sonarController.StartSignal();
            }
        }
    }
    #endregion
}
