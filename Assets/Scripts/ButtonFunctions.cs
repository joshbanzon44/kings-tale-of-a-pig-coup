using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonFunctions : MonoBehaviour
{
    //Function to quit game on button click
    public void Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
Application.Quit();
#endif
    }

    //Function to start new game, erase player data
    public void New(string sceneName)
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
        SceneManager.LoadScene(sceneName);
    }

    //Function to continue game
    public void Continue()
    {

    }

    //Function to change scene to the instructions
    public void How2Play(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    //Function to change scene to the credits for the game
    public void Credits(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

}
