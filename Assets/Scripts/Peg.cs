using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using EZCameraShake;
using DG.Tweening;

public class Peg : MonoBehaviour
{
    public bool isPlayer;
    [HideInInspector]
    public int playerNum;
    public bool isDed;
    public int color;
    public bool isInvisible;

    public string nickname;

    [Header("Input")]
    public Vector2 movementInput;

    [Header("Movement")]
    public float movementSpeed;
    public float movementSmoothing;
    public float minSpeed;
    public float slideSmoothing;

    [Header("Attack")]

    public float attackDelay;
    public float attackDamage;
    public Transform attackPoint;
    public float attackRadius;
    public LayerMask attackLayer;

    public LayerMask buttonLayer;
    public LayerMask teamPlaneLayer;

    [Space]

    public float knockbackVelPerDamage;
    public bool isStunned;
    public float attackStun;
    public float damageStun;
    float stunTimer;

    [Header("Coin Holding")]
    public Coin currCoin;
    public LayerMask coinLayer;
    public float coinHoldHeight;

    [Space]
    public Vector2 slideVel;
    public float slideVelLerp;
    public MeshRenderer meshRen;
    public MeshFlasher meshFlasher;
    public Animator attackAnim;
    [HideInInspector]
    public Health health;

    public TextMeshPro infoText;
    public float respawnDelay;
    public Vector2 dieForce;
    public float dieTorque;

    //Its a different number for every peg in the level
    [HideInInspector]
    public int frameNum;

    public TextMeshPro playerText;
    public Animator playerArrowAnim;
    public ParticleSystem smokeParticle;
    Vector2 smoothingVel;
    Rigidbody rb;

    void Awake()
    {
        if (GameManager.main)
        {
            SubscribeActions();
        }
    }

    void Start()
    {
        health = GetComponent<Health>();
        rb = GetComponent<Rigidbody>();

        SubscribeActions();

        if (isPlayer)
            playerText.text = "P" + playerNum;
    }

    void SubscribeActions()
    {
        GameManager.main.OnGameOver += ResetEnums;
        GameManager.main.OnGameInit += this.OnGameStarted;
    }

    public void OnGameStarted()
    {
        if (!gameObject.activeInHierarchy)
        {
            GameManager.main.OnGameOver -= this.ResetEnums;

            Destroy(gameObject);
            return;
        }

        SetColor();

        if (isPlayer)
        {
            playerText.color = GameManager.main.colors[color].color;
            playerArrowAnim.SetBool("GameStart", true);
        }

        frameNum = GameManager.main.currPegs.Count;

        if (frameNum > GameManager.main.maxFrameNum)
        {
            GameManager.main.maxFrameNum = frameNum;
        }

        GameManager.main.currPegs.Add(this);
    }

    void ResetEnums()
    {
        if (!gameObject.activeInHierarchy)
            return;

        CancelInvoke();
        StopAllCoroutines();
    }

    void SetColor()
    {
        meshRen.material = GameManager.main.colors[color];
    }

    void Update()
    {
        if (isDed)
            return;

        isInvisible = DecalCheck();

        stunTimer -= Time.deltaTime;

        if (stunTimer < 0)
            stunTimer = 0;

        isStunned = stunTimer > 0;

        Movement();

        SetDir();
    }

    public bool DecalCheck()
    {

        int layerMask = 1 << (20 + color);

        return Physics2D.OverlapPoint(transform.position.ToVector2(), layerMask) != null;
    }

    public void OnAttackPressed()
    {
        if (isDed)
            return;

        TryToPressButton();

        if (currCoin)
        {
            DropCoin();
        }
        else
        {
            TryToPickUpCoin();
        }

        if (currCoin)
            return;

        if (stunTimer > 0)
        {
            return;
        }

        attackAnim.SetTrigger("attack");

        Attack();
    }

    public void TryToPressButton()
    {
        Collider[] buttons = Physics.OverlapSphere(transform.position, 0.7f, buttonLayer);

        if (buttons.Length > 0)
        {
            LevelButton button = buttons[0].GetComponent<LevelButton>();

            if (button)
            {
                button.OnPressed();
            }
        }
    }

    public int CheckCurrTeam(Collider[] teamPlanes)
    {
        for (int i = 0; i < teamPlanes.Length; i++)
        {
            if (teamPlanes[i].gameObject.activeInHierarchy && teamPlanes[i].bounds.Contains(transform.position))
            {
                return i;
            }
        }

        return -1;
    }

    public void TryToPickUpCoin()
    {
        if (currCoin)
            return;

        Collider[] coinCols = Physics.OverlapSphere(transform.position, attackRadius, coinLayer);

        if (coinCols.Length == 0)
            return;

        currCoin = coinCols[0].gameObject.GetComponent<Coin>();

        if (!currCoin || currCoin.isHolded)
        {
            currCoin = null;
            return;
        }

        currCoin.GetHolded(this);

        currCoin.transform.position = transform.position + (Vector3.up * coinHoldHeight);

        currCoin.transform.parent = transform;
    }

    public void DropCoin()
    {
        if (!currCoin)
            return;

        currCoin.transform.parent = null;

        currCoin.GetDropped();

        currCoin = null;
    }

    void Attack()
    {
        meshFlasher.Flash(1);

        stunTimer = attackStun;

        if (!GameManager.main.isGameStarted)
            return;

        List<Health> healths = FindHealths(attackPoint.position, attackRadius);

        for (int i = 0; i < healths.Count; i++)
        {
            healths[i].GetDamage(attackDamage, transform.position.ToVector2());
        }
    }

