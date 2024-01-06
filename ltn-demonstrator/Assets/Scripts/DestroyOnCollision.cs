using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyOnCollision : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Barricade"))
        {
            Destroy(collision.gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Destroy ITEM on collision with BIN
        if (other.CompareTag("Bin"))
        {
            Destroy(other.gameObject);
        }
    }
}
