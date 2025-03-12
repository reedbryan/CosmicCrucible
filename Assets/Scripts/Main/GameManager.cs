using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.Events;



public class GameManager : MonoBehaviour
{
    // Global player lists - - - - - - - - - - - - - - - - - - - - - -
    
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

    // cpu spawning values - - - - - - - - - - - - - - - - - - - - - - -
    [SerializeField]private float cpu_spawntime_interval;
    [SerializeField]private float cpu_spawntimer;
    [SerializeField]private float cpu_count = 0;
    [SerializeField]private int max_cpu_count;
    [SerializeField]private int cpu_spawning_level = 1;


    // references - - - - - - - - - - - - - - - - - - - - - - - - - - - -
    public GameObject playerPrefab;
    public GameObject CPUPrefab;
    public GameObject empty;
    public Camera Cam;
    public GameplayCycle GC;

    public UnityEvent onResetMap;

    public void ResetMap()
    {
        onResetMap.Invoke();
    }
    
    private void Awake()
    {
        config_CPU_spanwing_values();

        // controllerList = [0, 1, 2, 3] (respective controller numbers)
        for (int i = 0; i <= 3; i++) 
        {
            controllerList.Add(i);
        }
    }

    void config_CPU_spanwing_values(){
        if (GameData.Gamemode == "survival"){
            // get a player count
            int player_count = 0;
            if (PlayerData.keyboard_player != null) player_count++;
            if (PlayerData.controller_players != null) player_count += PlayerData.controller_players.Count;
            
            // set cpu timer
            cpu_spawntime_interval = 10 / player_count;
            cpu_spawntimer = 3; // 3 second buffer before first CPU spawn

            // set cpu count
            max_cpu_count = 2 + (int)(player_count * 1.5f);
        }
        else if (GameData.Gamemode == "sandbox"){}
        else Debug.Log("ERROR: GameData.Gamemode not assigned");
    }

    private void Update()
    {
        // CAPPING FRAME RATE (IMPORTANT!!)
        Application.targetFrameRate = 60;

        newPlayerCheck();
        CPU_spawn_Routine();

        testControllerInput();
    }

