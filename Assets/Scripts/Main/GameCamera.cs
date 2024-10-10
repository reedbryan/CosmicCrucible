using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCamera : MonoBehaviour
{
    GameManager gm;
    /// <summary>
    /// The distance in unity meters that a player needs to be from the center of the screen for it to expand.
    /// </summary>
    [SerializeField] float expantionGap;

    /// <summary>
    /// The minimum orthagraphic size of the camera.
    /// </summary>
    [SerializeField] float minSize;

    private void Awake()
    {
        GameObject gmOb = GameObject.Find("GameManager");
        gm = gmOb.GetComponent<GameManager>();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        /* Main camera position in x,y plane */
        Vector2[] playerPositions = new Vector2[gm.inGamePlayerList.Count];
        for (int i = 0; i < playerPositions.Length; i++)
        {
            playerPositions[i] = gm.inGamePlayerList[i].transform.position;
        }
        if (gm.inGamePlayerList.Count >= 1)
        {
            Vector2 centerPos = getCenterPosition(playerPositions);
            transform.position = new Vector3(centerPos.x, centerPos.y, -10f);
        }
        else
        {
            transform.position = new Vector3(0,0,-10);
        }

        /* Main camera size */

        // Get longest distance between players
        float maxDistance = 0f;
        foreach (var item in playerPositions)
        {
            float distance = General.Distance(item, transform.position);
            if (distance > maxDistance)
            {
                maxDistance = distance;
            }
        }

        // Size calculations
        if (expantionGap + maxDistance < minSize)
        {
            Camera.main.orthographicSize = minSize;
        }
        else
        {
            Camera.main.orthographicSize = expantionGap + maxDistance;
        }
    }

    Vector2 getDistanceV(Vector2 pos1, Vector2 pos2)
    {
        Vector2 pos = pos1 - pos2;
        pos = new Vector2(Mathf.Abs(pos.x), Mathf.Abs(pos.y));
        return pos;
    }

    Vector2 getCenterPosition(Vector2[] positions)
    {
        Vector2 avPos = new Vector2(0,0);
        foreach (var item in positions)
        {
            avPos += item;
        }
        avPos /= positions.Length * 1f;
        return avPos;
    }
}
