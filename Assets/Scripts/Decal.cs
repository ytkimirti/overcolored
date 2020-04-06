using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Decal : MonoBehaviour
{
    public MeshRenderer meshRenderer;
    public CircleCollider2D col;

    [HideInInspector]
    public int count;

    void Start()
    {
        //Burası baya karışık aslında .d

        //Decal check olayını performanstan kısmak için iki boyutlu yapıyorum ve voila, performancee
        col.offset = Vector2.up * (transform.position.z / transform.localScale.y);

        meshRenderer.transform.localScale = Vector3.zero;

        meshRenderer.transform.localPosition = (meshRenderer.transform.localPosition.y + (count * 0.0003f)) * Vector3.up;

        meshRenderer.transform.DOScale(Vector3.one, 0.2f);
    }

    void Update()
    {

    }
}