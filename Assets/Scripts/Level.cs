using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour
{
    public Vector2 levelScale;
    public Vector2 levelOffset;

    public CoinArea[] coinAreas;

    public Transform[] spawnPoints;
    public Transform crateSpawnZone;

    void Start()
    {

    }

    void Update()
    {

    }

    public void Load()
    {
        gameObject.SetActive(true);
        GameManager.main.SetBorders(levelScale, levelOffset);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(Vector2.zero, levelScale.ToVector3() * 2);

        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(crateSpawnZone.position, crateSpawnZone.transform.localScale.ToVector2().ToVector3() * 2);
    }

    private void OnValidate()
    {
        FindObjectOfType<GameManager>().SetBorders(levelScale, levelOffset);
    }
}
