using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class CreditsButton : MonoBehaviour
{
    public GameObject credits_text_block;
    
    void Start(){
        credits_text_block.SetActive(false);
    }

    void Update(){
        if (credits_text_block.activeSelf == true && Input.anyKeyDown){
            credits_text_block.SetActive(false);
        }
    }
    
    // called onClick
    public void loadCredits()
    {
        credits_text_block.SetActive(true);
    }
}
