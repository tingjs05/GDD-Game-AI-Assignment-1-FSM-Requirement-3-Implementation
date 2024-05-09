using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushableObject : MonoBehaviour
{
    [SerializeField] float dropAngle = 90f;
    [SerializeField] float dropSpeed = 1f;
    [SerializeField] PushableObjectHitbox hitbox;
    private bool hasBeenDropped = false;

    // Start is called before the first frame update
    void Start()
    {
        hasBeenDropped = false;
    }

    public void DropObject()
    {
        if (hasBeenDropped) return;
        // set can hit boolean
        hasBeenDropped = true;
        if (hitbox != null) hitbox.canHit = hasBeenDropped;
        // start coroutine to slowly drop pillar
        StartCoroutine(Drop());
    }

    IEnumerator Drop()
    {
        // slowly interpolate between current rotation and target angle
        while (transform.rotation.x < dropAngle)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, 
                Quaternion.Euler(dropAngle, transform.rotation.y, transform.rotation.z), 
                dropSpeed * Time.deltaTime);
            yield return null;
        }
        // destroy game object when sucessfully dropped
        Destroy(gameObject);
    }
}
