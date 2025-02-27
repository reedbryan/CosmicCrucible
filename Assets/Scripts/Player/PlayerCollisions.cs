using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class PlayerCollisions : MonoBehaviour
{
    Rigidbody2D rb;
    PlayerID ID;
    public GameObject ExplosionPrefab;
    public GameObject DebrisPrefab;

    void Awake()
    {
        ID = GetComponent<PlayerID>();
        rb = GetComponent<Rigidbody2D>();
    }
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        GameObject hit = collision.gameObject;

        if (hit.CompareTag("Player")) // If this projectile makes contact with another projectile
        {            
            // Get other player components
            PlayerID other_ID = hit.GetComponent<PlayerID>();
            DamageIntake other_damageIntake = hit.GetComponent<DamageIntake>();
            Transform other_transform = hit.GetComponent<Transform>();
            PlayerCollisions other_playerCollisions = hit.GetComponent<PlayerCollisions>();

            // Disable movement
            ID.movement_enabled = false;
            
            // Calculate impact value using stored velocity
            int impact = (int)rb.velocity.magnitude;
            Debug.Log("Impact (before collision): " + impact);

            // Create explosion (based on impact value)
            createExplosion(impact);

            // Update velocity based on angle of collision
            //Vector2 angle_of_collision = (Vector2)(transform.position - other_transform.position).normalized;
            rb.velocity = other_playerCollisions.rb.velocity + rb.velocity * 0.5f; // * angle_of_collision

            // Deal damage
            other_damageIntake.alterHP(-1.5f * impact, other_transform.position);
        }
    }

    void createExplosion(int impact)
    {
        GameObject ex = Instantiate(ExplosionPrefab, transform.position, transform.rotation);
        ex.GetComponent<Explosion>().explosion(rb.velocity, impact);
        
        int num_debris = (int)(impact / 10) + UnityEngine.Random.Range(-1, 2);
        for (int i = 0; i < num_debris; i++)
        {
            GameObject newDebris = Instantiate(DebrisPrefab, transform.position, transform.rotation);
            Debris newdebScript = newDebris.GetComponent<Debris>();

            newdebScript.sendInfo(GetComponent<Rigidbody2D>(), 3, GetComponent<Rigidbody2D>().drag);
        }
    }
}
