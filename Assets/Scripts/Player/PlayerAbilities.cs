using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAbilities : MonoBehaviour
{
    public GameObject projectilePrefab;
    public GameObject missilePrefab;
    GameObject firingPoint;
    PlayerID ID;
    Rigidbody2D rb;
    PlayerGraphics graphics;

    // Timers
    public float boostCooldown;
    public float boostTimer;
    public float missileCooldown;
    public float missileTimer;
    public float primaryFireCooldown;
    public float primaryFireTimer;

    private void Awake()
    {
        ID = GetComponent<PlayerID>();
        rb = GetComponent<Rigidbody2D>();

        GameObject graphicsOb = transform.GetChild(2).gameObject;
        graphics = graphicsOb.GetComponent<PlayerGraphics>();

        firingPoint = transform.GetChild(0).gameObject;

        // Set cooldowns
        boostCooldown = ID.boostCooldown;
        missileCooldown = ID.missileCooldown;
        primaryFireCooldown = ID.primaryFireRate;

        boostTimer = boostCooldown;
        missileTimer = missileCooldown;
        primaryFireTimer = primaryFireCooldown;
    }

    private void Update()
    {
        timers();
    }

    void timers()
    {
        boostTimer -= 1 * Time.deltaTime;
        missileTimer -= 1 * Time.deltaTime;
        primaryFireTimer -= 1 * Time.deltaTime;
    }

    /// <summary>
    /// Pass the ability to be requested in the type paramiter. If the ability is of cooldown the request will be accepted and the ability will take action.
    /// </summary>
    /// <param name="type"></param>
    public void request(string type)
    {
        switch (type)
        {
            case "Boost":
                if (boostTimer <= 0)
                {
                    exicuteBoost();
                    boostTimer = boostCooldown;
                }
                break;
            case "Missile":
                if (missileTimer <= 0)
                {
                    shootMissile();
                    missileTimer = missileCooldown;
                }
                break;
            case "PrimaryFire":
                if (primaryFireTimer <= 0)
                {
                    shootProjectile();
                    primaryFireTimer = primaryFireCooldown;
                }
                break;
        }
    }

    void exicuteBoost()
    {
        if (ID.movement_enabled == false) 
            return;

        float radAngle = transform.eulerAngles.z * Mathf.Deg2Rad;

        float x = Mathf.Sin(radAngle) * -1f;
        float y = Mathf.Cos(radAngle);

        //rb.velocity = new Vector2(0f, 0f);
        graphics.boostParticules();
        rb.AddForce(new Vector2(x, y) * 500f);
    }

    void shootMissile()
    {
        GameObject nMissGameOb = Instantiate(missilePrefab, firingPoint.transform.position, firingPoint.transform.rotation);
        Missile nMiss = nMissGameOb.GetComponent<Missile>();

        nMiss.SendInfo(1000f, 1f, 2, 30f, true, 0.7f, gameObject);
    }

    void shootProjectile()
    {
        for (int i = 0; i < ID.primaryFireCount; i++)
        {
            GameObject nProjGameOb = Instantiate(projectilePrefab, firingPoint.transform.position, firingPoint.transform.rotation);
            Projectile nProj = nProjGameOb.GetComponent<Projectile>();

            nProj.SendInfo(ID.primaryFireForce * 10f, ID.primaryFireSize, ID.primaryFireMass, ID.primaryFireDrag, ID.primaryFireLifeTime, ID.primaryFirePower, ID.primaryFireSpread, gameObject);
        }
    }
}
