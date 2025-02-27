using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayCycle : MonoBehaviour
{
    // References
    public GameManager GM;
    public InGameUI inGameUI;

    // constant variables
    

    // global variables
    public bool gameOn;
    public float run_timer;
    public float best_time = 0;
    
    
    void Update()
    {
        if (gameOn){
            // update run timer
            run_timer += Time.deltaTime;
        }
    }

    public void EndRun(){
        gameOn = false;

        // display end of run text 
        inGameUI.DisplayPrevRun(run_timer);

        // wait for a couple seconds and reset the following:
        //  - CPUs
        //  - Runtimer
        // TODO
        best_time = run_timer;
        run_timer = 0;
        GM.ResetMap();

        // allow for the player to respawn
    }

    public void StartRun(){
        gameOn = true;
    }
}
