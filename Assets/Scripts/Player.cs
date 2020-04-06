using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public int playerID;

    [HideInInspector]
    public Peg peg;


    void Awake()
    {
        peg = GetComponent<Peg>();

        if (playerID == 0)
        {
            playerID = 1;
        }

        peg.isPlayer = true;
        peg.playerNum = playerID;
    }

    void Update()
    {
        if (!peg || GameManager.main.isGameOver)
            return;

        peg.movementInput = new Vector2(Input.GetAxisRaw(string.Format("p{0}h", playerID)), Input.GetAxisRaw(string.Format("p{0}v", playerID)));

        if (Input.GetButtonDown(string.Format("p{0}f", playerID)))
        {
            peg.OnAttackPressed();
        }
    }
}
