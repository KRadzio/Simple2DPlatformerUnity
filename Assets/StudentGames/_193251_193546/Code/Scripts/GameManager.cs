using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;
using UnityEngine.SceneManagement;
using System.Xml.Linq;
using UnityEngine.InputSystem;

namespace _193251_193546 { 

public enum GameState { GS_PAUSEMENU, GS_GAME, GS_LEVELCOMPLETED, GS_GAME_OVER, GS_OPTIONS, GS_HELPMENU }
public class GameManager: MonoBehaviour
{
    public GameState currentGameState = GameState.GS_PAUSEMENU;

    public Canvas inGameCanvas;

    public Canvas helpMenuCanvas;

    public TMP_Text scoreText;

    public TMP_Text finishScoreText;

    public TMP_Text highScoreText;

    public TMP_Text gameOverScoreText;

    public TMP_Text gameOverhighScoreText;

    public TMP_Text armorMessage;

    public TMP_Text ammoMessage;

    public TMP_Text keyMessage;

    public TMP_Text secretMessage;

    public TMP_Text exitMessage;

    public TMP_Text extraLifeMessage;

    public TMP_Text infoMessage;

    public TMP_Text quality;

    public Image[] keysTab;

    public Image heart;

    public Image secret;

    public TMP_Text livesText;

    public TMP_Text orzelText;

    public TMP_Text timeText;

    public TMP_Text ammoText;

    public TMP_Text armorText;

    public TMP_Text secretText;

    public Button optionsButton;

    public Canvas pauseMenuCanvas;

    public Canvas levelCompletedCanvas;

    public Canvas optionsCanvas;

    public Canvas gameoverCanvas;

    public Slider slider;

    const string keyHighScore = "HighScore_193251_193546";


    private int orzel = 0;
    private float timer = 0f;
    private int lives = 3;
    private int score = 0;
    private int keysFound = 0;
    private int ammo = 20;
    private int armor = 0;
    private float armorCooldown = 0;
    private float secretsFound = 0;
    public static GameManager Instance;

    public void OnResumeButtonClicked()
    {
        if(currentGameState != GameState.GS_GAME && currentGameState != GameState.GS_HELPMENU)
            InGame();
    }

