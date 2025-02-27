using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Debris : MonoBehaviour
{
    Rigidbody2D rb;
    [SerializeField] static Sprite debSprite0;
    [SerializeField] static Sprite debSprite1;
    [SerializeField] static Sprite debSprite2;
    [SerializeField] static Sprite debSprite3;
    [SerializeField] static Sprite debSprite4;
    [SerializeField] static Sprite debSprite5;

    [SerializeField] Sprite[] debSprites = { debSprite0 , debSprite1, debSprite2 , debSprite3 , debSprite4 , debSprite5 };

    ParticleSystem PS3; // boost like ps that is only set on at the beggening for the initial burst

    [SerializeField] Vector2 playerVelocity;
    [SerializeField] float rotationSpeed;
    float size;
    float drag;

    // timer
    [SerializeField] float lifeTime;

    public void sendInfo(Rigidbody2D sourceRB, float sizeValue, float dragValue)
    {
        playerVelocity = sourceRB.velocity;
        size = sizeValue;
        drag = dragValue;

        setInfo();
    }

    void setInfo()
    {
        // Get commponents
        rb = GetComponent<Rigidbody2D>();
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        PS3 = transform.GetChild(2).gameObject.GetComponent<ParticleSystem>();


        // gfx
        sr.sprite = debSprites[Random.Range(0, debSprites.Length - 1)];

        // movement
        rb = GetComponent<Rigidbody2D>();

        rb.drag = drag;
        size += Random.Range(size * -0.8f, size * 0.8f);
        transform.localScale = new Vector3(size, size, size);
        foreach (Transform child in transform)
        {
            child.localScale = new Vector3(size, size, size);
        }

        Vector2 newVelocity = new Vector2(
            playerVelocity.x + Random.Range(playerVelocity.x / -1.5f, playerVelocity.x / 1.5f),
            playerVelocity.y + Random.Range(playerVelocity.y / -1.5f, playerVelocity.y / 1.5f));
        rb.velocity = newVelocity;

        rb.AddTorque(rotationSpeed);

        // Particule system
        float angle = (Mathf.Atan2(playerVelocity.x, playerVelocity.y) * Mathf.Rad2Deg) - 90;
        //Debug.Log("Angle: " + angle);
        transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, angle);
        PS3.Emit(50);

        // Timer
        lifeTime += Random.Range(lifeTime * -0.8f, lifeTime * 0.8f);
    }

    private void Update()
    {
        // Death timer
        lifeTime -= 1 * Time.deltaTime;
        if (lifeTime <= 0)
        {
            Destroy(gameObject);
        }
    }
}