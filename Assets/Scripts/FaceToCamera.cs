using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceToCamera : MonoBehaviour
{
    Transform camTrans;

    void Start()
    {
        camTrans = CameraController.main.cam.transform;
    }

    void LateUpdate()
    {
        transform.LookAt(camTrans, Vector3.up);
    }
}
