using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayCycle : MonoBehaviour
{
    // References
    public GameManager GM;

    // constant variables
    

    // global variables
    public bool gameOn;
    public float run_timer;
    
    
    void Update()
    {
        if (gameOn){
            




            // update run timer
            run_timer += Time.deltaTime;
        }
        else{


        }
    }
}
