using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class AI : MonoBehaviour
{
    [System.Serializable]
    public enum taskType
    {
        attack,
        crate,
        collectCoin,
        deliverCoin,
        idle,
        playFortnite
    }

    [Header("Priorities")]
    public float enemyMult;
    public float coinMult;
    public float crateMult;

    [Header("Behaviour")]
    public float stopDistance;
    public float attackDistance;
    public float coinDistance;
    public float coinDropDistance;
    public float speedMult;

    public float randomWaitTime;

    [Space]

    public float randomAttackDelay;
    float attackTimer;

    [Header("Current Task")]

    public taskType currTask;
    public Transform currTarget;

    [HideInInspector]
    public Peg peg;

    void Start()
    {
        peg = GetComponent<Peg>();
    }

    public Transform FindClosest(List<Transform> transList, out float closestDist)
    {
        if (transList == null || transList.Count == 0)
        {
            closestDist = -1;
            return null;
        }

        if (transList.Count == 1)
        {
            closestDist = Vector2.Distance(transform.position.ToVector2(), transList[0].position.ToVector2());
            return transList[0];
        }

        int closestID = -1;
        closestDist = 100000f;

        for (int i = 0; i < transList.Count; i++)
        {
            float dist = (transList[i].position.ToVector2() - transform.position.ToVector2()).sqrMagnitude;

            if (dist < closestDist)
            {
                closestID = i;
                closestDist = dist;
            }
        }

        closestDist = Mathf.Sqrt(closestDist);

        return transList[closestID];
    }

    public void UpdateTask()
    {
        if (currTask == taskType.idle)
        {
            return;
        }

        if (peg.currCoin)
        {
            currTask = taskType.deliverCoin;
            currTarget = GameManager.main.currLevel.coinAreas[peg.color].transform;
            return;
        }

        float crateScore = 0;
        float enemyScore = 0;
        float coinScore = 0;

        Transform crateTrans = FindClosest(GameManager.main.currCrates.Where(x => x != null).Select(x => x.gameObject.transform).ToList(), out crateScore);

        Transform coinTrans = FindClosest(GameManager.main.currCoins.Where(x => x != null && x.color != peg.color && x != peg.currCoin && !(x.isHolded && x.currHolder.color == peg.color)).Select(x => x.gameObject.transform).ToList(), out coinScore);

        Transform enemyTrans = FindClosest(GameManager.main.currPegs.Where(x => x != null && !x.isInvisible && x.color != peg.color && x != peg)
                                                      .Select(x => x.gameObject.transform).ToList(), out enemyScore);


        crateScore = crateMult * (1 / crateScore);
        enemyScore = crateMult * (1 / enemyScore);
        coinScore = coinMult * (1 / coinScore);

        //print("Enemy score " + enemyScore);
        //print("Crate score " + crateScore);
        //print("Coin score " + coinScore);

        if (enemyScore > crateScore && enemyScore > coinScore)
        {
            currTarget = enemyTrans;
            currTask = taskType.attack;
        }
        else if (crateScore > enemyScore && crateScore > coinScore)
        {
            currTarget = crateTrans;
            currTask = taskType.crate;
        }
        else
        {
            currTarget = coinTrans;
            currTask = taskType.collectCoin;
        }
    }

    public void RemoveTask()
    {
        currTarget = null;
        currTask = taskType.idle;
    }

    public void DoTask()
    {
        float newDeltaTime = (GameManager.main.maxFrameNum + 1) * Time.deltaTime;

        if (currTask == taskType.idle || !currTarget || peg.isStunned)
        {
            peg.movementInput = Vector2.zero;
            return;
        }


        peg.movementInput = currTarget.position.ToVector2() - transform.position.ToVector2();

        float dist = Vector2.Distance(transform.position.ToVector2(), currTarget.position.ToVector2());

        if (currTask == taskType.attack || currTask == taskType.crate)
        {
            if (dist < attackDistance)
            {
                attackTimer -= newDeltaTime;

                if (attackTimer <= 0)
                {
                    peg.OnAttackPressed();
                    attackTimer = Random.Range(randomAttackDelay / 2, randomAttackDelay);

                    IdleWait(0.1f);
                }
            }
            else
            {
                if (attackTimer < randomAttackDelay)
                    attackTimer += newDeltaTime;
            }
        }
        else if (currTask == taskType.collectCoin && dist < coinDistance)
        {
            peg.OnAttackPressed();

            RandomWait();
        }
        else if (currTask == taskType.deliverCoin && dist < coinDropDistance)
        {
            peg.OnAttackPressed();

            RandomWait();
        }


        peg.movementInput = peg.movementInput.normalized * speedMult;
    }

    public void RandomWait()
    {
        IdleWait(Random.Range(randomWaitTime / 2, randomWaitTime));
    }

    public void IdleWait(float time)
    {
        StartCoroutine(waitEnum(time));
    }

    IEnumerator waitEnum(float time)
    {
        currTask = taskType.idle;

        yield return new WaitForSeconds(time);

        currTask = taskType.playFortnite;

        UpdateTask();
    }

    void Update()
    {
        if (peg.isDed || GameManager.main.isGameOver || !GameManager.main.isGameStarted)
            return;

        //Frame check, for optimization :P

        if (Time.frameCount % (GameManager.main.maxFrameNum + 1) == peg.frameNum)
        {
            UpdateTask();

            DoTask();
        }
    }
}