    public void OnRestartButtonClicked()
    {
        if(currentGameState != GameState.GS_GAME && currentGameState != GameState.GS_HELPMENU)
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void OnReturnToMainMenuButtonClicked()
    {
        int sceneIndex = SceneUtility.GetBuildIndexByScenePath("Scenes/Main Menu");
        if (sceneIndex >= 0)
        {
            if (currentGameState != GameState.GS_GAME && currentGameState != GameState.GS_HELPMENU)
                SceneManager.LoadSceneAsync(sceneIndex); //³adowanie sceny ³¹cz¹cej gry
        }
        else
        {
            if (currentGameState != GameState.GS_GAME && currentGameState != GameState.GS_HELPMENU)
                SceneManager.LoadScene("MainMenu");
        }  
    }

    public void OnBackToMenuButtonClicked()
    {
        if(currentGameState == GameState.GS_OPTIONS)
            PauseMenu();
    }

    public void OnOptionsButtonClicked()
    {
        quality.text = "Quality:" + QualitySettings.names[QualitySettings.GetQualityLevel()];
        if (currentGameState != GameState.GS_GAME && currentGameState != GameState.GS_HELPMENU)
            Options(); 
    }

    public void OnMinusButtonClicked()
    {
        if (currentGameState == GameState.GS_OPTIONS)
            QualitySettings.DecreaseLevel();
        quality.text = "Quality:" + QualitySettings.names[QualitySettings.GetQualityLevel()];
    }

    public void OnPlusButtonClicked()
    {
        if (currentGameState == GameState.GS_OPTIONS)
            QualitySettings.IncreaseLevel();
        quality.text = "Quality:" + QualitySettings.names[QualitySettings.GetQualityLevel()];
    }
    public void OnValueChanged()
    {
        if(currentGameState == GameState.GS_OPTIONS)
        {
            float vol = slider.value;
            SetVolume(vol);
        }    
    }
    public void SetVolume(float vol)
    {
        AudioListener.volume = vol;
    }

    public void killOrzel()
    {
            orzel++;
            orzelText.text = string.Format("{0}/67", orzel);  
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
        StartCoroutine(showExtraLifeMessage());
    }

    public void DecLive()
    {
        lives--;
        livesText.text = lives.ToString("0");
        if (lives < 1)
        {
            GameOver();
        }
            
    }

    public int GetLives()
    {
        return this.lives;
    }

    public void SetAmmo(int ammo)
    {
        this.ammo = ammo;
        ammoText.text = string.Format("Ammo: {00}", ammo);
    }

    public void SubtractAmmo()
    {
        ammo--;
        ammoText.text = string.Format("Ammo: {00}", ammo);
    }

    public int GetAmmo()
    {
        return this.ammo;
    }

    public void TakeArmor()
    {
        armor = 2;
        armorText.text = armor.ToString("0");
        StartCoroutine(showArmorMessage());
    }

    public int GetArmor()
    {
        return this.armor;
    }

    public void SubtractArmor()
    {
        if (armorCooldown <= 0)
        {
            armor--;
            armorText.text = armor.ToString("0");
            armorCooldown = 2.0f;
        }
    }

    public void setArmor(int newValue)
    {
        armor = newValue;
        armorText.text = armor.ToString("0");
    }

    public float GetArmorCooldown()
    {
        return armorCooldown;
    }

    public void incSecretCount() 
    { 
        secretsFound++; 
        secretText.text = secretText.text = string.Format("{0}/5", secretsFound);
        StartCoroutine(showSecretMessage());
    }

    private void SetGameState(GameState newGameState)
    {
        currentGameState = newGameState;
        if (newGameState == GameState.GS_GAME)
        {
            Time.timeScale = 1;
            inGameCanvas.enabled = true;
        }         
        else if(newGameState == GameState.GS_PAUSEMENU)
        {
            Time.timeScale = 0;
            inGameCanvas.enabled = false;
        }
        else if(newGameState == GameState.GS_LEVELCOMPLETED)
        {
            Time.timeScale = 0;
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
                highScoreText.text = "The Best Score: " + highScore;
            }
        }
        else if (newGameState == GameState.GS_GAME_OVER)
        {
            Time.timeScale = 0;
            Scene currentScene = SceneManager.GetActiveScene();
            if (currentScene.name == "Level1")
            {
                int highScore = PlayerPrefs.GetInt(keyHighScore);
                if (highScore < score)
                {
                    highScore = score;
                    PlayerPrefs.SetInt(keyHighScore, score);
                }
                gameOverScoreText.text = "Your Score: " + score;
                gameOverhighScoreText.text = "The Best Score: " + highScore;
            }
        }
        else if (newGameState == GameState.GS_HELPMENU)
        {
            Time.timeScale = 0;
            inGameCanvas.enabled = false;
        }
        optionsCanvas.enabled = currentGameState == GameState.GS_OPTIONS;
        pauseMenuCanvas.enabled = currentGameState == GameState.GS_PAUSEMENU;
        levelCompletedCanvas.enabled = currentGameState == GameState.GS_LEVELCOMPLETED;
        gameoverCanvas.enabled = currentGameState == GameState.GS_GAME_OVER;
        helpMenuCanvas.enabled = currentGameState == GameState.GS_HELPMENU;
    }

    public void AddKeys(Color newColor)
    {
        StartCoroutine(showKeyMessage(newColor));
        keysTab[keysFound].color = newColor;
        keysFound++;
    }

