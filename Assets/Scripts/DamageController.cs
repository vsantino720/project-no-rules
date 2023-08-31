using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageController : MonoBehaviour
{
    [SerializeField] private float damage = 10f;

    [SerializeField] private HealthController healthController;

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Collided with player...");

            healthController.TakeDamage(damage);
        }
    }
}
