using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Redirects damage call from INpcBehavior to parent script.
/// </summary>
public class DroneHPBehavior : MonoBehaviour, INpcBehavior
{
    public DroneEnemyBehavior parentScript;

    public void DealDamage(int amount)
    {
        parentScript.DealDamage(amount);
    }
}
