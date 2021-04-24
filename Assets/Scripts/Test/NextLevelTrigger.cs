using UnityEngine;

public class NextLevelTrigger : MonoBehaviour
{
    bool _entered = false;

    void OnTriggerEnter(Collider other)
    {
        if (!_entered && other.tag == "Player")
        {
            _entered = true;
            MoveToNextLevel();
        }
    }

    void MoveToNextLevel()
    {
        GameManager.singleton.ResourcesLoaderManager.LevelLoader.MoveToNextLevel();
    }
}