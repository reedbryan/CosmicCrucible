using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DamageMarker : MonoBehaviour
{
    Text damageText;
    [SerializeField] GameObject anchor;

    bool spawned = false;
    [SerializeField] float lifeTime;

    float xOffset;
    float yOffset;
    float rotOffset;
    [SerializeField] float offsetMovementSpeed;
    [SerializeField] float offsetRotationSpeed;

    // Basicly the start method
    public void sendInfo(float amount)
    {
        // Get components and such
        damageText = GetComponent<Text>();
        if (amount >= 0)
        {
            damageText.text = "+" + Mathf.Round(amount).ToString();
            damageText.color = Color.green;
        }
        else
        {
            damageText.text = Mathf.Round(amount).ToString();
            damageText.color = Color.red;
        }
        anchor = transform.parent.transform.parent.gameObject;
        anchor.transform.localScale += new Vector3(Mathf.Abs(amount), Mathf.Abs(amount), 0) * 0.01f;
        if (anchor.transform.localScale.x >= 1)
        {
            anchor.transform.localScale = new Vector3(1,1,1);
        }

        xOffset = Random.Range(-1f, 1f);
        yOffset = Random.Range(0, 1f);
        rotOffset = Random.Range(-1f, 1f);

        spawned = true;
    }

    private void Update()
    {
        if (!spawned)
            return;

        // Offset
        anchor.transform.position += new Vector3(xOffset, yOffset) * Time.deltaTime * offsetMovementSpeed;
        anchor.transform.eulerAngles += new Vector3(0, 0, rotOffset) * Time.deltaTime * offsetRotationSpeed * 15;

        // Timer
        if (lifeTime <= 0)
        {
            Destroy(anchor);
        }
        lifeTime -= 1 * Time.deltaTime;
    }
}
