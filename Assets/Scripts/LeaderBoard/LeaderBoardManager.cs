using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;


[System.Serializable]
public class LeadBoardData
{
    public int id;
    public string name;
    public int score;
}

[System.Serializable]
public class GetLeadBoardResult
{
    public List<LeadBoardData> lead_boards;
}

public class LeaderBoardManager
{
    private GameObject Line;
    private GameObject LeaderBoardContent;

    private Coroutine AllLeadBoard = null;
    const string BaseUrl = "http://51.91.99.249"; //If you get this url to add your score manually, the Lord of the hell Paimon will come to you cheaty boy.
    const string GetAllEndPoint = "/leader_board/";

    // Start is called before the first frame update
    public LeaderBoardManager()
    {
        Line = GameManager.singleton.ResourcesLoaderManager.UIElementsLoader.LeaderBoardUiElement;
        LeaderBoardContent = GameManager.singleton.ResourcesLoaderManager.CanvasElements.LeaderBoardContent;
        EventsManager.StartListening(nameof(EnterName.OnNameEntered), OnNameEntered);
    }

    public void GetRequestAndInstantiateIntoCanvas()
    {
        GameManager.singleton.StartCoroutine(GetRequestAndInstantiateIntoCanvasCorout());
    }

    void OnNameEntered(Args args)
    {
        if (args is NameArgs nameArgs)
        {
            GameManager.singleton.StartCoroutine(OnNameEnteredCoroutine(nameArgs.name));
        }
    }

    IEnumerator OnNameEnteredCoroutine(string name)
    {
        LeadBoardData data = null;
        yield return FetchLeaderBoardData(name, res => { data = res; });
        int currentScore = ScoreManager.GetCurrentScore();
        if (data == null || currentScore > data.score)
        {
            // Post data
            LeadBoardData newData = new LeadBoardData()
            {
                name = name,
                score = currentScore
            };
            yield return PostLeaderBoardData(newData);
        }
        GetRequestAndInstantiateIntoCanvas();
    }

    IEnumerator FetchLeaderBoardData(string name, Action<LeadBoardData> callback)
    {
        LeadBoardData data = null;
        using (UnityWebRequest webRequest = UnityWebRequest.Get(BaseUrl + GetAllEndPoint + name))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.ConnectionError)
            {
                Debug.Log("Error: "  + webRequest.error);
            }
            else
            {
                try
                {
                    data = JsonUtility.FromJson<LeadBoardData>(webRequest.downloadHandler.text);
                }
                catch (Exception e)
                {
                    Debug.LogError("Couldn't parse JSON correctly : " + e.Message);
                    data = null;
                }
            }
        }
        callback?.Invoke(data);
        yield return null;
    }

    IEnumerator PostLeaderBoardData(LeadBoardData data)
    {
        string json = JsonUtility.ToJson(data);
        using (UnityWebRequest webRequest = UnityWebRequest.Post(BaseUrl + GetAllEndPoint, json))
        {
            yield return webRequest.SendWebRequest();
        }
    }

    IEnumerator GetRequestAndInstantiateIntoCanvasCorout()
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(BaseUrl + GetAllEndPoint))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.ConnectionError)
            {
                Debug.Log("Error: " + webRequest.error);
            }
            else
            {
                GetLeadBoardResult leadBoardData = JsonUtility.FromJson<GetLeadBoardResult>(webRequest.downloadHandler.text);
                int i = 1;
                leadBoardData.lead_boards.OrderByDescending(x => x.score);
                foreach (LeadBoardData lb in leadBoardData.lead_boards)
                {
                    var Go = GameManager.Instantiate(Line);
                    Go.transform.SetParent(LeaderBoardContent.transform, false);
                    var CurrentLine = Go.GetComponent<LeaderBoardUIElement>();

                    CurrentLine.Position.SetText(i.ToString());
                    CurrentLine.Name.SetText(lb.name);
                    var ts = TimeSpan.FromSeconds(lb.score);
                    var formatTime= string.Format("{0:D2}:{1:D2}:{2:D2}",
                            ts.Hours,
                            ts.Minutes,
                            ts.Seconds);
                    CurrentLine.Score.SetText(formatTime);
                    i++;
                }
            }
        }
    }
}
