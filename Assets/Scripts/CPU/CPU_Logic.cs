using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CPU_Logic : MonoBehaviour
{
    Rigidbody2D rb;
    PlayerID ID;
    PlayerMovement movement;
    PlayerAbilities abilities;

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
    }

    private void Update()
    {
        getSerroundings();
        mainBehavior();
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
        //targetPosMarker.SetActive(false);

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
        float dPrime;
        float distance = General.Distance(transform.position, subject.transform.position);
        float dNet = 0;
        float x1 = GetComponent<DamageIntake>().HP; // This CPU hp

        if (subject.CompareTag("Player"))
        {
            dPrime = 3f;
            float x2 = subject.GetComponent<DamageIntake>().HP; // Other player hp

            dNet = dPrime * ((30 + x1 - x2) / 10) * (1 / distance);
        }
        if (subject.CompareTag("Pickup"))
        {
            dPrime = subject.GetComponent<PickUp>().desirability;

            dNet = dPrime * (1 / distance);
        }
        if (subject.CompareTag("Health"))
        {
            dPrime = subject.transform.GetChild(0).GetComponent<HealthPack>().healthGiven;

            dNet = dPrime * (1 / distance) * ((GetComponent<DamageIntake>().maxHP / x1) / 10);
        }
        if (subject.CompareTag("Projectile"))
        {
            if (subject.GetComponent<Projectile>().shooter == gameObject)
                dNet = 0;
            else
            {
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
