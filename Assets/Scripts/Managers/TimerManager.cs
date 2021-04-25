using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TimerManager
{
    private readonly TextMeshProUGUI timerText;
    private readonly int remainingTime;
    private float timer;
    public TimerManager()
    {
        timerText = GameManager.singleton.ResourcesLoaderManager.CanvasElements.TimerText;
        remainingTime = GameManager.singleton.ResourcesLoaderManager.GameConfig.RemainingTime;
        timer = remainingTime;
        GameManager.GameUpdateHandler += UpdateTimer;
    }

    private void UpdateTimer()
    {
        timer -= Time.deltaTime * Time.timeScale;
        var ts = TimeSpan.FromSeconds(timer);
        if (timer <= 0f)
        {
            GameManager.singleton.PlayerEvents.PlayerIsDead();
        }
        timerText.text = string.Format("{0:D2}:{1:D2}",
                ts.Minutes,
                ts.Seconds);
    }

}
