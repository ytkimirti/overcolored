using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntitySpawner : MonoBehaviour
{
    [Header("Crates")]

    public float crateSpawnTime;
    public float crateSpawnTimeRandomness;
    public float crateSpawnHeight;
    float crateSpawnTimer;

    [Header("References")]

    public GameObject cratePrefab;
    public Coin coinPrefab;

    public static EntitySpawner main;

    private void Awake()
    {
        main = this;
    }

    void Start()
    {

    }

    void Update()
    {
        if (!GameManager.main.isGameStarted || GameManager.main.isGameOver)
            return;

        crateSpawnTimer -= Time.deltaTime;

        if (crateSpawnTimer < 0)
        {
            crateSpawnTimer = Random.Range(crateSpawnTime - crateSpawnTimeRandomness, crateSpawnTime + crateSpawnTimeRandomness);

            Vector3 pos = GetRandomPos().ToVector3();

            pos.y = crateSpawnHeight;


            GameObject crateGO = Instantiate(cratePrefab, pos, Random.rotation, transform);
        }
    }

    Vector2 GetRandomPos()
    {
        Vector2 scale = GameManager.main.currLevel.crateSpawnZone.localScale.ToVector2();
        Vector2 offset = GameManager.main.currLevel.crateSpawnZone.position.ToVector2();

        return new Vector2(Random.Range(-scale.x, scale.x) + offset.x, Random.Range(-scale.y, scale.y) + offset.y);
    }
}
