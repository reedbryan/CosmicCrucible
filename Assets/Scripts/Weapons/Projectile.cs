using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The "SendInfo" method must be called after the projectile is instantiated for the projectile to move.
/// </summary>
public class Projectile : MonoBehaviour
{
    Rigidbody2D rb;
    public GameObject shooter;

    // Needs to be public so projectiles can communitate between each other
    public float force;
    public float size;
    public float drag;
    public float lifeTime;
    public float power;
    public float spread;
    public float mass;

    float timeAfterStop = 0f;

    /// <summary>
    /// The start method of the projectile. Should be called when gameOb is instansiated to
    /// fill out the information of the projectile.
    /// </summary>
    public void SendInfo(float forceValue, float sizeValue, float massValue, float dragValue, float lifeTimeValue, float powerValue, float spreadValue, GameObject shooterValue)
    {
        force = forceValue;
        size = sizeValue;
        drag = dragValue;
        lifeTime = lifeTimeValue;
        power = powerValue;
        shooter = shooterValue;
        spread = spreadValue;
        mass = massValue;

        Launch();
    }

    void Launch()
    {
        // Set information
        rb = GetComponent<Rigidbody2D>();
        Rigidbody2D playerRb = shooter.GetComponent<Rigidbody2D>();

        transform.localScale = new Vector3(size, size, size);
        rb.drag = drag;
        rb.mass = mass;

        // Launch
        Vector3 roationOffset = new Vector3(0, 0, Random.Range(spread * -1, spread));
        transform.eulerAngles += roationOffset;
        float radAngle = transform.eulerAngles.z * Mathf.Deg2Rad;

        float x = Mathf.Sin(radAngle) * -1f;
        float y = Mathf.Cos(radAngle);

        float forceOffset = Random.Range(spread * -1, spread);
        rb.AddForce(new Vector2(x, y) * (force + forceOffset));
        rb.velocity += playerRb.velocity;
    }

    private void Update()
    {
        // Count down lifetime
        if (lifeTime <= 0)
        {
            Destroy(gameObject);
        }
        lifeTime -= 1 * Time.deltaTime;

        // Destroy after 0.5s if proj stops moving
        if (rb.velocity == new Vector2(0f,0f))
        {
            if (timeAfterStop <= 0)
            {
                //Destroy(gameObject);
            }
            timeAfterStop -= 1 * Time.deltaTime;
        }

        //lookForCollisions();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        GameObject hit = collision.gameObject;

        if (hit == gameObject || hit == shooter) // if this projectile or the player that shot this projectile
        {
            return;
        }

        if (hit.CompareTag("Projectile")) // If this projectile makes contact with another projectile
        {
            Projectile hitProjectile = hit.GetComponent<Projectile>();
            if (hitProjectile.power >= power)
            {
                return;
            }
            else
            {
                power -= hitProjectile.power;
                Destroy(hit);
            }
        }

        DamageIntake dIn = hit.GetComponent<DamageIntake>();
        if (dIn)
        {
            dealDamage(hit);
        }
    }


    // - - - - - - - - - - - - - - Ray cast collision detection - - - - - - - - - - - /
    /*
    void lookForCollisions()
    {
        Collider2D hit = Physics2D.OverlapCircle(transform.position, size / 10f);
        if (hit)
        {
            GameObject hitGameOb = hit.gameObject;

            if (hitGameOb == gameObject || hitGameOb == shooter) // if this projectile or the player that shot this projectile
            {
                return;
            }

            if (hitGameOb.CompareTag("Projectile")) // If this projectile makes contact with another projectile
            {
                Projectile hitProjectile = hitGameOb.GetComponent<Projectile>();
                if (hitProjectile.power >= power)
                {
                    return;
                }
                else
                {
                    power -= hitProjectile.power;
                    Destroy(hitGameOb);
                }
            }

            if (hitGameOb.CompareTag("Player")) // If this projectile makes contact with a player
            {
                dealDamage(hitGameOb);
            }
        }
        else
        {
            return;
        }
    }
    */


    /// <summary>
    /// Can only deals damage to Game Objects with a "DamageIntake" script
    /// </summary>
    void dealDamage(GameObject subject) 
    {
        // Get DamageIntake script and make sure the object has it
        DamageIntake dIntake = subject.GetComponent<DamageIntake>();
        if (dIntake == null)
        {
            return;
        }

        //Debug.Log("Alter HP");

        // Deal damage
        dIntake.alterHP(power * -1f, transform.position);

        // Apply force
        Rigidbody2D subRb = subject.GetComponent<Rigidbody2D>();
        subRb.velocity += (rb.velocity * rb.mass) / subRb.mass / 10f; ;

        // Destroy this projectile to avoid dealing more damage
        Destroy(gameObject);
    }

    // On draw gizmos
    /*
    private void OnDrawGizmosSelected()
    {
        Debug.DrawLine(transform.position, new Vector3(transform.position.x, transform.position.y + size / 10f, 0));
    }
    */
}
