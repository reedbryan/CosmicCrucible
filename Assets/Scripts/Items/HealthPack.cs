using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPack : MonoBehaviour
{
    // Effect properties
    bool effectActive = false;
    public float healthGiven;

    /// <summary>
    /// This should only be called after effect script is attached to the player
    /// </summary>
    public void TriggerEffect()
    {
        Debug.Log("Health pack triggered");

        // Effect
        DamageIntake dmgIntake = transform.parent.transform.GetComponent<DamageIntake>();
        dmgIntake.alterHP(healthGiven, transform.position);

        Destroy(this);
    }

    private void Update()
    {
        if (!effectActive)
        {
            // Check if the parent of this game object is the player (this means the effect should be active)
            if (transform.parent.CompareTag("Player"))
                TriggerEffect();
        }
    }
}
