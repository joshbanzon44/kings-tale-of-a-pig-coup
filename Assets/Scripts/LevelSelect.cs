using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelect : MonoBehaviour
{
    
    public List<Button> buttons;
    private int highestLevelCompleted;

    // Start is called before the first frame update
    void Start()
    {
        highestLevelCompleted = PlayerPrefs.GetInt("LevelNum");

        for (int i = buttons.Count - 1; i >= highestLevelCompleted; i--) 
        {
            buttons[i].interactable = false;
        }
    }
}
