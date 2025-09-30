using UnityEngine;

public class LevelHandler : MonoBehaviour
{
    [SerializeField]
    private LevelData _levelData = null;

    public void Start()
    {
        ObjectAManager.Instance.InitLevel(_levelData);
    }
}

[System.Serializable]
public class LevelData
{
    public GameObject portal = null;
}
