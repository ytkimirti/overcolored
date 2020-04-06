using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    public bool isHolded;
    public Peg currHolder;
    public int color;
    public float rotateSpeed;
    public float droppedVel;
    SphereCollider col;
    Rigidbody rb;
    public Transform visualTrans;
    float currRot;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<SphereCollider>();

        color = -1;

        AddRandomVel();

        GameManager.main.currCoins.Add(this);
    }

    public void Update()
    {

    }

    public void CheckArea()
    {
        color = -1;

        foreach (CoinArea area in GameManager.main.currLevel.coinAreas)
        {
            if (area.CheckContains(transform.position.ToVector2()))
            {
                color = area.color;
                break;
            }
        }
    }

    public void GetHolded(Peg holder)
    {
        if (isHolded)
        {
            return;
        }

        ParticleManager.main.play(3, transform.position, GameManager.main.coinColor);

        AudioManager.main.Play("coin");

        currHolder = holder;

        col.enabled = false;
        isHolded = true;
        rb.isKinematic = true;
    }

    public void GetDropped()
    {
        if (!isHolded)
        {
            return;
        }

        AudioManager.main.Play("coin");

        currHolder = null;

        CheckArea();

        rb.isKinematic = false;
        isHolded = false;
        col.enabled = true;

        AddRandomVel();
        //transform.position = new Vector3(transform.position.x, 0, transform.position.z);
    }

    void AddRandomVel()
    {
        rb.velocity = (Random.insideUnitCircle * droppedVel).ToVector3() + Vector3.up * droppedVel;
    }

    void LateUpdate()
    {
        currRot += Time.deltaTime * rotateSpeed;

        transform.eulerAngles = Vector3.up * currRot;
    }
}
