using UnityEngine;

public class CollideObject : MonoBehaviour
{
    public bool Priority = true;

    public void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("Collide");
        if (Priority)
        {
            Debug.Log("Collided with " + collision.gameObject);
            if (collision.gameObject.TryGetComponent(out CollideObject comp))
            {
                Debug.Log("Disabled other");
                comp.Priority = false;
                Destroy(collision.gameObject);
            }

            return;
            var rand = Random.Range(0, 3);
            switch (rand)
            {
                case 0:
                    // nothing
                    break;
                case 1:
                    Destroy(collision.gameObject);
                    break;
                case 2:
                    //instantiate
                    break;
            }
        }
    }
}