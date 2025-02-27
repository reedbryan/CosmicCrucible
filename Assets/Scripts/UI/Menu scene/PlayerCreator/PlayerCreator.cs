using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCreator : MonoBehaviour
{
    public GameObject PlayerListDisplay;
    public GameObject InputFields;
    public Dropdown InputTypeDropdown;
    public Text budgetText;
    public int budget;
    private int controller_players_counter = 0;
    
    // Start is called before the first frame update
    void Start()
    {
        //PlayerData.testingData = "BRUH THIS TOO EASY";

        // Configure UI components
        PlayerListDisplay.SetActive(true);
        InputFields.SetActive(false);
        UpdateBudget(0);
    }

    public void UpdateBudget(int change){
        Debug.Log("Buget change: " + change);
        budget+=change;
        budgetText.text = "Budget: $" + budget;
    }

    public void OnAddPlayerButton(){
        //Debug.Log("OnAddPlayerButton");
        
        // hide the add player button
        PlayerListDisplay.SetActive(false);

        // show the input feilds
        InputFields.SetActive(true);
    }

    public void OnConfirmPlayerButton(){
        //Debug.Log("OnConfirmPlayerButton");
        
        // show the add player button
        PlayerListDisplay.SetActive(true);

        // hide the input feilds
        InputFields.SetActive(false);
        


        // Gather information from InputFields data:
        Dictionary<string, float> inputFieldData = CollectInputFieldData();

        PlayerList playerList = PlayerListDisplay.GetComponent<PlayerList>();
        inputFieldData["Player Number"] = playerList.players_added+1;

        // Upload the data to the static "PlayerData" variable:
        if (InputTypeDropdown.value == 0 && PlayerData.keyboard_player == null){ 
            // if keyboard input selected:
            //  - set the data to keyboard player 
            PlayerData.keyboard_player = inputFieldData; 
            
            //  - remove keyboard option from the dropdown
            InputTypeDropdown.options.RemoveAt(0);
            InputTypeDropdown.gameObject.transform.GetChild(0).GetComponent<Text>().text = "Controller";

            // update player list display
            PlayerListDisplay.GetComponent<PlayerList>().AddToList("Keyboard",  inputFieldData);

            Debug.Log("Keyboard Input selected");
            
        }
        else{
            // if controller input seslected:
            //  - check that there are less then 3 controller players
            if (controller_players_counter > 2){
                Debug.Log("MAX CONTROLLER PLAYERS REACHED");
            }
            else {
                //  - Ensure the static list is initialized
                if (PlayerData.controller_players == null)
                {
                    PlayerData.controller_players = new List<Dictionary<string, float>>();
                }

                // Add the collected data to the list
                PlayerData.controller_players.Add(inputFieldData);

                controller_players_counter++; // increment controller_player_counter

                // update player list display
                PlayerListDisplay.GetComponent<PlayerList>().AddToList("Controller",  inputFieldData);    

                Debug.Log("Controller Input selected");
            }
        }
    }

    public Dictionary<string, float> CollectInputFieldData()
    {
        // Create the dictionary to store the results
        Dictionary<string, float> data = new Dictionary<string, float>();

        // Iterate through all children of the parent GameObject
        foreach (Transform child in InputFields.transform)
        {            
            // Check if the child has the InputFieldInput tag
            if (child.CompareTag("InputFieldInput"))
            {
                // Add the data to the dictionary with its name as the key
                data[child.name] = child.GetComponent<CounterController>().current_value;
            }
        }

        return data;
    }
}
