using UnityEngine;

public interface IUA11YNavAgentEventReceiver
{
    void NavAgentReached();
}

public class UA11YNavAgentController : MonoBehaviour
{
    [HideInInspector]
    public IUA11YNavAgentEventReceiver eventReceiver;

    /// <summary>
    /// Minimal distance from where the audiosource should be heard
    /// </summary>
    public float minDistance = 4.0f;

    /// <summary>
    /// Collider of the Player
    /// </summary>
    private Collider playerCollider;

    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    /// <summary>
    /// Updates the position.
    /// </summary>
    /// <param name="position">New position of the lighthouse</param>
    /// <param name="distance">Distance from the NavAgent to the player.</param>
    public void UpdatePosition(Vector3 position, float distance) 
    {
        this.transform.position = position;

        if (audioSource != null)
        {
            // Ensure that the audiosource can be heard
            float suggestedDistance = distance * 1.5f;
            audioSource.maxDistance = suggestedDistance < minDistance ? minDistance : suggestedDistance;
        }
    }

    /// <summary>
    /// Ensures the that signal can be heard.
    /// </summary>
    /// <param name="distance">Distance from player to the NavAgent</param>
    /// 
    /// Used for manual ping. For the automatic ping, the user will hear that the signal will slowly fade out
    public void EnsureThatSignalCanBeHeard(float distance)
    {
        float suggestedDistance = distance * 1.5f;
        if(audioSource.maxDistance < suggestedDistance) 
        {
            audioSource.maxDistance = suggestedDistance;
        }
    }

    public void SetPlayerCollider(Collider collider)
    {
        playerCollider = collider;
    }

    #region Signaling

    /// <summary>
    /// Starts the signal. Loops depending on ShouldLoop was called or not
    /// </summary>
    public void StartSignal()
    {
        if(audioSource != null && !audioSource.isPlaying)
        {
            audioSource.Play();
        }
        else if(audioSource == null) 
        {
            Debug.LogError("UA11YNavAgentController: Audiosource is null");
        }
        else if(audioSource.isPlaying) 
        {
            Debug.LogWarning("UA11YNavAgentController: Audiosource is already playing");
        }
    }

    public void StopSignal() 
    {
        if (audioSource != null)
        {
            audioSource.Stop();
        }
        else
        {
            Debug.LogError("UA11YNavAgentController: Audiosource is null");
        }
    }

    public void ShouldLoop(bool shouldLoop)
    {
        if (audioSource != null)
        {
            audioSource.loop = shouldLoop;

            if(shouldLoop)
            {
                StartSignal();
            } 
            else
            {
                StopSignal();
            }
        }
        else if (audioSource == null)
        {
            Debug.LogError("UA11YNavAgentController: Audiosource is null");
        }
    }

    #endregion

    /// <summary>
    /// Trigger NavAgent reached of the event receiver
    /// </summary>
    private void OnTriggerEnter(Collider other)
    {
        if(eventReceiver != null && other == playerCollider)
        {
            eventReceiver.NavAgentReached();
        }
        else 
        {
            Debug.LogError("UA11YNavAgentController: EventReceiver is null");
        }
    }
}
