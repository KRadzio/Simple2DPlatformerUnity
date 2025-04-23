using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;
using UnityEngine.SceneManagement;

public enum GameState { GS_PAUSEMENU, GS_GAME, GS_LEVELCOMPLETED, GS_GAME_OVER }
public class GameManager: MonoBehaviour
{
    public GameState currentGameState = GameState.GS_PAUSEMENU;

    public Canvas inGameCanvas;

    public TMP_Text scoreText;

    public TMP_Text finishScoreText;

    public TMP_Text highScoreText;

    public Image[] keysTab;

    public Image heart;

    public TMP_Text livesText;

    public TMP_Text orzelText;

    public TMP_Text timeText;

    public Canvas pauseMenuCanvas;

    public Canvas levelCompletedCanvas;

    const string keyHighScore = "HighScoreLevel1";


    private int orzel = 0;
    private float orzelCooldown = 0;
    private float timer = 0f;
    private int lives = 3;
    private int score = 0;
    private int keysFound = 0;
    public static GameManager Instance;


    public void OnResumeButtonClicked()
    {
        InGame();
    }

    public void OnRestartButtonClicked()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void OnReturnToMainMenuButtonClicked()
    {
        SceneManager.LoadScene("MainMenu");
    }
    public void killOrzel()
    {
        if(orzelCooldown <= 0 )
        {
            orzel++;
            orzelText.text = string.Format("{0}/8", orzel);
            Debug.Log("Killed an enemy");
        }   
        orzelCooldown = 0.5f;
    }
    public void AddPoints(int points)
    {
        score += points;
        scoreText.text = score.ToString("0000");
    }

    public void AddLive()
    {
        lives++;
        livesText.text = lives.ToString("0");
    }

    public void DecLive()
    {
        lives--;
        livesText.text = lives.ToString("0");
        if (lives < 1)
        {
            //currentGameState = GameState.GS_PAUSEMENU;
            Debug.Log("Game Over");
            livesText.enabled = false;
            heart.enabled = false;
        }
            
    }

    public int GetLives()
    {
        return this.lives;
    }

    private void SetGameState(GameState newGameState)
    {
        currentGameState = newGameState;
        if (newGameState == GameState.GS_GAME)
            inGameCanvas.enabled = true;
        else if(newGameState == GameState.GS_PAUSEMENU)
        {
            inGameCanvas.enabled = false;
        }
        else if(newGameState == GameState.GS_LEVELCOMPLETED)
        {
            Scene currentScene = SceneManager.GetActiveScene();
            if(currentScene.name == "Level1")
            {
                int highScore = PlayerPrefs.GetInt(keyHighScore);
                if (highScore < score)
                {
                    highScore = score;
                    PlayerPrefs.SetInt(keyHighScore, score);
                }
                finishScoreText.text = "Your Score: " + score;
                highScoreText.text = "The Best Score " + highScore;
            }
        }
        pauseMenuCanvas.enabled = currentGameState == GameState.GS_PAUSEMENU;
        levelCompletedCanvas.enabled = currentGameState == GameState.GS_LEVELCOMPLETED;
    }

    public void AddKeys(Color newColor)
    {
        keysTab[keysFound].color = newColor;
        keysFound++;
    }

    void Awake()
    {
        Instance = this;
        inGameCanvas.enabled = false;
        scoreText.text = score.ToString("0000");
        livesText.text = lives.ToString("0");
        for(int i=0; i<keysTab.Length; i++)
        {
            keysTab[i].color = Color.gray;
        }
        InGame();
        if (!PlayerPrefs.HasKey(keyHighScore))
            PlayerPrefs.SetInt(keyHighScore, 0);
    }

    public void PauseMenu() { SetGameState(GameState.GS_PAUSEMENU); }
    public void InGame() { SetGameState(GameState.GS_GAME); }
    public void LevelCompleted() { SetGameState(GameState.GS_LEVELCOMPLETED); }
    public void GameOver() { SetGameState(GameState.GS_GAME_OVER); }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if (currentGameState == GameState.GS_PAUSEMENU)
            {
                InGame();
            }
                
            else if (currentGameState == GameState.GS_GAME)
            {
                PauseMenu();
            }
                
        }
        if(orzelCooldown > 0)
            orzelCooldown -= Time.deltaTime;
        if (orzelCooldown < 0)
            orzelCooldown = 0;
        timer += Time.deltaTime;
        int minutes = (int)(timer / 60);
        int seconds = (int)(timer % 60);
        timeText.text = string.Format("Time: {0:00}:{1:00}", minutes, seconds);
    }
}
