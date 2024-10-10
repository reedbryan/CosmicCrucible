using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayButton : MonoBehaviour
{    
    // called onClick
    public void lauchGame(){
        SceneManager.LoadScene("InGame");
    }
}