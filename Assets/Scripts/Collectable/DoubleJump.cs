using System.Collections;
using UnityEngine;

public class DoubleJump : MonoBehaviour
{
    public float jumpForce = 15f;

    private Collider col;
    private MeshRenderer mesh;

    private void Awake()
    {
        col = GetComponent<Collider>();
        mesh = GetComponent<MeshRenderer>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Rigidbody rb = other.GetComponent<Rigidbody>();

            if (rb != null)
            {
                rb.linearVelocity = new Vector3(rb.linearVelocity.x, jumpForce, rb.linearVelocity.z);
            }

            StartCoroutine(RespawnRoutine());
        }
    }

    private IEnumerator RespawnRoutine()
    {
     
        col.enabled = false;
        if (mesh != null) mesh.enabled = false;

        yield return new WaitForSeconds(5f);

        
        col.enabled = true;
        if (mesh != null) mesh.enabled = true;
    }
}
