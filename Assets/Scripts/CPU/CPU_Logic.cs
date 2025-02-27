using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CPU_Logic : MonoBehaviour
{
    Rigidbody2D rb;
    PlayerID ID;
    PlayerMovement movement;
    PlayerAbilities abilities;
    GameManager GM;

    [SerializeField] List<GameObject> objectsInRange = new List<GameObject>();
    [SerializeField] float bubbleRadius;

    float cutInterval;

    // Testing purposes
    [SerializeField] GameObject target;
    [SerializeField] float desirability;
    public GameObject targetPosMarker;
    
    void Awake()
    {
        ID = GetComponent<PlayerID>();
        rb = GetComponent<Rigidbody2D>();
        movement = GetComponent<PlayerMovement>();
        abilities = GetComponent<PlayerAbilities>();
        GM = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    private void Update()
    {
        getSerroundings();
        mainBehavior();
    }

    private void Start()
    {
        GM.onResetMap.AddListener(HandleReset);
    }

    private void HandleReset()
    {
        DamageIntake damageIntake = gameObject.GetComponent<DamageIntake>();
        damageIntake.alterHP(-damageIntake.maxHP - 1, gameObject.transform.position);
    }

    void getSerroundings()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, bubbleRadius);

        if (hits == null)
            return;

        foreach (var hit in hits)
        {
            bool alreadySeen = false;
            List<GameObject> objectsToRemove = new List<GameObject>();

            foreach (var anyObject in objectsInRange)
            {
                if (hit.gameObject == anyObject)
                    alreadySeen = true;

                if (anyObject.gameObject == null)
                {
                    objectsToRemove.Add(anyObject);
                }
            }

            foreach (var item in objectsToRemove)
            {
                objectsInRange.Remove(item);
            }

            if (!alreadySeen && hit.gameObject != gameObject)
            {
                objectsInRange.Add(hit.gameObject);
            }
        }
    }

    void mainBehavior()
    {
        float dMax = 0;
        GameObject mostDesirable = null;

        foreach (var item in objectsInRange)
        {
            float d = getDesirability(item);
            if (Mathf.Abs(d) > Mathf.Abs(dMax))
            {
                mostDesirable = item;
                dMax = d;
            }
        }

        if (mostDesirable == null)
        {
            return;
        }

        Rigidbody2D mostDrb = mostDesirable.GetComponent<Rigidbody2D>();
        if (mostDrb && (mostDrb.velocity.x != 0 || mostDrb.velocity.y != 0))
        {
            targetMovement(mostDesirable, mostDrb);
        }
        else
        {
            targetStatic(mostDesirable);
        }

        // Shooting
        if (mostDesirable.CompareTag("Player"))
        {
            abilities.request("PrimaryFire");
            if (General.Distance(transform.position, mostDesirable.transform.position) < 50f)
            {
                abilities.request("Missile");
            }
        }

        target = mostDesirable;
        desirability = dMax;
    }

    void targetMovement(GameObject mostDesirable, Rigidbody2D mostDrb)
    {
        float negativeCheck = 1;
        if (desirability <= 0) // If there is a negative desirability -> run from object
        {
            negativeCheck = -1;
            targetPosMarker.GetComponent<SpriteRenderer>().color = Color.red;
        }
        else
        {
            targetPosMarker.GetComponent<SpriteRenderer>().color = Color.blue;
        }

        // Get projectile velocity in vector2 form
        float radAngle = transform.eulerAngles.z * Mathf.Deg2Rad;
        float x = Mathf.Sin(radAngle) * -1f;
        float y = Mathf.Cos(radAngle);
        Vector2 primFireV = (ID.primaryFireForce * 100 / ID.primaryFireMass / 50 * new Vector2(x,y)) + rb.velocity;

        // Get the time it would take for the bullet to reach the other player and
        // distance the other player will travel during that time
        float timeForProj = General.timeToReach(transform.position, mostDesirable.transform.position, primFireV, ID.primaryFireDrag);
        Vector2 newTargetPosition = General.distanceInTime(mostDesirable.transform.position, timeForProj, mostDrb.velocity, mostDrb.drag);
        Vector2 targetPos = ((Vector2)transform.position - newTargetPosition).normalized;

        // Marker (for debuging)
        // Have false when the marker is not needed
        targetPosMarker.transform.position = newTargetPosition;
        targetPosMarker.SetActive(false);

        // Rotate the CPU to the new target position
        movement.setAxisInput(targetPos.x, targetPos.y * -1 * negativeCheck);

        // Forward thrust!!!
        movement.Thrust();
        PlayerGraphics gfx = transform.GetChild(2).gameObject.GetComponent<PlayerGraphics>();
        gfx.jetParticules();

        // Boost?!
        if (Mathf.Abs(desirability) >= 1 && General.Distance(transform.position, mostDesirable.transform.position) >= 6f)
        {
            abilities.request("Boost");
        }
    }

    void targetStatic(GameObject mostDesirable)
    {
        // Get target position
        Vector2 targetPos = (transform.position - mostDesirable.transform.position).normalized;

        // Forward thrust!!!
        RaycastHit2D[] rayHits = Physics2D.RaycastAll(transform.position, rb.velocity.normalized);
        Debug.DrawRay(transform.position, rb.velocity.normalized * 1000, Color.red);
        if (rayHits == null)
            return;

        bool inSights = false;
        foreach (var item in rayHits)
        {
            if (item.transform.gameObject == mostDesirable)
                inSights = true;
            else
                inSights = false;
        }
        if (inSights)
        {
            // Move
            movement.Thrust();
            PlayerGraphics gfx = transform.GetChild(2).gameObject.GetComponent<PlayerGraphics>();
            gfx.jetParticules();

            // Rotate
            movement.setAxisInput(targetPos.x, targetPos.y * -1);

            //Debug.Log("in sights");
        }
        else
        {
            /*
            // Move
            movement.Thrust();
            PlayerGraphics gfx = transform.GetChild(2).gameObject.GetComponent<PlayerGraphics>();
            gfx.jetParticules();

            // Rotate
            float angle = General.getAngle(transform.position, mostDesirable.transform.position);
            Vector2 rotationCoodinates;
            if (angle + 180 >= transform.eulerAngles.z)
            {
                rotationCoodinates = rotateCoordinates(false);
            }
            else
            {
                rotationCoodinates = rotateCoordinates(true);
            }
            movement.setAxisInput(rotationCoodinates.x * -1, rotationCoodinates.y * -1);
            */

            // Rotate
            movement.setAxisInput(targetPos.x, targetPos.y * -1);
            cutMovement(1f / (10 * General.Distance(transform.position, mostDesirable.transform.position)));
            //Debug.Log("NOT in sights");
        }
    }

    float getDesirability(GameObject subject)
    {
        float dPrime;   // Baseline desirability for a given object
                        // REF:  speed boosts have dPrime = 5
                        //       players have dPrime = 3
        float distance = General.Distance(transform.position, subject.transform.position);
        float dNet = 0;
        DamageIntake this_DI = GetComponent<DamageIntake>();
        float this_HP_norm = ((this_DI.HP - this_DI.maxHP) / this_DI.maxHP) + 1; // this CPU's hp: 1 full, 0 dead

        if (subject.CompareTag("Player"))
        {
            if (subject.GetComponent<PlayerID>().playerNumber == -1)
                dPrime = 1f;
            else 
                dPrime = 3f;
                
            DamageIntake subject_DI = subject.GetComponent<DamageIntake>();
            float other_HP_norm = (subject_DI.HP - subject_DI.maxHP) / subject_DI.maxHP; // Other player hp

            dNet = dPrime * ((30 + this_HP_norm - other_HP_norm) / 10) * (1 / distance);
            //Debug.Log("Player being looked at from a distance of: " + distance + "\nAnd a desirebility of: " + dNet);
        }
        if (subject.CompareTag("Pickup"))
        {
            dPrime = subject.GetComponent<PickUp>().desirability;
            
            dNet = dPrime * (1 / distance);
            //Debug.Log("Pickup being looked at from a distance of: " + distance + "\nAnd a desirebility of: " + dNet);
        }
        if (subject.CompareTag("Health"))
        {
            float health_recoverable = this_DI.maxHP - this_DI.HP;
            float health_given = subject.transform.GetChild(0).GetComponent<HealthPack>().healthGiven;
            if (health_recoverable < health_given){
                // if the healthpack heals more than the CPU needs
                dPrime = health_recoverable;
            }else {
                // if the healthpack heals less than the CPU needs i.e. they need that health pack
                dPrime = health_given;
            }
            
            dNet = dPrime * (100 / this_DI.maxHP) * (1 / distance);
            //Debug.Log("dPrime: " + dPrime);
            //Debug.Log("Health pack being looked at from a distance of: " + distance + "\nAnd a desirebility of: " + dNet);
        }
        if (subject.CompareTag("Projectile"))
        {
            if (subject.GetComponent<Projectile>().shooter == gameObject)
                dNet = 0;
            else
            {
                //Debug.Log("Projectile being looked at from a distance of: " + distance + "\nAnd a desirebility of: " + dNet);
                dPrime = -3f;
                dNet = dPrime * (1 / distance);
            }
        }

        return dNet;
    }

    void cutMovement(float cutValue)
    {
        if (cutInterval <= 0)
        {
            movement.Thrust();
            PlayerGraphics gfx = transform.GetChild(2).gameObject.GetComponent<PlayerGraphics>();
            gfx.jetParticules();
            cutInterval = cutValue;
        }
        cutInterval -= 1 * Time.deltaTime;
    }

    Vector2 rotateCoordinates(bool clockwise)
    {
        float currZangle = transform.eulerAngles.z;
        float newZangle = currZangle;

        //Debug.Log("curr Z angle: " + transform.eulerAngles.z);

        if (clockwise)
            newZangle -= 1;
        else
            newZangle += 1;

        //Debug.Log("Z angle: " + newZangle);

        Vector2 rotationCoordinates = new Vector2(Mathf.Sin(newZangle), Mathf.Cos(newZangle));

        //Debug.Log("Rotation Coordinates: " + rotationCoordinates);

        return rotationCoordinates;
    }
}
