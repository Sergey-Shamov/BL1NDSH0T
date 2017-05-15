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
    private Vector3 m_direction = Vector3.zero;
    private float m_speed = 0;
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

                    m_direction.y -= .03f;
                    m_speed += .05f;
                    if (UnityEngine.Random.Range(0, 10) == 0)
                    {
                        GameObject projectile = Instantiate(invisibleProjectilePrefab, transform.position + Vector3.down, transform.rotation);
                        projectile.GetComponent<Rigidbody>().velocity = Vector3.down * 5;
                    }
                }
                else
                {
                    if (m_flightLength > 0)
                        m_flightLength -= Time.deltaTime;
                    else
                    {
                        var order = GameManager.GetInstance().GetNpcManager().GetDroneNpcNextMove();
                        m_flightLength = order.moveLength;
                        speed = order.speed;
                        m_moveDirection = order.direction;
                    }

                    int directionMod = m_moveDirection == NPCManager.EnumMoveAroundDirections.Clockwise ? 1 : -1;
                    m_speed = m_speed + acceleration * Time.deltaTime * directionMod;
                    if (m_moveDirection == NPCManager.EnumMoveAroundDirections.Clockwise)
                        m_speed = Math.Min(m_speed, speed);
                    else
                        m_speed = Math.Max(m_speed, -speed);

                    Vector3 playerDirection = player.position - transform.position;
                    m_direction = new Vector3(-playerDirection.z, 0, playerDirection.x).normalized;

                    body.LookAt(player);
                    body.Rotate(Vector3.forward, (20 * (m_speed / speed)));

                    if (UnityEngine.Random.Range(0, 1000) == 0)  //TODO: different attack randomization
                    {
                        GameObject projectile = Instantiate(projectilePrefab, transform.position + body.forward * 2, body.rotation);
                        projectile.GetComponent<Rigidbody>().velocity = body.forward * 40;
                        projectile.GetComponent<LandNpcProjectileBehavior>().damage = damage;

                    }
                }

                transform.position += m_direction * m_speed * Time.deltaTime;
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
}
