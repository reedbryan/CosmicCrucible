using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageIntake : MonoBehaviour
{
    GameManager GM;
    PlayerID ID;
    InGameUI inGameUI;

    // references
    public GameplayCycle gameplayCycle;
    [SerializeField] GameObject debrisPrefab;

    public float maxHP;
    public float HP; // Hit Points/Heath
    /// <summary>
    /// HP gained per second
    /// </summary>
    [SerializeField] float passiveRegeneration;
    float PRtimer; // Passive regen timer

    bool DieFunc_mutex = true; // false = locked, true = unlocked

    private void Awake()
    {
        GameObject GMOb = GameObject.Find("GameManager");
        GM = GMOb.GetComponent<GameManager>();
        ID = GetComponent<PlayerID>();
        inGameUI = GMOb.GetComponent<InGameUI>();

        HP = maxHP;
    }

    private void Update()
    {
        if (HP < maxHP)
        {
            PRtimer -= 1 * Time.deltaTime;
            if (PRtimer <= 0)
            {
                PRtimer = 1;
                alterHP(passiveRegeneration, transform.position);
            }
        }
    }

    /// <summary>
    /// Change the current HP of the subject by "amount". If amount is negative it will take away HP if amount is positive it will add HP.
    /// </summary>
    /// <param name="amount"></param>
    /// <param name="location"></param>
    public void alterHP(float amount, Vector2 location)
    {
        //Debug.Log("Damage taken");

        // UI
        inGameUI.showDamage(amount, location);

        // Real damage
        HP += amount;
        if (HP <= 0 && DieFunc_mutex)
        {
            DieFunc_mutex = false; // lock mutex
            Die();
        }
        if (HP > maxHP)
        {
            HP = maxHP;
        }
    }

    void createDebris()
    {
        GameObject newDebris = Instantiate(debrisPrefab, transform.position, transform.rotation);
        Debris newdebScript = newDebris.GetComponent<Debris>();

        newdebScript.sendInfo(GetComponent<Rigidbody2D>(), 3 , GetComponent<Rigidbody2D>().drag);
    }

    public void Die()
    {
        if (gameObject.CompareTag("Player")) // If game object this script is attached to is a player
        {
            for (int i = 0; i < Random.Range(5, 12); i++)
            {
                createDebris();
            }

            GM.removeInGamePlayer(ID);
        }
        Debug.Log("Player #" + ID.playerNumber + " died");
        Destroy(this.gameObject);
    }
}
