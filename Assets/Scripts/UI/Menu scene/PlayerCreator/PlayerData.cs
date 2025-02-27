using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class for storing static data about the player(s)
/// </summary>
public class PlayerData : MonoBehaviour
{
    /// <summary>
    /// Index to get the base values of any given statistic for the player
    /// </summary>
    public static Dictionary<string, float> base_values = new Dictionary<string, float>
    {
        { "thrust", 17f },
        { "drag", 1f },
        { "rotation speed", 15f },
        { "fire rate", 0.15f },  // Use integer; convert if needed for display (0.1 as a float isn't allowed in an int dictionary)
        { "damage", 10f },
        { "speed", 200f },
        { "accuracy", 5f },
        { "count", 1f },
        { "missile cooldown", 5f },
        { "boost cooldown", 3f }
    }; 
    
    public static string testingData;

    public static Dictionary<string, float> keyboard_player;
    public static List<Dictionary<string, float>> controller_players;
}
