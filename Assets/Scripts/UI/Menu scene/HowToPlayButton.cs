using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HowToPlayButton : MonoBehaviour
{
    public GameObject how_to_play_text_block;
    
    void Start(){
        how_to_play_text_block.SetActive(false);
    }

    void Update(){
        if (how_to_play_text_block.activeSelf == true && Input.anyKeyDown){
            how_to_play_text_block.SetActive(false);
        }
    }
    
    // called onClick
    public void loadhow_to_play()
    {
        how_to_play_text_block.SetActive(true);
    }
}
