using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LaunchButton : MonoBehaviour
{    
    public PlayerList playerList;
    public Text text;

    public GameObject GameModeSelector;

    void Start(){
        GameModeSelector.SetActive(false);
    }

    public void onGameModeSelectButton(string gamemode){
        GameData.Gamemode = gamemode;
        SceneManager.LoadScene("InGame");
    }

    public void onLaunchButton(){
        if (playerList.players_added != 0){
            // if players in player list --> show the Game Mode selecor
            GameModeSelector.SetActive(true);
        }
        else {
            // if no players added to play list --> activate buffer (see update func)
            buffer_active = true;
        }
    }

    private bool buffer_active = false;
    private float buffer = 1.8f;
    void Update(){
        
        if (buffer_active && buffer > 0){
            // if buffer is active:
            
            // decreament by time.dt 
            buffer -= Time.deltaTime;

            // give the buffer message
            text.fontSize = 50;
            text.color = new Color(1 ,0, 0.2878246f, 1);
            text.text = "Add Player";
        }
        else {
            // if buffer is not active:
            // reset buffer
            buffer_active = false;  
            buffer = 0.8f;          

            // diplay regular button stuff
            text.fontSize = 70;
            text.color = Color.white;
            text.text = "Launch";
        }
    }
}