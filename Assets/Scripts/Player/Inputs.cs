using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inputs : MonoBehaviour
{
    PlayerID ID; // The ID of the player this script is attached to
    PlayerMovement movement;
    PlayerAbilities abilities;
    PlayerGraphics graphics;

    public bool usingController;

    public float xAxisInput;
    public float yAxisInput;

    // Debuging
    //[SerializeField] float x;
    //[SerializeField] float y;

    // Hold input booleans
    bool B5down = false;


    private void Awake()
    {
        ID = GetComponent<PlayerID>();
        movement = GetComponent<PlayerMovement>();
        abilities = GetComponent<PlayerAbilities>();

        GameObject graphicsGameOb = transform.GetChild(2).gameObject;
        graphics = graphicsGameOb.GetComponent<PlayerGraphics>();
    }

    // Update is called once per frame
    void Update()
    {
        if (usingController)
        {
            ControllerInputs();
        }
        else
        {
            KeyBoardInputs();
        }
        
    }

    void KeyBoardInputs()
    {
        Vector2 screenPosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        Vector2 worldPosition = Camera.main.ScreenToWorldPoint(screenPosition);
        Vector2 axis = ((Vector2)transform.position - worldPosition).normalized;

        movement.setAxisInput(axis.x, axis.y * -1);

        // Forward boost movement
        if (Input.GetMouseButton(0)) // Left click
        {
            movement.Thrust();
            graphics.jetParticules();
        }

        // Primary fire
        if (Input.GetMouseButton(1) || Input.GetKey(KeyCode.Space)) // Right click or space (if using trackpad)
        {
            abilities.request("PrimaryFire");
        }

        // Boost
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            abilities.request("Boost");
        }

        // Missile
        if (Input.GetKeyDown(KeyCode.F))
        {
            abilities.request("Missile");
        }

    }

    void ControllerInputs()
    {
        // L joystick inputs
        float x = Input.GetAxisRaw("J" + ID.controllerNumber + "Horizontal");
        float y = Input.GetAxisRaw("J" + ID.controllerNumber + "Vertical");

        movement.setAxisInput(x * -1, y * -1);

        if (Input.GetButtonDown("J" + ID.controllerNumber + "B0")) // button #1 on the controller
        {
            // TBD
        }


        if (Input.GetButtonDown("J" + ID.controllerNumber + "B1")) // button #2 on the controller
        {
            // TBD
        }


        if (Input.GetButtonDown("J" + ID.controllerNumber + "B2")) // button #3 on the controller
        {
            // TBD
        }


        if (Input.GetButtonDown("J" + ID.controllerNumber + "B3")) // button #4 on the controller
        {
            // TBD
        }

        // Fire missile (not permanent)
        if (Input.GetButtonDown("J" + ID.controllerNumber + "B4")) // button #5 on the controller (L1)
        {
            abilities.request("Missile");
        }


        // Primary fire 
        if (Input.GetButton("J" + ID.controllerNumber + "B5")) // button #6 on the controller (R1)
        {
            abilities.request("PrimaryFire");
        }

        // Ability one
        if (Input.GetButtonDown("J" + ID.controllerNumber + "B6")) // button #7 on the controller (L2)
        {
            abilities.request("Boost");
        }


        // Forward boost movement
        if (Input.GetButton("J" + ID.controllerNumber + "B7")) // button #8 on the controller (R2)
        {
            movement.Thrust();
            graphics.jetParticules();
        }


        if (Input.GetButtonDown("J" + ID.controllerNumber + "B8")) // button #9 on the controller
        {
            // TBD
        }


        if (Input.GetButtonDown("J" + ID.controllerNumber + "B9")) // button #10 on the controller
        {
            // TBD
        }


        if (Input.GetButtonDown("J" + ID.controllerNumber + "B10")) // Left joystick push on the controller
        {
            // TBD
        }


        if (Input.GetButtonDown("J" + ID.controllerNumber + "B11")) //  Right joystick push on the controller
        {
            // TBD
        }


        // Input testing
        /*
        for (int i = 0; i <= 15; i++)
        {
            if (Input.GetButtonDown("J" + controlerNumber + "B" + i))
            {
                Debug.Log("J" + controlerNumber + "B" + i);
            }
        }
        */
    }
}