    public IEnumerator showKeyMessage(Color newColor)
    {
        string colorName = "";
        if (newColor == Color.yellow)
            colorName = "yellow";
        if (newColor == Color.red)
            colorName = "red";
        if (newColor == Color.blue)
            colorName = "blue";

        armorMessage.enabled = false;
        ammoMessage.enabled = false;
        secretMessage.enabled = false;
        keyMessage.enabled = false;
        keyMessage.text = "You picked up a " + colorName + " key";
        keyMessage.enabled = true;
        yield return new WaitForSeconds(2.0f);

        float elapsedTime = 0.0f;
        float fadeDuration = 0.5f;
        Color startColor = keyMessage.color;
        Color endColor = new Color(startColor.r, startColor.g, startColor.b, 0f);

        while (elapsedTime < fadeDuration)
        {
            keyMessage.color = Color.Lerp(startColor, endColor, elapsedTime / fadeDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }   
        keyMessage.enabled = false;
        keyMessage.color = startColor;
    }

    public IEnumerator showSecretMessage()
    {
        armorMessage.enabled = false;
        ammoMessage.enabled = false;
        secretMessage.enabled = false;
        keyMessage.enabled = false;
        secretMessage.enabled = true;
        yield return new WaitForSeconds(2.0f);

        float elapsedTime = 0.0f;
        float fadeDuration = 0.5f;
        Color startColor = secretMessage.color;
        Color endColor = new Color(startColor.r, startColor.g, startColor.b, 0f);

        while (elapsedTime < fadeDuration)
        {
            secretMessage.color = Color.Lerp(startColor, endColor, elapsedTime / fadeDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        secretMessage.enabled = false;
        secretMessage.color = startColor;
    }


    public IEnumerator showAmmoMessage()
    {
        armorMessage.enabled = false;
        ammoMessage.enabled = false;
        secretMessage.enabled = false;
        keyMessage.enabled = false;
        ammoMessage.enabled = true;
        yield return new WaitForSeconds(2.0f);

        float elapsedTime = 0.0f;
        float fadeDuration = 0.5f;
        Color startColor = ammoMessage.color;
        Color endColor = new Color(startColor.r, startColor.g, startColor.b, 0f);

        while (elapsedTime < fadeDuration)
        {
            ammoMessage.color = Color.Lerp(startColor, endColor, elapsedTime / fadeDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        ammoMessage.enabled = false;
        ammoMessage.color = startColor;
    }

    public IEnumerator showArmorMessage()
    {
        armorMessage.enabled = false;
        ammoMessage.enabled = false;
        secretMessage.enabled = false;
        keyMessage.enabled = false;
        infoMessage.enabled = false;

        armorMessage.enabled = true;
        yield return new WaitForSeconds(2.0f);

        float elapsedTime = 0.0f;
        float fadeDuration = 0.5f;
        Color startColor = armorMessage.color;
        Color endColor = new Color(startColor.r, startColor.g, startColor.b, 0f);

        while (elapsedTime < fadeDuration)
        {
            armorMessage.color = Color.Lerp(startColor, endColor, elapsedTime / fadeDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        armorMessage.enabled = false;
        armorMessage.color = startColor;
    }

    public IEnumerator showExitMessage()
    {
        armorMessage.enabled = false;
        ammoMessage.enabled = false;
        secretMessage.enabled = false;
        keyMessage.enabled = false;

        exitMessage.enabled = true;
        yield return new WaitForSeconds(2.0f);

        float elapsedTime = 0.0f;
        float fadeDuration = 0.5f;
        Color startColor = exitMessage.color;
        Color endColor = new Color(startColor.r, startColor.g, startColor.b, 0f);

        while (elapsedTime < fadeDuration)
        {
            exitMessage.color = Color.Lerp(startColor, endColor, elapsedTime / fadeDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        exitMessage.enabled = false;
        exitMessage.color = startColor;
    }

    public IEnumerator showExtraLifeMessage()
    {
        armorMessage.enabled = false;
        ammoMessage.enabled = false;
        secretMessage.enabled = false;
        keyMessage.enabled = false;
        exitMessage.enabled = false;

        extraLifeMessage.enabled = true;
        yield return new WaitForSeconds(2.0f);

        float elapsedTime = 0.0f;
        float fadeDuration = 0.5f;
        Color startColor = extraLifeMessage.color;
        Color endColor = new Color(startColor.r, startColor.g, startColor.b, 0f);

        while (elapsedTime < fadeDuration)
        {
            extraLifeMessage.color = Color.Lerp(startColor, endColor, elapsedTime / fadeDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        extraLifeMessage.enabled = false;
        extraLifeMessage.color = startColor;
    }

    public IEnumerator fadeInfoMessage()
    {
        yield return new WaitForSeconds(5.0f);

        float elapsedTime = 0.0f;
        float fadeDuration = 0.5f;
        Color startColor = infoMessage.color;
        Color endColor = new Color(startColor.r, startColor.g, startColor.b, 0f);

        while (elapsedTime < fadeDuration)
        {
            infoMessage.color = Color.Lerp(startColor, endColor, elapsedTime / fadeDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        infoMessage.enabled = false;
    }

    void Awake()
    {
        Instance = this;
        inGameCanvas.enabled = false;
        scoreText.text = score.ToString("0000");
        livesText.text = lives.ToString("0");
        ammoText.text = string.Format("Ammo: {00}", ammo);
        armorText.text = armor.ToString("0");
        secretText.text = string.Format("{0}/5", secretsFound);
        armorMessage.enabled = false;
        ammoMessage.enabled = false;
        secretMessage.enabled = false;
        keyMessage.enabled = false;
        exitMessage.enabled = false;
        extraLifeMessage.enabled = false;
        infoMessage.enabled = true;
        StartCoroutine(fadeInfoMessage());
        for (int i=0; i<keysTab.Length; i++)
        {
            keysTab[i].color = Color.gray;
        }
        SetVolume(0.5f);
        InGame();
        if (!PlayerPrefs.HasKey(keyHighScore))
            PlayerPrefs.SetInt(keyHighScore, 0); 
    }

    public void PauseMenu() { SetGameState(GameState.GS_PAUSEMENU); }
    public void InGame() { SetGameState(GameState.GS_GAME); }
    public void LevelCompleted() { SetGameState(GameState.GS_LEVELCOMPLETED); }
    public void GameOver() { SetGameState(GameState.GS_GAME_OVER); }

    public void Options() { SetGameState(GameState.GS_OPTIONS); }

    public void HelpMenu() { SetGameState(GameState.GS_HELPMENU); }

    public GameState getCurrGameState() { return currentGameState; }
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
        if (Input.GetKeyDown(KeyCode.F1))
        {
            if (currentGameState == GameState.GS_HELPMENU)
            {
                InGame();
            }

            else if (currentGameState == GameState.GS_GAME)
            {
                HelpMenu();
            }
        }  
        if(Input.GetKeyDown(KeyCode.Escape) && currentGameState == GameState.GS_HELPMENU)
            InGame();
        if (armorCooldown > 0)
            armorCooldown -= Time.deltaTime;
        if (armorCooldown < 0)
            armorCooldown = 0;
        timer += Time.deltaTime;
        int minutes = (int)(timer / 60);
        int seconds = (int)(timer % 60);
        timeText.text = string.Format("Time: {0:00}:{1:00}", minutes, seconds);
        optionsCanvas.enabled = currentGameState == GameState.GS_OPTIONS;
        pauseMenuCanvas.enabled = currentGameState == GameState.GS_PAUSEMENU;
        levelCompletedCanvas.enabled = currentGameState == GameState.GS_LEVELCOMPLETED;
        gameoverCanvas.enabled = currentGameState == GameState.GS_GAME_OVER;
        helpMenuCanvas.enabled = currentGameState == GameState.GS_HELPMENU;
    }
   }
}