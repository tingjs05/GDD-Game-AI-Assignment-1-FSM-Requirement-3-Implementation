using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushableObjectHitbox : MonoBehaviour
{
    [HideInInspector] public bool canHit = false;

    // Start is called before the first frame update
    void Start()
    {
        canHit = false;
    }

    void OnTriggerEnter(Collider other)
    {
        // only check collisions if can hit
        if (!canHit) return;

        // check collisions with player and enemy
        if (other.CompareTag("Player"))
        {
            PlayerController controller = other.GetComponent<PlayerController>();
            if (controller == null) return;
            // stun player if hit
            controller.Stun();
        }
        else if (other.CompareTag("Enemy"))
        {
            Debug.Log("Hit enemy!");
        }
    }
}
