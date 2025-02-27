using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    Inputs inputs;
    Rigidbody2D rb;
    PlayerID ID;

    public float curLookAngle; // cur = current

    public float xAxisInput;
    public float yAxisInput;


    void Awake()
    {
        inputs = GetComponent<Inputs>();
        ID = GetComponent<PlayerID>();
        rb = GetComponent<Rigidbody2D>();
    }

    public void setAxisInput(float x, float y)
    {
        xAxisInput = x;
        yAxisInput = y;
    }

    // Update is called once per frame
    void Update()
    {
        // Set drag
        rb.drag = ID.drag;

        /* look in direction of Left joystick - - - - - - - - - - - - - - - - - - */

        float xRotPoint;
        float yRotPoint;
        if (xAxisInput >= 0.1f || xAxisInput <= -0.1f || yAxisInput >= 0.1f || yAxisInput <= -0.1f)
        {
            xRotPoint = xAxisInput; // Max -> 1 and -1
            yRotPoint = yAxisInput; // Max -> 1 and -1

            float angle = Mathf.Atan2(xRotPoint, yRotPoint);
            angle *= Mathf.Rad2Deg; // Convert to degrees so it can be used in transform

            curLookAngle = angle;

            // slerp towards angle
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, 0, curLookAngle), ID.rotationSpeed / 100f);
        }

        /* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - */
    }

    public void Thrust()
    {
        if (ID.movement_enabled == false) 
            return;

        float radAngle = transform.eulerAngles.z * Mathf.Deg2Rad;

        float x = Mathf.Sin(radAngle) * -1f;
        float y = Mathf.Cos(radAngle);

        rb.AddForce(new Vector2(x, y) * ID.thrust);
        //rb.AddForce(new Vector2(xAxisInput * 10f,yAxisInput * -10f) / 10f);
    }
}
