using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameCamera : MonoBehaviour
{
    GameManager gm;
    /// <summary>
    /// The distance in unity meters that a player needs to be from the center of the screen for it to expand.
    /// </summary>
    [SerializeField] float expantionGap;
    [SerializeField] float maxSize;

    /// <summary>
    /// The minimum orthagraphic size of the camera.
    /// </summary>
    [SerializeField] float minSize;

    [SerializeField] float caseNumber = 0;

    private void Awake()
    {
        GameObject gmOb = GameObject.Find("GameManager");
        gm = gmOb.GetComponent<GameManager>();

        // set position (mostly for z value)
        transform.position = new Vector3(0, 0, -10f);
    }

    // Update is called once per frame
    void LateUpdate()
    {
        
        /* Get positions of all players in the game */
        List<PlayerID> ingame_player_list = gm.inGamePlayerList;

        Vector2[] real_player_positions = new Vector2[gm.playerList.Count];         // Player posistions, not including CPUs
        Vector2[] all_player_positions = new Vector2[ingame_player_list.Count];     // Player posistions, including CPUs

        //Debug.Log("ingame_player_list.Count" + ingame_player_list.Count);     
        int real_player_index = 0;
        for (int i = 0; i < ingame_player_list.Count; i++)
        {
            if (ingame_player_list[i].controllerNumber != -1){ // add only the real players to player_positions
                //Debug.Log("i: " + i);
                //Debug.Log("real_player_positions[i]: " + real_player_positions[real_player_index]);
                //Debug.Log("ingame_player_list[i].transform.position: " + ingame_player_list[i].transform.position);
                real_player_positions[real_player_index] = ingame_player_list[i].transform.position;
                real_player_index++;
            }
            all_player_positions[i] = ingame_player_list[i].transform.position;
        }


        // Get longest distance between players
        float max_distance = 0f;
        foreach (var item in all_player_positions)
        {
            float distance = General.Distance(item, transform.position);
            if (distance > max_distance)
            {
                max_distance = distance;
            }
        }

        
        /* Evaluatation */
        // Here we find the ideal position and orthographic size for the camera
        // in the next section we increment the camera's size and position towards those idea values for smooth camera adjustment 
        Vector2 ideal_pos;
        float ideal_orthSize;

        // BASE CASE: there are no players
        if (ingame_player_list.Count < 1)
        {
            // place the camera at 0,0
            transform.position = new Vector3(0,0,-10);
            // set the size to min
            Camera.main.orthographicSize = minSize;
            // don't run the other cases
            caseNumber = 0;
            return;
        }

        // CASE 1: Getting all players on screen leads to a camera size larger than the max size
        if (expantionGap + max_distance > maxSize){
            
            if (real_player_positions.Length <= 1){
                caseNumber = 1;
                
                // if there is only one player
                // - follow the player
                ideal_pos = real_player_positions[0];
                // - set the camera size to max
                ideal_orthSize = maxSize;
            }
            else {
                caseNumber = 1.5f;
                
                // if there are multiple players:
                // - find the max distance between them
                max_distance = 0f;
                foreach (var item in real_player_positions)
                {
                    float distance = General.Distance(item, transform.position);
                    if (distance > max_distance)
                    {
                        max_distance = distance;
                    }
                }
                // - place the camera at their center
                ideal_pos = getCenterPosition(real_player_positions);

                // - set the camera size to fit all real players on screen
                ideal_orthSize = expantionGap + max_distance;
            }
        }
        // CASE 2: The max distance leads to a camera smaller than the min size
        else if (expantionGap + max_distance < minSize){
            caseNumber = 2;
            // place the camera at the center of all players
            ideal_pos = getCenterPosition(all_player_positions);
            // set the camera size to min
            ideal_orthSize = minSize;
        }
        // CASE 3: Getting all the players on screen isn't a problem
        else {
            caseNumber = 3;
            // place the camera at the center of all players
            ideal_pos = getCenterPosition(all_player_positions);
            // set the camera size to fit all players
            ideal_orthSize = expantionGap + max_distance;
        }


        /* increment camera position */
        // Current Values:
        float current_orthSize = Camera.main.orthographicSize;
        Vector2 current_pos = transform.position;
        
        // Difference Between Current and Ideal Values:
        float diff_orthSize = ideal_orthSize - current_orthSize;
        Vector2 diff_pos = ideal_pos - current_pos;

        // Increment orthagraphic camera size in real time
        Camera.main.orthographicSize += diff_orthSize * Time.deltaTime;
        
        // Increment position in real time
        transform.position += new Vector3(diff_pos.x, diff_pos.y, 0) * Time.deltaTime;
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





/*
OLD CAMERA CODE

// CASE 1: Getting all players on screen leads to a camera size larger than the max size
if (expantionGap + max_distance > maxSize){
    
    if (real_player_positions.Length <= 1){
        caseNumber = 1;
        
        // if there is only one player
        // - follow the player
        transform.position = new Vector3(real_player_positions[0].x, real_player_positions[0].y, -10);
        // - set the camera size to max
        Camera.main.orthographicSize = maxSize;
    }
    else {
        caseNumber = 1.5f;
        
        // if there are multiple players:
        // - find the max distance between them
        max_distance = 0f;
        foreach (var item in real_player_positions)
        {
            float distance = General.Distance(item, transform.position);
            if (distance > max_distance)
            {
                max_distance = distance;
            }
        }
        // - place the camera at their center
        Vector2 centerPos = getCenterPosition(real_player_positions);
        transform.position = new Vector3(centerPos.x, centerPos.y, -10f);

        // - set the camera size to fit all real players on screen
        Camera.main.orthographicSize = expantionGap + max_distance;
    }
}
// CASE 2: The max distance leads to a camera smaller than the min size
else if (expantionGap + max_distance < minSize){
    caseNumber = 2;
    // place the camera at the center of all players
    Vector2 centerPos = getCenterPosition(all_player_positions);
    transform.position = new Vector3(centerPos.x, centerPos.y, -10f);
    // set the camera size to min
    Camera.main.orthographicSize = minSize;
}
// CASE 3: Getting all the players on screen isn't a problem
else {
    caseNumber = 3;
    // place the camera at the center of all players
    Vector2 centerPos = getCenterPosition(all_player_positions);
    transform.position = new Vector3(centerPos.x, centerPos.y, -10f);
    // set the camera size to fit all players
    Camera.main.orthographicSize = expantionGap + max_distance;
}

*/
