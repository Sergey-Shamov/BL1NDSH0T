using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LandNpcProjectileBehavior : MonoBehaviour
{
    public int damage = 10;

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
            GameManager.GetInstance().AddPoints(-damage);
        Destroy(gameObject);
    }
}
