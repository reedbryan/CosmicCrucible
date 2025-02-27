using UnityEngine;
using UnityEngine.Scripting;
using System.Collections.Generic;
using UnityEngine.UI; // For UI components

public class PlayerList : MonoBehaviour
{
    public GameObject NoPlayersText;
    public GameObject Player1;
    public GameObject Player2;
    public GameObject Player3;
    public GameObject Player4;
    public int players_added = 0;

    void Start(){
        NoPlayersText.SetActive(true);
        Player1.SetActive(false);
        Player2.SetActive(false);
        Player3.SetActive(false);
        Player4.SetActive(false);
    }
    
    public void AddToList(string input_type, Dictionary<string, float> feildData){
        NoPlayersText.SetActive(false);
        
        // generate stats text
        string stats = "";
        int index = 0;
        foreach (KeyValuePair<string, float> stat in feildData){
            index++;
            //Debug.Log("index: " + index);
            if (index == 4){
                index = 0;
                stats += "" + stat.Key + ": " + stat.Value.ToString() + "\n";
            } else if (index == 1){
                stats += stat.Key + ": " + stat.Value.ToString() + "    ";
            } else {
                stats += "" + stat.Key + ": " + stat.Value.ToString() + "    ";
            }
        }
        
        players_added++; // increment players added
        GameObject player;
        switch (players_added)
        {
            case 1:
                player = Player1;
                break;

            case 2:
                player = Player2;
                break;

            case 3:
                player = Player3;
                break;

            case 4:
                player = Player4;
                break;

            default:
                player = null;
                Debug.Log("BROKEN");
                break;
        }
        player.transform.GetChild(0).transform.GetChild(0).GetComponent<Text>().text += input_type; // title text
        player.transform.GetChild(0).transform.GetChild(1).GetComponent<Text>().text = stats;       // stats text
        player.SetActive(true);

    }
}