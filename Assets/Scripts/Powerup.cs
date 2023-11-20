using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Powerup : MonoBehaviour
{


    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Switch statement depending on tag of object touched
        switch (collision.gameObject.tag)
        {
            case "Player":
                

                break;
            
            default:
                break;
        }
    }

}
