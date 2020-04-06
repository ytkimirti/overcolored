using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crate : MonoBehaviour
{
    public MeshFlasher meshFlasher;
    public float knockbackMult;
    Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        GameManager.main.currCrates.Add(this);
    }

    void Update()
    {

    }

    public void OnDamage(Vector2 pos)
    {
        meshFlasher.Flash();

        //Add vel

        AudioManager.main.Play("hit");

        Vector2 dir = transform.position.ToVector2() - pos;

        dir.Normalize();

        rb.velocity += dir.ToVector3() * knockbackMult;
    }

    public void Die()
    {
        Instantiate(EntitySpawner.main.coinPrefab, transform.position, Quaternion.identity);

        ParticleManager.main.play(0, transform.position);

        AudioManager.main.Play("crate_explode");

        Destroy(gameObject);
    }
}
