using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent), typeof(AudioSource))]
public class EnemyController : MonoBehaviour
{
    [HideInInspector]
    public int AudioTimeSample
    {
        get { return audioSource.timeSamples; }
    }

    [Header("Movement Settings")]
    [Range(0f, 100f)]
    public float moveSpeed = 2f;

    private NavMeshAgent agent;
    private AudioSource audioSource;

    public void PlayVoice(AudioClip clip, int samplePos)
    {
        audioSource.clip = clip;
        audioSource.timeSamples = samplePos;
        audioSource.Play();
    }

    public void StopVoice()
    {
        audioSource.Stop();
    }

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        agent.SetDestination(GameManager.Instance.player.position);
        Debug.Log(audioSource.timeSamples);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.Instance.GameOver();
        }
    }
}
