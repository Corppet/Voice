using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [HideInInspector] public static GameManager Instance { get; private set; }

    private bool _isInPlay = true;
    [HideInInspector] public bool IsInPlay
    {
        get { return _isInPlay; }

        set
        {
            if (value)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                crosshair.SetActive(true);
                return;
            }
            else
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                crosshair.SetActive(false);
            }
            _isInPlay = value;
        }
    }

    [HideInInspector] public UnityEvent OnEnemyHit;

    [Header("Enemy Settings")]
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private AudioClip[] monologues;
    private EnemyController currentEnemy;
    private AudioClip currentClip;
    private int currentSample;

    [Space(5)]

    [Header("Controls")]
    [SerializeField] private KeyCode menuKey = KeyCode.Escape;

    [Space(10)]

    [Header("Other References")]
    public Transform player;
    [SerializeField] private GameObject crosshair;
    [SerializeField] private GameObject hitmarker;
    [SerializeField] private Slider progressSlider;
    [SerializeField] private string lostScene = "Lose";
    [SerializeField] private string wonScene = "Win";

    public void ReturnToMenu()
    {
        SceneManager.LoadScene(0);
    }

    private void EndGame()
    {
        IsInPlay = false;
        if (currentEnemy != null)
        {
            currentEnemy.StopVoice();
            Destroy(currentEnemy.gameObject);
        }
    }

    public void GameOver()
    {
        EndGame();
        SceneManager.LoadScene(lostScene);
    }

    public void GameWin()
    {
        EndGame();
        SceneManager.LoadScene(wonScene);
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        OnEnemyHit = new();
    }

    private void Start()
    {
        IsInPlay = true;

        OnEnemyHit.AddListener(EnemyHit);

        currentEnemy = null;
        currentSample = 0;

        hitmarker.SetActive(false);

        SetupEnemies();
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.Escape))
        {
            ReturnToMenu();
        }

        if (!IsInPlay)
        {
            return;
        }

        if (currentEnemy.AudioTimeSample >= currentClip.samples)
        {
            GameWin();
        }

        progressSlider.value = (float)currentEnemy.AudioTimeSample / currentClip.samples;
    }

    private void SetNewEnemy()
    {
        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
        GameObject enemy = Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);
        currentEnemy = enemy.GetComponent<EnemyController>();
        currentEnemy.PlayVoice(currentClip, currentSample);
    }

    private void SetupEnemies()
    {
        currentClip = monologues[Random.Range(0, monologues.Length)];
        currentSample = 0;

        SetNewEnemy();
    }

    private void EnemyHit()
    {
        currentSample = currentEnemy.AudioTimeSample;

        // destroy the hit enemy
        Destroy(currentEnemy.gameObject);
        StartCoroutine(ShowHitmarker());

        // assign a new enemy starting from the current sample
        SetNewEnemy();
    }

    private IEnumerator ShowHitmarker()
    {
        hitmarker.SetActive(true);
        yield return new WaitForSeconds(.5f);
        hitmarker.SetActive(false);
    }
}
