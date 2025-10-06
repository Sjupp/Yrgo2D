using UnityEngine;

public class ObjectAManager : MonoBehaviour
{
    public static ObjectAManager Instance = null;

    private int _score = 0;

    public GameObject portalRefThingy = null;

    public void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddScore(int scoreToAdd)
    {
        _score += scoreToAdd;
    }

    public void InitLevel(LevelData levelData)
    {
        portalRefThingy = levelData.portal;
    }
}