    private void OnDrawGizmos()
    {
        //Gizmos.color = Color.red;
        //Gizmos.DrawWireSphere(transform.position.ToVector2(), 0.1f);
    }

    public List<Health> FindHealths(Vector3 pos, float radius)
    {
        Collider[] cols = Physics.OverlapSphere(pos, radius, attackLayer);

        List<Health> healths = new List<Health>();

        for (int i = 0; i < cols.Length; i++)
        {
            Health h = cols[i].gameObject.GetComponent<Health>();

            if (!h && cols[i].gameObject.transform.parent)
            {
                h = cols[i].gameObject.transform.parent.gameObject.GetComponent<Health>();
            }

            if (h && h != health)
            {
                healths.Add(h);
            }
        }

        return healths;
    }

    public void Die()
    {
        if (isDed)
            return;

        //if (isPlayer)
        //playerArrowAnim.gameObject.SetActive(false);

        smokeParticle.Play();

        CameraShaker.Instance.ShakeOnce(isPlayer ? 2 : 1f, 3, 0, 0.6f);

        ParticleManager.main.play(1, transform.position, GameManager.main.colors[color].color);

        DropCoin();

        GameManager.main.currPegs.Remove(this);


        isDed = true;

        meshFlasher.isActive = false;
        meshFlasher.CancelFlash();

        meshRen.material = GameManager.main.dedMaterial;

        slideVel = Vector2.zero;

        rb.constraints = RigidbodyConstraints.None;

        //Add the force

        if (GameManager.main.isGameOver)
        {
            //Means this is the final DEATH

            rb.velocity = ((transform.position - GameManager.main.timerText.transform.position).normalized.ToVector2() * dieForce.x).ToVector3() + Vector3.up * dieForce.y;
        }
        else
        {
            AudioManager.main.Play("explosion");
            rb.velocity = Vector3.up * dieForce.y;
        }


        rb.angularVelocity = (Random.insideUnitCircle).normalized.ToVector3() * dieTorque;

        if (!GameManager.main.isGameOver)
            StartCoroutine(RespawnEnum());
    }

    IEnumerator RespawnEnum()
    {
        yield return new WaitForSeconds(respawnDelay);

        transform.DOMove(GameManager.main.currLevel.spawnPoints[color].GetChild(0).position + Vector3.up * 0.25f, 1);

        transform.DORotate(Vector3.zero, 1);

        infoText.text = "RESPAWN IN";

        yield return new WaitForSeconds(1);

        infoText.text = "3";

        yield return new WaitForSeconds(1);

        infoText.text = "2";

        yield return new WaitForSeconds(1);

        infoText.text = "1";

        yield return new WaitForSeconds(1);

        infoText.text = "";

        Respawn();
    }

    public void Respawn()
    {
        if (!isDed)
            return;

        smokeParticle.Stop();

        if (isPlayer)
        {
            playerArrowAnim.gameObject.SetActive(true);

            playerArrowAnim.SetTrigger("Respawn");
        }

        isDed = false;

        meshFlasher.isActive = true;

        isInvisible = false;

        isStunned = false;
        stunTimer = 0;

        currCoin = null;

        rb.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionY;

        transform.position = new Vector3(transform.position.x, 0, transform.position.z);

        transform.rotation = Quaternion.identity;

        health.health = health.maxHealth;

        GameManager.main.currPegs.Add(this);

        health.OnRespawned();

        SetColor();

        meshFlasher.Flash(16);
    }

    public void OnDamage(float dmg, Vector2 pos)
    {
        if (isDed)
        {
            //This means we are at the SAME frame that we just died
            if (rb.velocity.y == dieForce.y)
            {
                rb.velocity = ((transform.position.ToVector2() - pos).normalized * dieForce.x).ToVector3() + rb.velocity;
            }

            return;
        }

        if (stunTimer < damageStun)
            stunTimer = damageStun;

        isStunned = true;

        DropCoin();

        AudioManager.main.Play("hit");

        CameraShaker.Instance.ShakeOnce(isPlayer ? 2 : 0.4f, 5, 0, 0.2f);

        ParticleManager.main.play(3, transform.position, GameManager.main.colors[color].color);

        DecalSpawner.main.SpawnDecal(transform.position.ToVector2(), 1.2f, color);

        meshFlasher.Flash();

        //Add vel

        Vector2 dir = transform.position.ToVector2() - pos;

        dir.Normalize();

        AddVel(dir * knockbackVelPerDamage * dmg);
    }

    void SetDir()
    {
        if (movementInput == Vector2.zero)
            return;

        float rot = -movementInput.ToAngle() + 90;

        transform.localEulerAngles = Vector3.up * rot;
    }

    public void AddVel(Vector2 vel)
    {
        slideVel += vel;
    }

    void Movement()
    {
        movementInput = Vector2.ClampMagnitude(movementInput, 1);

        Vector2 newVel = Vector2.zero;

        if (movementInput == Vector2.zero && rb.velocity.magnitude < minSpeed)
        {
            newVel = Vector2.Lerp(rb.velocity.ToVector2(), Vector2.zero, Time.deltaTime * slideSmoothing);
        }
        else
        {
            Vector2 targetVel = movementInput * movementSpeed;

            newVel = Vector2.SmoothDamp(rb.velocity.ToVector2(), targetVel, ref smoothingVel, movementSmoothing);
        }

        //Slide vel
        slideVel = Vector2.Lerp(slideVel, Vector2.zero, slideVelLerp * Time.deltaTime);

        rb.velocity = newVel.ToVector3() + slideVel.ToVector3();

    }
}
