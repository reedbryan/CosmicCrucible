using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Barrier : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D collision)
    {
        GameObject hit = collision.gameObject;

        // on player collision
        if (hit.tag == "Player")
        {
            if (hit.GetComponent<CPU_Logic>() != null){
                hit.GetComponent<DamageIntake>().Die();
            }
            else {
                Rigidbody2D rb = hit.GetComponent<Rigidbody2D>();
                rb.velocity *= -0.85f; // Reverse and dampen player velocity
            }
        }
    }
}
