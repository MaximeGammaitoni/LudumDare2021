using UnityEngine;

public class WinPanel : MonoBehaviour
{
    [SerializeField]
    GameObject _panel = null;

    void Awake()
    {
        EventsManager.StartListening(nameof(StatesEvents.OnWinIn), OnWin);
    }
    
    void OnDestroy()
    {
        EventsManager.StopListening(nameof(StatesEvents.OnWinIn), OnWin);
    }

    void OnWin(Args args)
    {
        _panel.SetActive(true);
    }
}