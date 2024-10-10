using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundLayer : MonoBehaviour
{
    /// <summary>
    /// Controls the deapth placement of the layer. If the deapth is 0.5 ->
    /// position = mainCamera.transform.position * deapth + cameraOffset;
    /// and movement will be divided by two. Therefore the lower deapth
    /// is the less the layer will move with the camera. (if deapth is equal
    /// to zero there will be no movement)
    /// </summary>
    public float deapth;

    GameObject mainCamera;

    private void Awake()
    {
        mainCamera = GameObject.Find("Main Camera");
    }

    // Update is called once per frame
    void Update()
    {
        if (deapth <= 0)
        {
            Destroy(this);
        }
        transform.position = (Vector2)mainCamera.transform.position * deapth;
    }
}
