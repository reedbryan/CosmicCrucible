using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGraphics : MonoBehaviour
{
    ParticleSystem jetsPS;
    ParticleSystem boostPS;
    GameObject player;

    // Firing point
    GameObject firingPoint;
    [SerializeField] Vector2 startAnimPos;
    [SerializeField] Vector2 endAnimPos;
    [SerializeField] float animPoint;

    [SerializeField] float jetEmissionRate;
    bool jetsOn = false;
    float stTimer = 0.1f;
    float timer;


    private void Awake()
    {
        player = transform.parent.gameObject;
        firingPoint = player.transform.GetChild(0).gameObject;

        GameObject jetPSGameOb = player.transform.GetChild(1).gameObject; // Jets
        jetsPS = jetPSGameOb.GetComponent<ParticleSystem>();

        GameObject boostPSGameOb = player.transform.GetChild(3).gameObject; // Boost
        boostPS = boostPSGameOb.GetComponent<ParticleSystem>();

        timer = stTimer;
    }

    private void Update()
    {
        // Timer (to turn jets off I think)
        timer -= 1 * Time.deltaTime;
        if (timer <= 0)
        {
            timer = stTimer;
            jetsOn = false;
        }

        // Emission
        var em = jetsPS.emission;
        em.enabled = true;

        if (!jetsOn)
        {
            em.rateOverTime = 0;
        }
        else
        {
            em.rateOverTime = jetEmissionRate;
        }

        transform.position = new Vector3(transform.position.x, transform.position.y, -1);

        // Color (temporary)
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        PlayerID ID = player.GetComponent<PlayerID>();

        switch (ID.playerNumber)
        {
            case 1:
                sr.color = Color.red;
                break;
            case 2:
                sr.color = Color.blue;
                break;
            case 3:
                sr.color = Color.green;
                break;
            case 4:
                sr.color = Color.yellow;
                break;
            default:
                sr.color = Color.black;
                break;
        }

        // Firing point anim
        float topPoint = player.GetComponent<PlayerAbilities>().primaryFireCooldown;
        float currPoint = player.GetComponent<PlayerAbilities>().primaryFireTimer;
        if (topPoint < 0.2 && currPoint > 0)
        {
            animPoint = currPoint / topPoint;
        }
        else if (currPoint > 0)
        {
            if (currPoint > 0.2)
            {
                currPoint = 0.2f;
            }
            animPoint = currPoint / 0.2f;
        }

        Vector2 animPosDiff = startAnimPos - endAnimPos;
        firingPoint.transform.localPosition = startAnimPos - (animPosDiff * animPoint);
    }

    public void boostParticules()
    {
        // Emission
        boostPS.Emit(100);
    }

    public void jetParticules()
    {
        jetsOn = true;
    }
}
