using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EZCameraShake;

public class CameraController : MonoBehaviour
{

    public bool follow;
    public float lerpSpeed;

    [Space]

    public Vector2 cameraFollowOffset;
    public float playersFollowBias;

    [Space]

    public float holderAngle;
    public Vector2 holderOffset;

    [Header("References")]

    public Camera cam;
    public Transform holder;

    Player[] players;

    public static CameraController main;

    private void Awake()
    {
        main = this;
    }


    void Start()
    {
        UpdateHolder();

        GameManager.main.OnGameOver += this.OnGameOver;
        GameManager.main.OnGameInit += this.OnGameStarted;
    }

    void OnGameStarted()
    {
        players = FindObjectsOfType<Player>();

        follow = true;
    }

    void OnGameOver()
    {
        //cam.gameObject.GetComponent<CameraShaker>().enabled = false;
    }

    private void OnValidate()
    {
        UpdateHolder();

    }

    void UpdateHolder()
    {
        holder.localPosition = new Vector3(0, holderOffset.y, holderOffset.x);
        holder.localEulerAngles = Vector3.right * holderAngle;
    }

    void LateUpdate()
    {
        if (GameManager.main.isGameOver || !follow)
        {
            return;
        }

        Vector2 targetPos = findTargetPos() + cameraFollowOffset;

        transform.position = Vector3.Lerp(transform.position, targetPos.ToVector3(), Time.deltaTime * lerpSpeed);
    }

    Vector2 findTargetPos()
    {
        int validCount = 0;
        Vector2 posSum = Vector2.zero;

        foreach (Health health in GameManager.main.currHealths)
        {
            if (health && health.transform.position.y < 5)
            {
                validCount++;
                posSum = posSum + health.transform.position.ToVector2();
            }
        }

        if (validCount == 0)
        {
            return Vector2.zero;
        }

        Vector2 healthsPos = posSum / validCount;

        posSum = Vector2.zero;

        foreach (Player player in players)
        {
            posSum += player.transform.position.ToVector2();
        }

        Vector2 playersPos = posSum / players.Length;

        return Vector2.Lerp(healthsPos, playersPos, playersFollowBias);
    }
}
