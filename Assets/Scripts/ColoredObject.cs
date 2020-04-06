using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColoredObject : MonoBehaviour
{
    [Range(0, 6)]
    public int color;

    public bool yAndZEqual;

    void Start()
    {
        MeshRenderer meshRen = GetComponent<MeshRenderer>();

        if (yAndZEqual)
        {
            transform.localScale = new Vector3(transform.localScale.x, transform.localScale.z, transform.localScale.z);
        }

        Collider2D col = GetComponent<Collider2D>();

        if (col)
        {
            col.offset = Vector2.up * (transform.position.z / transform.localScale.y);
        }

        meshRen.material = GameManager.main.colors[color];

        gameObject.layer = 20 + color;
    }

    void OnValidate()
    {
        MeshRenderer meshRen = GetComponent<MeshRenderer>();

        meshRen.material = FindObjectOfType<GameManager>().colors[color];
    }

    void Update()
    {

    }
}
