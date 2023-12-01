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
    public void New()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
        SceneManager.LoadScene("Level1");
    }

    //Function to continue game
    public void Continue()
    {
        SceneManager.LoadScene("LevelSelect");
    }

    //Function to change scene to the instructions
    public void How2Play()
    {
        SceneManager.LoadScene("HowToPlay");
    }

    //Function to change scene to the credits for the game
    public void Credits()
    {
        SceneManager.LoadScene("Credits");
    }

    //Function to return to menu
    public void Menu()
    {
        SceneManager.LoadScene("Menu");
    }

    public void Select(string s)
    {
        SceneManager.LoadScene(s);
    }

}
