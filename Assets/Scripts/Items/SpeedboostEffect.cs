using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedboostEffect : MonoBehaviour
{
    PlayerID ID; // Player that collides with item
    //GameObject container; // Game object that is anchoring this script

    // Physics
    [SerializeField] float thrustMul;
    [SerializeField] float dragMul;
    [SerializeField] float rotationSpeedMul;

    // Effect properties
    bool effectActive = false;
    [SerializeField] float effectDuration;

    /// <summary>
    /// This should only be called after effect script is attached to the player
    /// </summary>
    public void triggerEffect() 
    {
        Debug.Log("Effect triggered");

        // Assign values
        ID = transform.parent.GetComponent<PlayerID>();

        ID.rotationSpeed *= rotationSpeedMul;
        ID.drag *= dragMul;
        ID.thrust *= thrustMul;

        // activated timer
        effectActive = true;
    }

    private void Update()
    {
        if (!effectActive)
        {
            // Check if the parent of this game object is the player (this means the effect should be active)
            if (transform.parent.CompareTag("Player"))
                triggerEffect();
        }
        // While effect is active
        // Count down the timer and if it becomes 0 destroy the script
        else
        {
            effectDuration -= 1 * Time.deltaTime;
            if (effectDuration <= 0)
            {
                reverseEffect();

                Destroy(this);
            }
        }
    }

    void reverseEffect()
    {
        ID.rotationSpeed /= rotationSpeedMul;
        ID.drag /= dragMul;
        ID.thrust /= thrustMul;
    }
}
