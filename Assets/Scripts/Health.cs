using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    public bool isDed = false;
    public float health;
    public float maxHealth = 100;

    [HideInInspector]
    public HealthBar healthBar;

    [HideInInspector]
    public Peg peg;
    [HideInInspector]
    public Crate crate;

    private void Awake()
    {
        GameManager.main.OnGameInit += this.OnGameStarted;
    }

    void Start()
    {
        healthBar = GetComponentInChildren<HealthBar>();
        peg = GetComponent<Peg>();
        crate = GetComponent<Crate>();
        health = maxHealth;


    }

    void OnGameStarted()
    {
        if (!gameObject.activeInHierarchy)
            return;

        GameManager.main.currHealths.Add(this);
    }

    public void OnRespawned()
    {
        if (!isDed)
            return;

        isDed = false;

        GameManager.main.currHealths.Add(this);

        health = maxHealth;
    }

    void Update()
    {
        if (healthBar)
        {
            healthBar.currHealth = Mathf.Clamp01(health / maxHealth);
        }
    }

    public void GetDamage(float damage, Vector2 pos)
    {
        health -= damage;

        if (health <= 0)
        {
            Die();
        }

        peg?.OnDamage(damage, pos);

        crate?.OnDamage(pos);

        healthBar?.OnDamage();
    }

    public void Die()
    {
        if (isDed)
            return;

        isDed = true;

        GameManager.main.currHealths.Remove(this);

        peg?.Die();

        crate?.Die();
    }
}
