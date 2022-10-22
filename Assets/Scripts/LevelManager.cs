using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using Cinemachine;

public class LevelManager : MonoBehaviour
{
    private int currentLevel;

    public static LevelManager instance;

    private float timeTakenThisLevel;

    private LevelCompleteUIController levelCompleteUI;

    public static event Action OnPlayerDeath;

    private bool playerIsDead;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);

        if (instance == null)
        {
            instance = this;
        } else
        {
            DestroyImmediate(gameObject);
        }
    }

    private void Start()
    {
        InitManager();
    }

    private void InitManager()
    {
        timeTakenThisLevel = 0;
        playerIsDead = false;

        foreach (var listener in FindObjectsOfType<CinemachineImpulseListener>())
        {
            listener.enabled = false;
            listener.enabled = true;
        }
    }

    public void RegisterLevelCompleteUI(LevelCompleteUIController ui)
    {
        levelCompleteUI = ui;
    }

    public void HandleLevelComplete()
    {
        if (!InLevelCompleteUI()) levelCompleteUI.Show(timeTakenThisLevel);
    }

    public void OnDeath()
    {
        if (levelCompleteUI.IsVisible() || playerIsDead) return;

        playerIsDead = true;

        OnPlayerDeath?.Invoke();

        Invoke("RestartOrGoToNextLevel", 1.5f);
    }

    private void RestartOrGoToNextLevel()
    {
        SceneManager.LoadScene(currentLevel);

        InitManager();
    }

    private void Update()
    {
        if (!InLevelCompleteUI()) timeTakenThisLevel += Time.deltaTime;
    }

    public bool IsOnFinalLevel()
    {
        return currentLevel == SceneManager.sceneCountInBuildSettings - 1;
    }

    public void GoToNextLevel()
    {
        if (InLevelCompleteUI())
        {
            if (IsOnFinalLevel())
            {
                currentLevel = 0;
            } else
            {
                currentLevel++;
            }

            RestartOrGoToNextLevel();
        }
    }

    public bool InLevelCompleteUI()
    {
        return levelCompleteUI.IsVisible();
    }

    public int GetCurrentLevel()
    {
        return currentLevel;
    }
}
