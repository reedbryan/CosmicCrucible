using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpGraphics : MonoBehaviour
{
    ParticleSystem PS;

    GameObject this_PickUp;

    private void Awake()
    {
        this_PickUp = transform.parent.gameObject;
    }
}
