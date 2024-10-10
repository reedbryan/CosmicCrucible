using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Testing : MonoBehaviour
{
    public GameObject obj;

    // Update is called once per frame
    void Update()
    {
        General.RotateTowards(transform, obj.transform.position, 3f);
    }
}
