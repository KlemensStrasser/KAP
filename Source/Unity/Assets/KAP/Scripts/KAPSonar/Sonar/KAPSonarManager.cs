using UnityEngine;
using UnityEngine.AI;
using UnityEditor;

// For this to work, you have to have the following setup:
// - A NavMesh
// - A Player that either
//     - has the camera attached
//     - has an audiolistener attached. (Don't forget to remove the audiolistener from the camera!)
// - A Player with a rigidbody attached

public class KAPSonarManager : MonoBehaviour, IKAPSonarEventReceiver
{
    /// Transform of the Player
    public Transform playerTransform;

    /// The position we want to guide the player to
    private Vector3 targetPosition;
    /// The current path to the targetPositon
    private NavMeshPath path;
    /// The index of the corner in the path where the lighhouse is currenty
    /// positioned
    private int cornerIndex = -1;

    /// Gameobject of the sonar
    private GameObject sonar;

    /// The sonarController attached to the sonar
    private KAPSonarController sonarController;

    private AudioSource soundEffectAudioSource;
    private AudioClip sonarReachedAudioClip;

    private void Awake()
    {
        sonar = Resources.Load<GameObject>("Prefabs/Sonar/KAPSonar");
        sonar = Instantiate<GameObject>(sonar);
        sonar.name = "KAPSonar";
        sonar.transform.SetParent(this.transform);

        // To have some kind of valid position before the fun beginds
        targetPosition = playerTransform.position;
        sonarController = sonar.GetComponent<KAPSonarController>();
        sonarController.eventReceiver = this;
        sonar.SetActive(false);

        path = new NavMeshPath();

        soundEffectAudioSource = gameObject.AddComponent<AudioSource>();

        // TODO: Adjust volume correctly
        soundEffectAudioSource.volume = 0.75f;

        sonarReachedAudioClip = Resources.Load("Audio/Sonar/kap_SonarReached") as AudioClip;
    }

    private bool RecalculatePath()
    {
        bool didRecalculate;

        if (NavMesh.CalculatePath(playerTransform.position, targetPosition, NavMesh.AllAreas, path))
        {
            // First corner is the start position
            // Second corner is the first place to position the lighthouse
            cornerIndex = 1;
            didRecalculate = true;

            // TODO: Straight lines only!
        }
        else
        {
            Debug.LogWarning("Path could not be calculated");
            didRecalculate = false;
        }

        return didRecalculate;
    }

    private void RepositionLighthhouse()
    {
        if (cornerIndex >= 0 && path != null && path.corners.Length > cornerIndex && sonarController != null)
        {
            Vector3 currentCornerPosition = path.corners[cornerIndex];

            // Take the y position from the player so that it floats right in front of him/her
            Vector3 lightHousePosition = new Vector3(currentCornerPosition.x, playerTransform.position.y, currentCornerPosition.z);
            float distance = Vector3.Distance(lightHousePosition, playerTransform.position);
            sonarController.UpdatePosition(lightHousePosition, distance);
        }
    }

    public void SetTargetPosition(Vector3 targetPosition)
    {
        this.targetPosition = targetPosition;

        if (RecalculatePath())
        {
            this.sonar.SetActive(true);
            RepositionLighthhouse();
            sonarController.StartSignal();
        }
    }


    #region IKAPSonarEventReceiver

    void IKAPSonarEventReceiver.LighthouseReached()
    {
        if (cornerIndex == path.corners.Length - 1)
        {
            // TODO: Play different sound
            this.sonar.SetActive(false);
        }
        else
        {
            cornerIndex += 1;
            RepositionLighthhouse();
        }

        if (soundEffectAudioSource != null && sonarReachedAudioClip != null)
        {
            soundEffectAudioSource.PlayOneShot(sonarReachedAudioClip);
        }
    }
    #endregion
}
