using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CoinArea : MonoBehaviour
{
    [HideInInspector]
    public int color;

    public int currCoinCount;

    [Space]

    public TextMeshPro coinCountText;
    BoxCollider col;

    void Start()
    {
        col = GetComponent<BoxCollider>();


    }

    void Update()
    {
        coinCountText.text = currCoinCount.ToString();
    }

    public bool CheckContains(Vector2 pos)
    {
        return col.bounds.Contains(pos.ToVector3());
    }

    public int UpdateCoinCount()
    {
        int count = 0;

        foreach (Coin coin in GameManager.main.currCoins)
        {
            if (coin)
            {
                if (col.bounds.Contains(coin.transform.position))
                {
                    coin.color = color;
                    count++;
                }
                else
                {
                    coin.CheckArea();
                }
            }
        }

        currCoinCount = count;

        return currCoinCount;
    }

    private void OnTriggerEnter(Collider other)
    {
        UpdateCoinCount();
    }

    private void OnTriggerExit(Collider other)
    {
        UpdateCoinCount();
    }


}
