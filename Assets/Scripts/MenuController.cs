using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using EZCameraShake;
using DG.Tweening;

public class MenuController : MonoBehaviour
{
    public NumInput teamCount;
    public NumInput teamSize;
    public NumInput playerCount;

    [Space]

    public Player[] players;
    public BoxCollider[] teamPlanes;
    public GameObject warningText;

    [Header("Level Animation")]

    public float dropHeight;

    [Space]

    public GameObject pegPrefab;
    public TextMeshPro infoText;
    public Transform pegStartTrans;
    public Animator menuAnim;

    public static MenuController main;

    void Start()
    {
        UpdatePlayers();
        UpdateTeamPlanes();

        SetNormalText();

        GameManager.main.SetBorders(new Vector2(30, 30), Vector2.zero);
    }

    void Update()
    {
        warningText.SetActive(teamSize.number > 2);

        if (CanStart())
        {
            infoText.text = "PRESS SPACE TO START";

            if (Input.GetKeyDown(KeyCode.Space))
            {
                AttemptStart();
            }
        }
        else
        {
            SetNormalText();
        }
    }

    public void SpawnPeg(int color, Vector2 spawnPos, Vector2 targetPos)
    {
        GameObject pegGo = Instantiate(pegPrefab, spawnPos.ToVector3() + Vector3.up * 0.25f, Quaternion.identity);

        Peg peg = pegGo.GetComponent<Peg>();

        peg.color = color;

        peg.transform.DOMove(targetPos.ToVector3() + Vector3.up * 0.25f, 1);
    }

    public void SpawnPegsInTeam(int color, int pegCount, Level level)
    {
        for (int i = 0; i < pegCount; i++)
        {
            //First spawn point is left for the players
            SpawnPeg(color, pegStartTrans.position.ToVector2(), findSpawnPoint(color, level).ToVector2());
        }
    }

    Vector3 findSpawnPoint(int color, Level level)
    {
        foreach (Transform sp in level.spawnPoints[color])
        {
            if (sp.name != "taken")
            {
                sp.name = "taken";
                return sp.transform.position.ToVector2().ToVector3() + Vector3.up * 0.25f;
            }
        }

        Debug.LogError("Not enough spawn points!");

        return Vector3.zero;
    }

    public void SpawnPegs(Level level, int[] playerCounts)
    {
        for (int i = 0; i < teamCount.number; i++)
        {
            int pegCount = teamSize.number - playerCounts[i];

            SpawnPegsInTeam(i, pegCount, level);
        }
    }

    public void LoadLevel()
    {
        //Lets say it's 2, then we'll load the level in index 0. Since you cant play it with 1 team .d
        Level level = GameManager.main.levels[teamCount.number - 2];

        GameManager.main.currLevel = level;

        level.Load();

        GameManager.main.timerAreaTrans.gameObject.SetActive(true);

        level.transform.position = Vector3.up * dropHeight;
        GameManager.main.timerAreaTrans.position = Vector3.up * dropHeight;

        level.transform.DOMoveY(0, 1);
        GameManager.main.timerAreaTrans.DOMoveY(0, 1);

        //A list to keep an eye on player count on every team
        int[] teamPlayerCounts = new int[4];

        for (int i = 0; i < playerCount.number; i++)
        {
            //Increase the player count on that color
            teamPlayerCounts[players[i].peg.color]++;

            //Move it to its spawn pos
            players[i].transform.DOMove(findSpawnPoint(players[i].peg.color, level), 1);
        }

        SpawnPegs(level, teamPlayerCounts);

        menuAnim.SetTrigger("Close");

        GameManager.main.StartGameEnum();
    }

    void SetNormalText()
    {
        infoText.text = "SELECT A TEAM BY STANDING ON ITS COLOR";
    }

    IEnumerator wrongEnum()
    {
        infoText.text = "EVERY PLAYER SHOULD SELECT A TEAM";
        infoText.color = Color.red;
        CameraShaker.Instance.ShakeOnce(2, 3, 0, 0.3f);

        yield return new WaitForSeconds(2f);

        SetNormalText();
    }

    public void UpdatePlayers()
    {
        for (int i = 0; i < players.Length; i++)
        {
            players[i].gameObject.SetActive(i < playerCount.number);
        }
    }

    public void UpdateTeamPlanes()
    {
        for (int i = 0; i < teamPlanes.Length; i++)
        {
            teamPlanes[i].gameObject.SetActive(i < teamCount.number);
        }
    }

    public bool CanStart()
    {
        for (int i = 0; i < playerCount.number; i++)
        {
            if (players[i].peg.CheckCurrTeam(teamPlanes) == -1)
            {
                return false;
            }
        }

        return true;
    }

    public void AttemptStart()
    {
        for (int i = 0; i < playerCount.number; i++)
        {
            if (players[i].peg.CheckCurrTeam(teamPlanes) == -1)
            {
                return;
            }
        }

        for (int i = 0; i < playerCount.number; i++)
        {
            players[i].peg.color = players[i].peg.CheckCurrTeam(teamPlanes);
        }

        print("GAME STARTED");

        LoadLevel();
    }
}
