using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class InGameUI : MonoBehaviour
{
    public GameManager GM;
    public GameplayCycle GameplayCycle;
    public Canvas canvas;

    private void Start () {
        if (GameData.Gamemode == "survival"){
            InvokeRepeating("Update_Run_Timer", 1f, 1f);  //1s delay, repeat every 1s (for runtime timer)
        }
        else {
            Run_Timer_Text.text = "Press n to spawn CPU";
        }
    }
    
    private void Update()
    {
        Update_Spawn_Instructions();
        Update_Player_List();
    }

    public Text Player_List_Text;
    public Text Run_Timer_Text;
    private int local_player_count;

    void Update_Run_Timer(){  
        
        // Clamp the value to a maximum of 999
        int clampedValue = Mathf.Clamp(Mathf.FloorToInt(GameplayCycle.run_timer), 0, 999);

        // Convert to string with leading zeros
        string formated_runtime_value = clampedValue.ToString("D3");
        Run_Timer_Text.text = "Run time: " + formated_runtime_value;
    }
    void Update_Player_List(){
        
        // Check for change in GM.inGamePlayerList - if not then return
        int actual_player_count = GM.inGamePlayerList.Count;
        if (GM.inGamePlayerList.Count == local_player_count){
            return;
        }
        local_player_count = actual_player_count;

        // If change in GM.inGamePlayerList:
        // Update the player info
        int lastNum = 1000000;
        string player_list_str = "";
        foreach (PlayerID player in GM.playerList){
            string status = "Dead";
            
            foreach (PlayerID pID in GM.inGamePlayerList){
                if (pID.playerNumber == player.playerNumber){
                    status = "Alive";
                }
            }
            
            if (player.playerNumber <= lastNum){
                player_list_str = "Player " + player.playerNumber + ": " + status + "\n" + player_list_str;
            }
        }
        
        int botCount = 0;
        foreach (PlayerID player in GM.inGamePlayerList){
            if (player.playerNumber == -1){
                botCount++;
            }
        }
        player_list_str = player_list_str + "CPUs: " + botCount;

        Player_List_Text.text = player_list_str;
    }


    public Text Spawn_Instructions_Text;
    void Update_Spawn_Instructions(){
        if (GM.inGamePlayerList.Count <= 0){
            Spawn_Instructions_Text.enabled = true;
        }
        else Spawn_Instructions_Text.enabled = false;
    }

    public void DisplayPrevRun(float run_time){
        // Clamp the value to a maximum of 999
        int clampedValue = Mathf.Clamp(Mathf.FloorToInt(run_time), 0, 999);

        // Convert to string with leading zeros
        string formated_runtime_value = clampedValue.ToString("D3");
        
        Spawn_Instructions_Text.text = "Previous Run Time: " + formated_runtime_value + "\nPress 'space' or 'x' to start a new run";
    }

    public void OnExitButton(){
        SceneManager.LoadScene("Menu");
    }


    public GameObject DamageMarkerPrefab; // Input in inspector

    /// <summary>
    /// Should be called from damageIntake script whenever a gameObject takes damage.
    /// </summary>
    /// <param name="amount"></param>
    /// <param name="location"></param>
    public void showDamage(float damage, Vector2 location)
    {
        GameObject dmOb = Instantiate(DamageMarkerPrefab, location, transform.rotation);
        DamageMarker dm = dmOb.transform.GetChild(0).GetChild(0).GetComponent<DamageMarker>();

        dm.sendInfo(damage);
    }
}
