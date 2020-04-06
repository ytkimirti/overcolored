using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial : MonoBehaviour
{
    public static bool tutorialShowed;
    public int currTutorial;

    Vector3 startPos;

    float timer;

    void Start()
    {
        if (tutorialShowed)
        {
            gameObject.SetActive(false);
        }

        startPos = transform.position;

        for (int i = 0; i < 5; i++)
        {
            transform.GetChild(i).transform.position = startPos + new Vector3(Screen.width * -i, 0, 0);
        }
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (Input.anyKeyDown && timer > 0.7f)
        {
            currTutorial++;

            timer = 0;
        }

        if (currTutorial == 6)
        {
            tutorialShowed = true;
            gameObject.SetActive(false);
            return;
        }

        Vector3 newPos = startPos + Vector3.right * Screen.width * currTutorial;

        transform.position = Vector3.Lerp(transform.position, newPos, 5 * Time.deltaTime);
    }
}
