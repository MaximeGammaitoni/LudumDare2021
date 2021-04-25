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
    }
    public void GetRequestAndInstantiateIntoCanvas()
    {
        GameManager.singleton.StartCoroutine(GetRequestAndInstantiateIntoCanvasCorout());
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
