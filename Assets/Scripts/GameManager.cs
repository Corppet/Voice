using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
    [SerializeField] private AudioClip[] monologues;
    private EnemyController currentEnemy;
    private float currentSample;

    [Header("Controls")]
    [SerializeField] private KeyCode menuKey = KeyCode.Escape;

    [SerializeField] private GameObject crosshair;

    public void ReturnToMenu()
    {
        SceneManager.LoadScene(0);
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

    public void GameOver()
    {
        IsInPlay = false;
        Debug.Log("Game Over");
    }

    private void Start()
    {
        IsInPlay = true;

        currentEnemy = null;
        currentSample = 0;
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.Escape))
        {
            ReturnToMenu();
        }
    }
}
