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
        EventsManager.StartListening(nameof(LevelLoader.OnGameFinished), StopUpdate);
    }

    ~TimerManager()
    {
        EventsManager.StopListening(nameof(LevelLoader.OnGameFinished), StopUpdate);
    }

    void StopUpdate(Args args = null)
    {
        GameManager.GameUpdateHandler -= UpdateTimer;
    }

    private void UpdateTimer()
    {
        if (timer <= 0f)
        {
            return;
        }
        
        timer -= Time.deltaTime * Time.timeScale;
        var ts = TimeSpan.FromSeconds(timer);
        if (timer <= 0f)
        {
            StopUpdate();
            timer = 0f;
            GameManager.singleton.PlayerEvents.PlayerIsDead();
        }
        timerText.text = string.Format("{0:D2}:{1:D2}",
                ts.Minutes,
                ts.Seconds);
    }

    public void RemoveTime()
    {
        timer -= 5f;
        var ts = TimeSpan.FromSeconds(timer);
        timerText.text = string.Format("{0:D2}:{1:D2}",
        ts.Minutes,
        ts.Seconds);

    }

    public int timeLeft => Mathf.Max(0, (int)timer);

}
