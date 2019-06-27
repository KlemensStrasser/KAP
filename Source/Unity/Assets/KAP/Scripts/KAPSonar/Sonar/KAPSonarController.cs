using UnityEngine;

public interface IKAPSonarEventReceiver
{
    void LighthouseReached();
}

public class KAPSonarController : MonoBehaviour
{
    [HideInInspector]
    public IKAPSonarEventReceiver eventReceiver;

    private AudioSource audioSource;
    private BoxCollider boxCollider;

    private void Awake()
    {
        audioSource = this.GetComponent<AudioSource>();
    }

    /// <summary>
    /// Updates the position.
    /// </summary>
    /// <param name="position">New position of the lighthouse</param>
    /// <param name="distance">Distance from the lighthouse to the player.</param>
    public void UpdatePosition(Vector3 position, float distance) 
    {
        this.transform.position = position;

        if (audioSource != null)
        {
            // Ensure that the audiosource can be heard
            audioSource.maxDistance = distance * 1.5f;
        }
    }

    public void StartSignal()
    {
        if(audioSource != null && !audioSource.isPlaying)
        {
            audioSource.Play();
        }
        else if(audioSource == null) 
        {
            Debug.LogError("KAPLighthouse: Audiosource is null");
        }
        else if(audioSource.isPlaying) 
        {
            Debug.LogWarning("KAPLighthouse: Audiosource is already playing");
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
            Debug.LogError("KAPLighthouse: Audiosource is null");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(eventReceiver != null)
        {
            eventReceiver.LighthouseReached();
        }
        else 
        {
            Debug.LogError("KAPLighthouse: EventReceiver is null");
        }
    }
}
