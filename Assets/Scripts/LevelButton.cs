using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LevelButton : MonoBehaviour
{

    public UnityEvent onPressed;

    [Space]
    Animator anim;
    MeshFlasher meshFlasher;

    void Start()
    {
        anim = GetComponentInChildren<Animator>();
        meshFlasher = GetComponentInChildren<MeshFlasher>();
    }

    void Update()
    {

    }

    public void OnPressed()
    {
        AudioManager.main.Play("click");

        onPressed.Invoke();

        anim.SetTrigger("Press");

        meshFlasher.Flash();
    }
}
