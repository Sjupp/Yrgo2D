using UnityEngine;

public class ObjectBUnit : MonoBehaviour
{

    public void Die()
    {
        ObjectAManager.Instance.AddScore(5);
    }
}
