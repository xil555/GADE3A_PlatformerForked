using UnityEngine;

public class ParentPlayerMovement : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        collision.transform.SetParent(transform);
    }

   void OnCollisionExit(Collision collision)
    {
        collision.transform.SetParent(null);
    }
}
