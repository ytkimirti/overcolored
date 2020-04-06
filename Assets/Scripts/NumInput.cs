using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Events;

public class NumInput : MonoBehaviour
{
    public int number;
    [Space]
    public int min;
    public int max;

    [Space]

    public UnityEvent onChange;

    [Space]

    public TextMeshPro numText;
    public MeshFlasher meshFlasher;

    void Awake()
    {
        if (PlayerPrefs.HasKey(name))
        {
            number = PlayerPrefs.GetInt(name);
        }

        ClampNum();

        numText.text = number.ToString();
    }

    private void Start()
    {
        GameManager.main.OnGameInit += SaveSettings;
    }

    void Update()
    {

    }

    public void ClampNum()
    {
        number = Mathf.Clamp(number, min, max);
    }

    public void SaveSettings()
    {
        PlayerPrefs.SetInt(name, number);
    }

    public void UpdateNum()
    {
        ClampNum();

        onChange.Invoke();

        numText.text = number.ToString();
        meshFlasher.Flash();
    }

    public void Increase()
    {
        if (number < max)
        {
            number++;
            UpdateNum();
        }
    }

    public void Decrease()
    {
        if (number > min)
        {
            number--;
            UpdateNum();
        }
    }
}
