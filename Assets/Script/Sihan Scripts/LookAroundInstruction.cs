using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAroundInstruction : MonoBehaviour
{
    public GameObject UIInstruction;
    public GameObject Photo;
    public float timeRemaining = 6;
    private bool timerIsRunning = false;
    private void Start()
    {
        timerIsRunning = true;
    }
    private void Update()
    {
        if (timerIsRunning)
        {
            if (timeRemaining > 0)
            {
                timeRemaining -= Time.deltaTime;
            }
            else
            {
                TimerEnded();
                timeRemaining = 0;
                timerIsRunning = false;
            }
        }
    }

    private void TimerEnded()
    {
        UIInstruction.SetActive(false);
        Photo.SetActive(true);

    }

}
