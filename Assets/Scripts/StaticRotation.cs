using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticRotation : MonoBehaviour
{
    void Start()
    {

    }

    void LateUpdate()
    {
        transform.rotation = Quaternion.identity;
    }
}
