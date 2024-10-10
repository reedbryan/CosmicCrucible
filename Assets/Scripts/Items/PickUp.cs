using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// When attached to a game object, this script allows the game object to be picked up by players and give them
/// buffs, debuffs, damage, etc...
/// </summary>
public class PickUp : MonoBehaviour
{
    [SerializeField] GameObject itemEffect;
    [SerializeField] float lifeTime;

    public float desirability;

    private void Awake()
    {
        itemEffect = transform.GetChild(0).gameObject;
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        GameObject other = collider.gameObject;

        if (other.CompareTag("Player"))
        {
            PlayerHit(other);
        }
    }

    void PlayerHit(GameObject player)
    {
        itemEffect.transform.parent = player.transform;
        Destroy(gameObject);
    }

    private void Update()
    {
        lifeTime -= 1 * Time.deltaTime;
        if (lifeTime <= 0)
        {
            Destroy(gameObject);
        }
    }
}
