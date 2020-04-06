using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using EZCameraShake;
using DG.Tweening;
using NaughtyAttributes;
using System;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public bool isGameStarted;
    public bool isGameOver;
    public float gameTime;
    public float gameTimer;

    [Space]

    public float superSlowMoSpeed;

    [Space]

    public Material[] colors;
    public Material dedMaterial;
    public Color coinColor;
    public Color drawColor;

    [Header("Borders")]

    public Level currLevel;
    public Level[] levels;
    [Space]
    public float borderThickness;
    public Transform borderU;
    public Transform borderD;
    public Transform borderR;
    public Transform borderL;

    [Header("References")]

    public TextMeshPro timerText;
    public Transform timerAreaTrans;
    public TextMeshPro pressAnyKeyText;
    public GameObject corpseCleanerCollider;

    public ParticleSystem fireworkR;
    public ParticleSystem fireworkL;

    [Header("Realtime Lists")]

    public List<Peg> currPegs;
    public List<Crate> currCrates;
    public List<Health> currHealths;
    public List<Coin> currCoins;

    [HideInInspector]
    public int maxFrameNum;

    public static GameManager main;

    public Action OnGameOver;
    public Action OnGameInit;

    private void Awake()
    {
        main = this;
    }

    void Start()
    {
        //StartGameEnum();
    }

    public void SetBorders(Vector2 scale, Vector2 offset)
    {
        borderU.transform.localPosition = new Vector3(0, 0, scale.y + borderThickness) + offset.ToVector3();
        borderD.transform.localPosition = new Vector3(0, 0, -scale.y - borderThickness) + offset.ToVector3();
        borderR.transform.localPosition = new Vector3(scale.x + borderThickness, 0, 0) + offset.ToVector3();
        borderL.transform.localPosition = new Vector3(-scale.x - borderThickness, 0, 0) + offset.ToVector3();
    }

    IEnumerator GameLoopEnum()
    {
        AudioManager.main.StopMenuMusic();

        timerText.text = "READY";

        InitGame();

        AudioManager.main.Play("starter");

        yield return new WaitForSecondsRealtime(3);

        timerText.text = "GO";

        AudioManager.main.gameMusic.Play();

        StartGame();

        CameraShaker.Instance.ShakeOnce(2, 5, 0, 0.6f);

        gameTimer = gameTime;

        yield return new WaitForSecondsRealtime(1);

        while (gameTimer > 0)
        {
            gameTimer -= 1;

            timerText.text = gameTimer.ToString();

            if (gameTimer < 4)
            {
                timerText.DOColor(Color.red, 0.3f).SetLoops(2, LoopType.Yoyo);
                timerText.transform.DOPunchRotation(Vector3.forward * 40, 0.3f);
            }
            else
            {
                timerText.transform.DOPunchRotation(Vector3.forward * 17, 0.3f);
            }

            yield return new WaitForSecondsRealtime(1);
        }



        //TIMER RUNS OFF
        timerText.text = "STOOOOP";
        timerText.transform.DOPunchRotation(Vector3.forward * 17, 0.1f);

        corpseCleanerCollider.SetActive(true);

        float newTimeScale = 1;


        while (newTimeScale > superSlowMoSpeed)
        {
            newTimeScale -= Time.unscaledDeltaTime / 3f;


            if (newTimeScale <= .5f && !isGameOver)
            {
                GameOver();
                //HEHE

                AudioManager.main.gameMusic.Stop();

                Peg[] pegs = FindObjectsOfType<Peg>();

                print(pegs.Length);

                AudioManager.main.Play("explosion_big");

                for (int i = 0; i < pegs.Length; i++)
                {
                    print("Killin this boi " + pegs[i].gameObject.name);

                    if (pegs[i] && pegs[i].gameObject.activeInHierarchy)
                    {
                        pegs[i].Die();


                        yield return new WaitForSecondsRealtime(0.02f);
                    }
                }
            }

            if (newTimeScale <= superSlowMoSpeed)
            {
                newTimeScale = superSlowMoSpeed;
            }

            Time.timeScale = newTimeScale;
            Time.fixedDeltaTime = newTimeScale * 0.02f;

            yield return new WaitForEndOfFrame();
        }

        yield return new WaitForSecondsRealtime(2f);

        CameraController.main.holder.DOMove(new Vector3(0, 10, -2.48f), 1).SetUpdate(true);

        AudioManager.main.PlayStartChord();

        timerText.text = "THE WINNER IS...";
        //timerText.transform.DOScale(Vector3.one * 2, 0.946f).SetUpdate(true);

        float shakeDegree = 5;
        timerText.transform.localEulerAngles = new Vector3(90, 0, -shakeDegree);

        Tween shakeTween = timerText.transform.DORotate(new Vector3(90, 0, shakeDegree), 0.1f).SetLoops(-1, LoopType.Yoyo).SetUpdate(true);

        yield return new WaitForSecondsRealtime(2f);

        shakeTween.Kill();

        timerText.transform.localEulerAngles = new Vector3(90, 0, 0);

        AudioManager.main.PlayOtherChord();

        //SHOW EM WO WINS
        int winnerColor = FindTheWinner();

        Color col = Color.magenta;

        if (winnerColor != -1)
        {
            col = colors[winnerColor].color;
        }

        switch (winnerColor)
        {
            case -1:
                timerText.text = "ITS A DRAW";
                col = drawColor;
                break;
            case 0:
                timerText.text = "REDDD";
                break;
            case 1:
                timerText.text = "BLUEEE";
                break;
            case 2:
                timerText.text = "GREENNN";
                break;
            case 3:
                timerText.text = "YELLOWW";
                break;
        }

        fireworkL.startColor = col;
        fireworkR.startColor = col;
        fireworkL.Play();
        fireworkR.Play();

        timerText.color = col;

        timerText.transform.DOPunchScale(Vector3.one * 0.3f, 0.1f).SetUpdate(true);

        while (newTimeScale < 1)
        {
            newTimeScale += Time.unscaledDeltaTime / 2f;

            if (newTimeScale >= 1)
            {
                newTimeScale = 1;
            }

            Time.timeScale = newTimeScale;
            Time.fixedDeltaTime = newTimeScale * 0.02f;

            yield return new WaitForEndOfFrame();
        }

        yield return new WaitForSeconds(1);

        pressAnyKeyText.DOColor(drawColor, 0.6f);

        while (!Input.anyKey)
        {
            yield return new WaitForEndOfFrame();
        }

        RestartGame();
    }

    public void RestartGame()
    {
        Fader.main.FadeIn();

        Invoke("LoadScene", Fader.main.fadeSpeed);
    }

    public void LoadScene()
    {
        SceneManager.LoadScene(0, LoadSceneMode.Single);

    }

    int FindTheWinner()
    {
        int winnerColor = -1;
        int maxScore = 0;

        for (int i = 0; i < currLevel.coinAreas.Length; i++)
        {
            int score = currLevel.coinAreas[i].UpdateCoinCount();

            if (score > maxScore)
            {
                maxScore = score;
                winnerColor = i;
            }
        }

        return winnerColor;
    }

    public void GameOver()
    {
        isGameOver = true;

        OnGameOver?.Invoke();
    }

    public void StartGameEnum()
    {
        StartCoroutine(GameLoopEnum());
    }

    public void InitGame()
    {
        for (int i = 0; i < currLevel.coinAreas.Length; i++)
        {
            currLevel.coinAreas[i].color = i;
        }

        OnGameInit?.Invoke();
    }

    public void StartGame()
    {
        isGameStarted = true;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            RestartGame();
        }
    }
}
