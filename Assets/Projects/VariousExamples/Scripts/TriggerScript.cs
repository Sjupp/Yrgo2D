using UnityEngine;

public class TriggerScript : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log("Player entered the room");
        }
        else
        {
            Debug.Log("Somebody entered the room");
        }
    }

    private void OnTriggerStay(Collider other)
    {
        other.transform.position += transform.position;
    }
}
