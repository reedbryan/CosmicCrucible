using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The "SendInfo" method must be called after the missile is instantiated for the missile to move.
/// </summary>
public class Missile : MonoBehaviour
{
    Rigidbody2D rb;
    GameObject shooter;
    ParticleSystem jetsPS;

    // Needs to be public so projectiles can communitate between each other
    public float force;
    public float size;
    public float lifeTime;
    public float power;
    public bool tracking;
    public float waitTime;
    [SerializeField] float jetsEmissionRate;

    /// <summary>
    /// If the missile is currently in motion.
    /// </summary>
    private bool accelerating = false;
    /// <summary>
    /// If the missile is waiting to be put into motion.
    /// </summary>
    private bool waiting = false;


    /// <summary>
    /// The start method of the projectile. Should be called when gameOb is instansiated to
    /// fill out the information of the projectile.
    /// </summary>
    public void SendInfo(float forceValue, float sizeValue, float lifeTimeValue, float powerValue, bool isTracking, float waitTimeValue, GameObject shooterValue)
    {
        jetsPS = GetComponent<ParticleSystem>();

        force = forceValue;
        size = sizeValue;
        lifeTime = lifeTimeValue;
        power = powerValue;
        shooter = shooterValue;
        tracking = isTracking;
        waitTime = waitTimeValue;

        Launch();
    }

    void Launch()
    {
        // Set information
        rb = GetComponent<Rigidbody2D>();
        rb.drag = 1;
        transform.localScale = new Vector3(size, size, size);

        // Waiting prossess

        waiting = true;
        General.ForwardForce(force * 40, transform, rb);
        Rigidbody2D playerRb = shooter.GetComponent<Rigidbody2D>();
        rb.velocity += playerRb.velocity;
    }

    private void Update()
    {
        // Time before acceleration
        if (waiting)
        {
            if (waitTime <= 0)
            {
                accelerating = true;
                waiting = false;
            }
            waitTime -= 1 * Time.deltaTime;

            // Particule Emission
            var em = jetsPS.emission;
            var main = jetsPS.main;
            main.startSpeed = 0.1f;
            em.enabled = true;
            em.rateOverTime = jetsEmissionRate / 10;
        }

        if (accelerating)
        {
            // Count down lifetime
            if (lifeTime <= 0)
            {
                Destroy(gameObject);
            }
            lifeTime -= 1 * Time.deltaTime;
            General.ForwardForce(force, transform, rb);

            // Particule Emission
            var em = jetsPS.emission;
            var main = jetsPS.main;
            main.startSpeed = 1f;
            em.enabled = true;
            em.rateOverTime = jetsEmissionRate;
        }

        if (tracking)
        {
            // Get nearest player
            GameManager gm = GameObject.Find("GameManager").GetComponent<GameManager>();

            bool playerFound = false;
            float minDistance = 10000000f;
            Vector2 mdPos = new Vector2(0,0);
            foreach (var item in gm.inGamePlayerList)
            {
                Vector2 playerPos = item.gameObject.transform.position;
                float distance = General.Distance(transform.position, playerPos);
                if (distance < minDistance && item.gameObject != shooter)
                {
                    minDistance = distance;
                    mdPos = playerPos;
                    playerFound = true;
                }
            }

            if (playerFound)
            {
                General.RotateTowards(transform, mdPos, 50f);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        GameObject hit = collision.gameObject;

        if (hit == gameObject || hit == shooter) // if this projectile or the player that shot this projectile
        {
            return;
        }

        if (hit.CompareTag("Player")) // If this projectile makes contact with a player
        {
            dealDamage(hit);
        }
    }

    void dealDamage(GameObject subject)
    {
        // Get DamageIntake script and make sure the object has it
        DamageIntake dIntake = subject.GetComponent<DamageIntake>();
        if (dIntake == null)
        {
            return;
        }

        // Deal damage
        dIntake.alterHP(power * -1f, transform.position);

        // Apply force
        Rigidbody2D subRb = subject.GetComponent<Rigidbody2D>();
        subRb.velocity += (rb.velocity * rb.mass) / subRb.mass / 10f; ;

        // Destroy this projectile to avoid dealing more damage
        Destroy(gameObject);
    }
}
