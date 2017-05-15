using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneEnemyBehavior : MonoBehaviour
{
    public Transform player;
    public Transform body;
    public GameObject invisibleProjectilePrefab;
    public GameObject projectilePrefab;
    public float speed = 1;
    public float acceleration = 1;
    public int hitPoints = 150;
    public int damage = 20;
    public int rewardPoints = 100;
    public DroneEnemyBehavior drone;

    private NPCManager.EnumMoveAroundDirections m_moveDirection;
    private Vector3 m_direction = Vector3.zero;                     // Actual speed vector. Length is units/sec.
    private float m_flightLength = 0;

    private bool m_isActive;
    private bool m_dying;

    void Start()
    {
        if ((null != player) && (null != body))
        {
            m_isActive = true;
        }
    }
    
    void Update()
    {
        if (GameManager.GetInstance().GetNpcManager().IsGameOver())
            Destroy(gameObject);
        else
        {
            if (m_isActive)
            {
                if (m_dying)
                {
                    // TODO: replace. A temporary solution for drone death. Collider not detecting collisions.
                    if (transform.position.y < 0)
                        Destroy(gameObject);

                    m_direction.y -= Time.deltaTime * 7;
                    
                    if (UnityEngine.Random.Range(0, 10) == 0)
                    {
                        GameObject projectile = Instantiate(invisibleProjectilePrefab, transform.position + Vector3.down, transform.rotation);
                        projectile.GetComponent<Rigidbody>().velocity = Vector3.down * 5;
                    }
                }
                else
                {
                    // 1. Check flight order
                    if (m_flightLength > 0)
                        m_flightLength -= Time.deltaTime;
                    else
                    {
                        // get new flight orders
                        var order = GameManager.GetInstance().GetNpcManager().GetDroneNpcNextMove();
                        m_flightLength = order.moveLength;
                        speed = order.speed;
                        m_moveDirection = order.direction;
                    }

                    // 2. Calculate direction
                    Vector3 newDirection;
                    Vector3 playerDirection = player.position - transform.position;
                    int directionFactor = (m_moveDirection == NPCManager.EnumMoveAroundDirections.Clockwise) ? 1 : -1;
                    newDirection = new Vector3(-directionFactor * playerDirection.z, 0, directionFactor * playerDirection.x);   // direction is a vector perpendicular to direction to the player
                    newDirection = newDirection.normalized * speed;                                                             // desired direction vector
                    Vector3 directionChangeVector = (newDirection - m_direction);                                               // to make drone seem inertial, calculate velocity difference vector
                    float changeValue = directionChangeVector.sqrMagnitude;
                    float maxChangeValue = acceleration * Time.deltaTime;
                    if ((maxChangeValue * maxChangeValue) < changeValue)
                        directionChangeVector = directionChangeVector.normalized * maxChangeValue;                              // direction change is limited to simulate inertia

                    m_direction += directionChangeVector;

                    // 3. Calculate drone orientation and roll
                    transform.LookAt(player);
                    // Roll is greater the greater the speed is. However, roll is limited to 20 degree.
                    // Sometimes new order gives speed less than before, in such case roll remains 20.
                    float angle = (20 * MyGetPortion(m_direction.sqrMagnitude, speed * speed) * (Vector3.Cross(m_direction, transform.forward).y > 0 ? 1 : -1));
                    transform.Rotate(Vector3.forward, angle);

                    // 4. Attack
                    if (UnityEngine.Random.Range(0, 1000) == 0)  //TODO: different attack randomization
                    {
                        GameObject projectile = Instantiate(projectilePrefab, transform.position + transform.forward * 2, body.rotation);
                        projectile.GetComponent<Rigidbody>().velocity = body.forward * 40;
                        projectile.GetComponent<LandNpcProjectileBehavior>().damage = damage;

                    }
                }

                transform.position += m_direction * Time.deltaTime;
            }
        }
    }

    public void DealDamage(int amount)
    {
        if (!m_dying)
        {
            hitPoints -= amount;
            if (hitPoints < 0)
            {
                m_dying = true;
                GameManager.GetInstance().AddPoints(rewardPoints);
            }
        }
    }

    /// <summary>
    /// Returns portion of part in whole. If part is greater then whole, returns 1.
    /// Portion is absolute.
    /// </summary>
    /// <param name="part">Value to measure.</param>
    /// <param name="whole">Reference value.</param>
    /// <returns>Returns portion of part in whole.</returns>
    private float MyGetPortion(float part, float whole)
    {
        float portion = Math.Abs(part / whole);
        return portion > 1 ? 1 : portion;
    }
}
