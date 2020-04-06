using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DecalSpawner : MonoBehaviour
{

    public GameObject decalPrefab;

    public static DecalSpawner main;

    int decalCounter;

    private void Awake()
    {
        main = this;
    }

    void Start()
    {

    }

    public void SpawnDecal(Vector2 pos, float radius, int color)
    {
        GameObject decalGO = Instantiate(decalPrefab, pos.ToVector3(), Quaternion.identity, transform);

        decalGO.transform.localScale = Vector3.one * radius;

        //int layerMask = 1 << (20 + color);

        decalGO.layer = 20 + color;

        Decal decal = decalGO.GetComponent<Decal>();

        decal.meshRenderer.material = GameManager.main.colors[color];

        decal.count = decalCounter;

        decalCounter++;
    }

    void Update()
    {

    }
}
