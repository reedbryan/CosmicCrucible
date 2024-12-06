using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// YOYO WHAS GOOD

public class GameManager : MonoBehaviour
{
    // Global variables - - - - - - - - - - - - - - - - - - -
    
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


    public float cpu_spawntime_interval;
    private float cpu_spawntimer;
    private float cpu_count = 0;


    // references - - - - - - - - - - - - - - - - - - - - - - -
    public GameObject playerPrefab;
    public GameObject CPUPrefab;
    public GameObject empty;
    public Camera Cam;
     public GameplayCycle gameplayCycle;

    private void Awake()
    {
        // set cpu timer
        cpu_spawntimer = cpu_spawntime_interval;
        
        int max = 4; // max is the amount of controllers
        max--; 

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

        // cpu spawn routine
        if (gameplayCycle.gameOn){
            cpu_spawntimer -= Time.deltaTime;
            if (cpu_spawntimer < 0){
                if (cpu_count <= 3){
                    spawnCPU();
                    cpu_spawntimer = cpu_spawntime_interval + cpu_spawntime_interval * cpu_count;
                }
            }
        }

        testControllerInput();
    }


    void testControllerInput(){
        /* - - Test controller number - - */
        if (Input.GetButtonDown("J1B0"))
        {
            Debug.Log("Controller 1");
        }
        if (Input.GetButtonDown("J2B0"))
        {
            Debug.Log("Controller 2");
        }
        if (Input.GetButtonDown("J3B0"))
        {
            Debug.Log("Controller 3");
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
        GameObject newCPU = spawnPlayer(CPUPrefab, true);
        newCPU.GetComponent<PlayerID>().playerNumber = -1;
        cpu_count++;
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
        /*  if this is the first player entering the game
            then toggle the gameOn global variable */  
        if (gameplayCycle.gameOn == false){
            gameplayCycle.gameOn = true;
        }
        
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

                nPlayer = spawnPlayer(playerPrefab, false);
                createPlayerProfile(nPlayer, playerNumber, controllerNumber);
                assignPlayerValues(controllerNumber, playerNumber, nPlayer, false);
            }
            // If the player does have a profile on the gm and therefor is NOT SPAWNING FOR THE FIRST TIME.
            else
            {
                Debug.Log("Found match");

                nPlayer = spawnPlayer(match.gameObject, false);
                assignPlayerValues(controllerNumber, nPlayer.GetComponent<PlayerID>().playerNumber, nPlayer, false);
            }
        }
        // If no players have been spawn so for and therefore this IS SPAWNING FOR THE FIRST TIME.
        else
        {
            Debug.Log("No players yet");

            nPlayer = spawnPlayer(playerPrefab, false);
            createPlayerProfile(nPlayer, playerNumber, controllerNumber);
            assignPlayerValues(controllerNumber, playerNumber, nPlayer, false);
        }
    }

    GameObject spawnPlayer(GameObject bluePrint, bool isCPU)
    {
        GameObject nPlayer;

        if (inGamePlayerList.Count > 0)
        {
            // Get average distance between players
            Vector2 avPos = Vector2.zero;
            foreach (var ID in inGamePlayerList)
            {
                avPos += new Vector2(ID.transform.position.x + UnityEngine.Random.Range(-10f, 10f),
                                    ID.transform.position.y + UnityEngine.Random.Range(-10f, 10f));
            }
            avPos = avPos / inGamePlayerList.Count;

            // get a location inside of camera view, but away from the player
            Vector2 buffer = new Vector2(Cam.orthographicSize * 1.85f, Cam.orthographicSize);
            buffer *= UnityEngine.Random.Range(0.1f, 0.5f);
            avPos += buffer;

            // Spawn new player at the above position
            nPlayer = Instantiate(bluePrint, avPos, transform.rotation);
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