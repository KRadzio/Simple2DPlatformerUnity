using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void OnLevel1ButtonPressed()
    {
        SceneManager.LoadScene("Level1");
    }

    public void OnExitToDesktopButtonPressed()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
        Application.Quit();
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
