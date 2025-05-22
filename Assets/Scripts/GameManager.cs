using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManeger : MonoBehaviour
{
    public static GameManeger Instance;
    public int currentlevel = 1;

    private bool persistentSceneLoaded = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadPersistentScene();
        }
    }

    void LoadPersistentScene()
    {
        if (!persistentSceneLoaded)
        {
            SceneManager.LoadScene("PersistentScene", LoadSceneMode.Additive);
            persistentSceneLoaded = true;
        }
    }

    public void StartGame()
    {
        PlayerHealth.ResetStaticHealth(); // ← Reset health at game start
        currentlevel = 1;
        LoadLevel(currentlevel);
    }

    public void LoadLevel(int level)
    {
        SceneManager.LoadScene("Level" + level);
    }

    public void LoadNextLevel()
    {
        currentlevel++;
        if (currentlevel > 5)
        {
            SceneManager.LoadScene("VictoryScene");
        }
        else
        {
            LoadLevel(currentlevel);
        }
    }

    public void GameOver()
    {
        SceneManager.LoadScene("GameOverScene");
    }

    public void LoadStartScene()
    {
        Debug.Log("Load StartScene");
        PlayerHealth.ResetStaticHealth(); // ← Also reset health on going to start
        SceneManager.LoadScene("StartScene");
    }
} 


