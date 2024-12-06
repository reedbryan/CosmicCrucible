using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerID : MonoBehaviour
{
    public int playerNumber;
    public int controllerNumber;
    GameManager gm;

    /* Player stats */

    public float timeAlive;

    // Physics
    public float thrust;
    public float drag;
    public float rotationSpeed;

    // Primary fire
    public float primaryFireRate;
    public float primaryFirePower;
    public float primaryFireForce;
    public float primaryFireDrag;
    public float primaryFireSize;
    public float primaryFireLifeTime;
    public float primaryFireSpread;
    public int primaryFireCount;
    public float primaryFireMass;

    // Missile
    public float missileCooldown;

    // Ability one
    public float boostCooldown;


    private void Awake()
    {
        // Get game manager script
        GameObject gmOb = GameObject.Find("GameManager");
        gm = gmOb.GetComponent<GameManager>();

        timeAlive = 0;
    }

    void Update(){
        timeAlive += Time.deltaTime;
    }
}
