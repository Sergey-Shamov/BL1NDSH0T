using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectDestroyerBehavior : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        Destroy(collision.collider.gameObject);
    }
}
