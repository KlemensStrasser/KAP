﻿using UnityEngine;

public interface IKAPSonarEventReceiver
{
    void SonarReached();
}

public class KAPSonarController : MonoBehaviour
{
    [HideInInspector]
    public IKAPSonarEventReceiver eventReceiver;

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
    /// <param name="distance">Distance from the sonar to the player.</param>
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
            Debug.LogError("KAPSonarController: Audiosource is null");
        }
        else if(audioSource.isPlaying) 
        {
            Debug.LogWarning("KAPSonarController: Audiosource is already playing");
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
            Debug.LogError("KAPSonarController: Audiosource is null");
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
            Debug.LogError("KAPSonarController: Audiosource is null");
        }
    }

    #endregion

    /// <summary>
    /// Trigger sonar reached of the event receiver
    /// </summary>
    private void OnTriggerEnter(Collider other)
    {
        if(eventReceiver != null && other == playerCollider)
        {
            eventReceiver.SonarReached();
        }
        else 
        {
            Debug.LogError("KAPSonarController: EventReceiver is null");
        }
    }
}