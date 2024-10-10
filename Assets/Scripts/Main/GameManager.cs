using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    /// <summary>
    /// PlayerList is an array of PlayerID held on child gameobjects of the game manager. These gameobjects act as profiles for the players in the game and are not destroyed when the player
    /// dies but instead will serve as blue prints to respawn.
    /// </summary>
    public List<PlayerID> playerList;
    /// <summary>
    /// gamePlayerList is an array of PlayerIDs held on the players. These are what is referenced by other player scripts. They are
    /// created when the player is spawned in and if it is a respawn the playerList blueprint will be copied on to this. The elements
    /// in this list need to be acurate as the camera tracks all players in this list.
    /// </summary>
    public List<PlayerID> inGamePlayerList;
    /// <summary>
    /// All the controller numbers that HAVE NOT yet been assigned to players
    /// </summary>
    public List<int> controllerList = new List<int>();

    // prefabs
    public GameObject playerPrefab;
    public GameObject CPUPrefab;
    public GameObject empty;

    private void Awake()
    {
        int max = 3; // max is the amount of controllers

        // controllerList = [0, 1, 2, 3] (respective controller numbers)
        for (int i = 0; i <= max; i++) 
        {
            controllerList.Add(i);
        }
    }

    private void Update()
    {
        // CAPPING FRAME RATE (IMPORTANT!!)
        Application.targetFrameRate = 60;

        newPlayerCheck();

        newCPUCheck();

        /* - - Test controller number - - */
        if (Input.GetButtonDown("J1B0"))
        {
            Debug.Log("Controller 1");
        }
        if (Input.GetButtonDown("J2B0"))
        {
            Debug.Log("Controller 2");
        }
        /* - - - - - - - - - - - - - - - - */
    }

    void newCPUCheck()
    {
        // Input check (if n key pressed -> spawn cpu)
        if (Input.GetKeyDown("n"))
        {
            spawnCPU();
        }
    }
    public void spawnCPU(){
        GameObject newCPU = spawnPlayer(CPUPrefab);
        newCPU.GetComponent<PlayerID>().playerNumber = -1;
    }

    void newPlayerCheck()
    {
        // Get input from controlers who are not assigned to players
        List<int> controllersToRemouve = new List<int>(); // temp list
        bool gotInput = false;
        foreach (var controllerNumber in controllerList)
        {
            // NOTE: The controllerNumber 0 means that the player is using the keyboard

            if (controllerNumber != 0)
            {
                // Controller check
                if (Input.GetButtonDown("J" + controllerNumber + "B1") && !gotInput)
                {
                    addPlayer(controllerNumber, playerList.Count + 1);
                    controllersToRemouve.Add(controllerNumber);
                    gotInput = true;
                }
            }
            else
            {
                // Keyboard check
                if (Input.GetKeyDown(KeyCode.Space) && !gotInput)
                {
                    addPlayer(0, playerList.Count + 1); // Controllernumber 0 = keyboard
                    controllersToRemouve.Add(controllerNumber);
                    gotInput = true;
                }
            }
        }

        foreach (var item in controllersToRemouve)
        {
            controllerList.Remove(item);
        }
    }

    /// <summary>
    /// Creates new profile on gm.
    /// </summary>
    /// <param name="controllerNumber"></param>
    /// <param name="playerNumber"></param>
    public void addPlayer(int controllerNumber, int playerNumber)
    {
        // create new player and the playerID and save the ID
        GameObject nPlayer;

        if (playerList.Count > 0)
        {
            PlayerID match = null;
            bool matchFound = false;

            //Debug.Log("controllerNumber: " + controllerNumber);

            foreach (var profile in playerList)
            {
                //Debug.Log("profile.controllerNumbers: " + profile.controllerNumber);
                if (profile.controllerNumber == controllerNumber)
                {
                    if (!matchFound)
                    {
                        match = profile;
                        matchFound = true;
                    }
                }
                else
                {
                    if (!matchFound)
                        match = null;
                }
            }

            // If the player does not have a profile on the gm and therefore IS SPAWNING FOR THE FIRST TIME.
            if (match == null)
            {
                Debug.Log("Did not find match");

                nPlayer = spawnPlayer(playerPrefab);
                createPlayerProfile(nPlayer, playerNumber, controllerNumber);
                assignPlayerValues(controllerNumber, playerNumber, nPlayer, false);
            }
            // If the player does have a profile on the gm and therefor is NOT SPAWNING FOR THE FIRST TIME.
            else
            {
                Debug.Log("Found match");

                nPlayer = spawnPlayer(match.gameObject);
                assignPlayerValues(controllerNumber, nPlayer.GetComponent<PlayerID>().playerNumber, nPlayer, false);
            }
        }
        // If no players have been spawn so for and therefore this IS SPAWNING FOR THE FIRST TIME.
        else
        {
            Debug.Log("No players yet");

            nPlayer = spawnPlayer(playerPrefab);
            createPlayerProfile(nPlayer, playerNumber, controllerNumber);
            assignPlayerValues(controllerNumber, playerNumber, nPlayer, false);
        }
    }

    GameObject spawnPlayer(GameObject bluePrint)
    {
        GameObject nPlayer;

        if (inGamePlayerList.Count > 0)
        {
            // Get average distance between players
            Vector2 avPos = Vector2.zero;
            foreach (var ID in inGamePlayerList)
            {
                avPos += new Vector2(ID.transform.position.x + Random.Range(-10f, 10f),
                                     ID.transform.position.y + Random.Range(-10f, 10f));
            }

            // Spawn new player at the above position
            nPlayer = Instantiate(bluePrint, avPos / inGamePlayerList.Count, transform.rotation);
        }
        else
        {
            // Spawn new player at 0,0,0
            nPlayer = Instantiate(bluePrint, Vector3.zero, transform.rotation);
        }

        inGamePlayerList.Add(nPlayer.GetComponent<PlayerID>());
        nPlayer.SetActive(true);
        return nPlayer;
    }

    void assignPlayerValues(int controllerNumber, int playerNumber, GameObject player, bool isProfile)
    {
        PlayerID ID = player.GetComponent<PlayerID>();

        // add ID to inGamePlayerList and set new player to a controler/keyboard
        ID.playerNumber = playerNumber;
        ID.controllerNumber = controllerNumber;

        if (isProfile)
            player.name = "player" + playerNumber + "Backup";
        else
            player.name = "Player" + playerNumber;

        Inputs nIn = player.GetComponent<Inputs>();
        if (controllerNumber == 0)
        {
            nIn.usingController = false;
        }
        else
        {
            nIn.usingController = true;
        }

        //Debug.Log("Controller number: " + controllerNumber);
        //Debug.Log("Player added");
    }

    void createPlayerProfile(GameObject player, int playerNumber, int controllerNumber)
    {
        //Debug.Log("Created prof");

        // Instantiate a clone of the player
        GameObject anchor = Instantiate(player);

        // Parent the clone to the Game Manager gameObject and name it
        anchor.transform.parent = gameObject.transform; 

        // Assign values
        assignPlayerValues(controllerNumber, playerNumber, anchor, true);

        // add ID to playerlist
        playerList.Add(anchor.GetComponent<PlayerID>());

        // Set the clone to notactive so it wont interact with the other gameObs in the environment
        anchor.SetActive(false);

        /*  Old code:
        // Remove all children from the clone (healthbar, jets PS, etc)
        for (int i = 0; i < anchor.transform.childCount; i++)
        {
            Destroy(anchor.transform.GetChild(i).gameObject);
        }

        // Remove all non-playerID components from the clone
        foreach (var component in anchor.GetComponents<Component>()) 
        {
            if (component != anchor.GetComponent<PlayerID>() && component != anchor.transform)
                Destroy(component);
        }
        */


        /*
         * The clone now serves as the profile for the assigned player. It is attached
         * to the game manager among the other profiles named "player(x)Backup". These profiles
         * are only to be used as blueprint when respawning the assigned player.
         */
    }

    /// <summary>
    /// Remove a player from the inGamePlayer list. This should be called when a player dies.
    /// </summary>
    /// <param name="ID"></param>
    public void removeInGamePlayer(PlayerID ID)
    {
        //Debug.Log("Removed player");

        inGamePlayerList.Remove(ID);

        // Run through all ints in controllerlist
        foreach (var number in controllerList)
        {
            // If the controllerNumber you are trying to add is already in the list then don't add it
            if (number == ID.controllerNumber)
            {
                return;
            }
        }

        controllerList.Add(ID.controllerNumber);
    }
}