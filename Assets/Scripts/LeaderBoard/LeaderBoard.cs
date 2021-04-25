using System.Collections;
using System.Collections.Generic;
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

public class LeaderBoard : MonoBehaviour
{

    private GameObject Line;
    private GameObject Table;

    private Coroutine AllLeadBoard = null;
    private string BaseUrl = "http://51.91.99.249";


    // Start is called before the first frame update
    void Start()
    {
        
        Line = GameManager.singleton.ResourcesLoaderManager.LeaderBoardLine;
        Table = GameManager.singleton.ResourcesLoaderManager.LeaderBoardTable;
        Table = Instantiate(Table);
        Table.transform.SetParent(this.transform);

    }

    // Update is called once per frame
    void Update()
    {
        if (this.enabled && AllLeadBoard == null)
        {
            AllLeadBoard = StartCoroutine(GetRequest(BaseUrl + "/leader_board/"));
        }

    }

    IEnumerator GetRequest(string uri)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
        {
            // On envoie la requête et on attend la réponse
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.ConnectionError)
            {
                Debug.Log("Error: " + webRequest.error);
            }
            else
            {
                GetLeadBoardResult leadBoardData = JsonUtility.FromJson<GetLeadBoardResult>(webRequest.downloadHandler.text);
                int i = 1;
                foreach (LeadBoardData lb in leadBoardData.lead_boards)
                {

                    var Go = Instantiate(Line);
                    Go.transform.SetParent(Table.transform, false);
                    var CurrentLine = Go.GetComponent<Line>();
                    CurrentLine.Place.SetText(i.ToString());
                    CurrentLine.Name.SetText(lb.name);
                    CurrentLine.Score.SetText(lb.score.ToString());

                }

            }
        }
    }
}