    private void CPU_spawn_Routine(){
        // SURVIVAL routine
        if (GameData.Gamemode == "survival"){
            // cpu spawn routine
            if (GC.gameOn && cpu_count < max_cpu_count){
                cpu_spawntimer -= Time.deltaTime;
                if (cpu_spawntimer < 0){
                    spawnCPU();
                    
                    // calculate the cpu spawning level
                    cpu_spawning_level = (int)GC.run_timer / 20;
                    if (cpu_spawning_level > 10) cpu_spawning_level = 10;
                    if (cpu_spawning_level < 1) cpu_spawning_level = 1;

                    // re-assign the spawn timer based on the spawning level
                    cpu_spawntimer = cpu_spawntime_interval / cpu_spawning_level;
                }
            }
        }

        // SANDBOX routine
        else if (GameData.Gamemode == "sandbox"){
            // Input check (if n key pressed -> spawn cpu)
            if (Input.GetKeyDown("n"))
            {
                spawnCPU();
            }
        }


        else Debug.Log("ERROR: GameData.Gamemode not assigned");
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

    public void spawnCPU(){
        spawnPlayer(CPUPrefab, true);
        Debug.Log("cpu_count: " + cpu_count);
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
                //Debug.Log("CONTROLLER#" + controllerNumber);
                
                // Controller check
                if (Input.GetButtonDown("J" + controllerNumber + "B1") && !gotInput && PlayerData.controller_players != null)
                {
                    Debug.Log("CONTROLLER#" + controllerNumber + " HIT");
                    
                    addPlayer(controllerNumber);
                    controllersToRemouve.Add(controllerNumber);
                    gotInput = true;
                }
            }
            else
            {
                //Debug.Log("KEYBOARD");
                // Keyboard check
                if (Input.GetKeyDown(KeyCode.Space) && !gotInput && PlayerData.keyboard_player != null)
                {
                    Debug.Log("KEYBOARD HIT");
                    addPlayer(controllerNumber); // Controllernumber 0 = keyboard
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

    void spawnPlayer(GameObject profile, bool isCPU)
    {
        GameObject nPlayer;
        Vector2 pos;

        string name;

        // get a position to spawn from
        if (!isCPU){
            // if spawning a new REAL player:
            
            // if this isn't the first real player:
            if (playerList.Count > 1){
                // get the average position between players
                pos = Vector2.zero;
                foreach (var player in playerList)
                {
                    pos += new Vector2(player.transform.position.x + UnityEngine.Random.Range(-10f, 10f)
                                        ,player.transform.position.y + UnityEngine.Random.Range(-10f, 10f));
                }
                pos = pos / playerList.Count;
            }
            // if it is the first real player:
            else {
                // set position to 0,0,0
                pos = Vector2.zero;
            }

            // name the player
            name = "player#" + profile.GetComponent<PlayerID>().playerNumber;
        }
        else{
            // if spawning a new CPU
            // get a position withen cameraview
            float buffer = Cam.orthographicSize;
            pos = Cam.transform.position + new Vector3(buffer * UnityEngine.Random.Range(-1f,1f)
                                                              ,buffer * UnityEngine.Random.Range(-1f,1f));
            
            // name the CPU
            name = "CPU";
        }

        nPlayer = Instantiate(profile, pos, transform.rotation);    // create player from profile
        nPlayer.name = name;                                        // name player
        inGamePlayerList.Add(nPlayer.GetComponent<PlayerID>());     // add player to in game players list
        nPlayer.SetActive(true);                                    // set the player to active
    }

    /// <summary>
    /// Creates new profile on gm.
    /// </summary>
    /// <param name="controllerNumber"></param>
    /// <param name="playerNumber"></param>
    public void addPlayer(int controllerNumber)
    {        
        /*  if this is the first player entering the game
            then toggle the gameOn global variable */  
        if (GC.gameOn == false){
            GC.StartRun();
        }


        // Check if this profile exists in the player list
        // if it does that means the player isn't new but is just respawning
        PlayerID match = null;
        bool matchFound = false;
        foreach (var profile in playerList)
        {
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

            GameObject profile = createPlayerProfile(controllerNumber);
            spawnPlayer(profile, false);
        }
        // If the player does have a profile on the gm and therefor is NOT SPAWNING FOR THE FIRST TIME.
        else
        {
            Debug.Log("Found match");

            spawnPlayer(match.gameObject, false);
        }
    }
    
    GameObject createPlayerProfile(int controllerNumber)
    {        
        //Debug.Log("createPlayerProfile");
        
        // Instantiate a basic player prefab
        // This will act as the player's profile (data to be used when respawning the player)
        GameObject profile = Instantiate(playerPrefab);
        PlayerID ID = profile.GetComponent<PlayerID>();

        //Debug.Log("1");

        // get correct player data
        Dictionary<string, float> playerData;
        //Debug.Log("1.1");
        if (controllerNumber == 0){
            // if keyboard player
            playerData = PlayerData.keyboard_player;
            //Debug.Log("1.1.1");
        }
        else {
            // if controller player
            if (PlayerData.controller_players[controllerNumber-1] == null){
                //Debug.Log("CONTROLLER NUMBER GREATER THAN PLAYERS ADDED, FIX THIS SHIT"); // FIXXXXXXXXXX
                return null;
            }
            else {
                playerData = PlayerData.controller_players[controllerNumber-1];
                //Debug.Log("1.1.2");
            }
        }
        
        //Debug.Log("2");

        // assign the player data to the profile: - - - - - - - - - - - - - - - - - - - - - - - - - - - -
        
        // admin
        ID.playerNumber = (int)playerData["Player Number"];
        ID.controllerNumber = controllerNumber;
        profile.name = "player" + ID.playerNumber + "Backup";

        // movement
        ID.thrust = playerData["thrust"];
        ID.drag = playerData["drag"];
        ID.rotationSpeed = playerData["rotation speed"];

        // primary fire
        ID.primaryFireRate = playerData["fire rate"];
        ID.primaryFirePower = playerData["damage"];
        ID.primaryFireSpread = playerData["accuracy"];
        ID.primaryFireCount = (int)playerData["count"];
        ID.primaryFireForce = playerData["speed"];

        // ability cooldowns
        ID.missileCooldown = playerData["missile cooldown"];
        ID.boostCooldown = playerData["boost cooldown"];

        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

        //Debug.Log("3");

        // configure inputs
        Inputs inputs = profile.GetComponent<Inputs>();
        if (controllerNumber == 0)
            inputs.usingController = false;
        else
            inputs.usingController = true;

        //Debug.Log("4");
        
        // Parent the clone to the Game Manager gameObject and name it
        profile.transform.parent = gameObject.transform; 

        //Debug.Log("5");

        // add ID to playerlist
        playerList.Add(ID);

        //Debug.Log("6");

        // Set the clone to notactive so it wont interact with the other gameObs in the environment
        profile.SetActive(false);
        
        //Debug.Log("7");

        return profile;

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

        
        if (ID.controllerNumber != -1){ // CPU's are set to -1
            // check if all players are dead
            bool player_alive_flag = false;
            foreach (var player in inGamePlayerList){
                if (player.controllerNumber != -1){
                    // player alive
                    player_alive_flag = true;
                }
            }
            if (player_alive_flag == false){
                GC.EndRun();
                Debug.Log("End Run");
            }
            
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
        else{
            // if CPU decrment the CPU counter
            cpu_count--;
        }
    }
}