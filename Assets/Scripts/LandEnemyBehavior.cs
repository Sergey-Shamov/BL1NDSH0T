using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface INpcBehavior
{
    void DealDamage(int amount);
}

public class LandEnemyBehavior : MonoBehaviour, INpcBehavior
{
    private enum EnumStates
    {
        Waiting,
        OnRoute,
        PrepareAttack,
        Attacking
    }

    public float rotationSpeed = 90;
    public float moveSpeed = 2;
    public int HP = 100;
    public int damage = 20;
    public int rewardPoints = 40;

    public Transform player;
    public GameObject projectilePrefab;

    private CharacterController m_controller;

    private int m_routeId;                                          // current route id
    private int m_waypointId;                                       // current target waypoint id
    private Vector3 m_waypoint;                                     // target waypoint
    private NPCManager.EnumRouteFollowerDirections m_direction;     // current route movement direction
    private EnumStates m_state = EnumStates.Waiting;
    private bool m_isActive;

	public void Initialize()
    {
        m_controller = GetComponent<CharacterController>();
        if ((null != m_controller) && (null != player))
            m_isActive = true;
	}
	
	
	void Update ()
    {
        if (GameManager.GetInstance().GetNpcManager().IsGameOver())
            Destroy(gameObject);
        else
        {
            if (m_isActive)
            {
                MyProcessState();
            }
        }
	}

    public void SetInitialRouteValues(int routeId, int waypointId, Vector3 waypoint, NPCManager.EnumRouteFollowerDirections direction)
    {
        m_routeId = routeId;
        m_waypointId = waypointId;
        m_waypoint = waypoint;
        m_direction = direction;
    }

    public void DealDamage(int amount)
    {
        if (HP > 0)
        {
            HP -= amount;
            if (HP <= 0)
            {
                Destroy(gameObject);    // TODO: add effects
                GameManager.GetInstance().AddPoints(rewardPoints);
            }
        }
    }

    private void MyProcessState()
    {
        switch(m_state)
        {
            case EnumStates.Waiting:
                MyToOnRoute();
                break;
            case EnumStates.OnRoute:
                MyMove();
                break;
            case EnumStates.PrepareAttack:
                MyPrepareAttack();
                break;
            case EnumStates.Attacking:
                MyAttack();
                break;
        }
    }

    private void MyPrepareAttack()
    {
        // Check if we are looking at player
        Vector3 playerDirection = (player.position - transform.position).normalized;
        playerDirection.y = 0;
        Vector3 forwardDirection = transform.forward;
        forwardDirection.y = 0;
        if (Vector3.Angle(playerDirection, forwardDirection) > 5)
            MyRotateTowards(player.position);
        else
        {
            MyShoot();
            m_state = EnumStates.Attacking;
        }
    }

    private void MyAttack()
    {
        if (true)   //TODO: if shooting is done
            m_state = EnumStates.Waiting;
    }

    private void MyMove()
    {
        // Check if waypoint is reached
        if ((m_waypoint - transform.position).sqrMagnitude < 1)
        {
            // Wait for orders
            m_state = EnumStates.Waiting;
        }
        else
        {
            // Movement
            // Check for obstacles
            if ((m_controller.collisionFlags & CollisionFlags.CollidedSides) == CollisionFlags.CollidedSides)
                transform.Rotate(transform.up, rotationSpeed * Time.deltaTime);  // obstacle, avoid by turning
            else
            {
                // no obstacle, turn towards waypoint
                MyRotateTowards(m_waypoint);
            }

            m_controller.SimpleMove(transform.forward * moveSpeed);
        }

        if (UnityEngine.Random.Range(0, 1200) == 0)  //TODO: different attack randomization
        {
            // Check if we can see player. If so, attack. If not, keep moving.
            RaycastHit hit;
            if (Physics.Raycast(transform.position, player.position - transform.position, out hit))
            {
                if ((null != hit.collider) && (hit.collider.gameObject.tag == "Player"))
                {
                    m_state = EnumStates.PrepareAttack;
                }
            }
        }
    }

    private void MyToOnRoute()
    {
        // Check if waypoint is reached
        if ((m_waypoint - transform.position).sqrMagnitude < 1)
        {
            // Get new waypoint
            var nextWaypoint = GameManager.GetInstance().GetNpcManager().GetLandNpcNextWaypoint(m_routeId, m_waypointId, m_direction);
            m_waypoint = nextWaypoint.waypoint;
            m_waypointId = nextWaypoint.waypointId;
            m_direction = nextWaypoint.direction;
        }

        // Either way continue movement
        m_state = EnumStates.OnRoute;
    }

    private void MyShoot()
    {
        Ray targetRay = new Ray(transform.position, player.position - transform.position);
        GameObject projectile = Instantiate(projectilePrefab, targetRay.GetPoint(2), Quaternion.LookRotation(targetRay.direction));
        projectile.GetComponent<Rigidbody>().velocity = targetRay.direction * 40;
        projectile.GetComponent<LandNpcProjectileBehavior>().damage = damage;
    }

    private void MyRotateTowards(Vector3 target)
    {
        Vector3 direction = target - transform.position;
        direction.y = 0;    // height difference does not matter
        transform.rotation = Quaternion.LookRotation(Vector3.RotateTowards(transform.forward, direction, ((rotationSpeed * 3.14f) / 180) * Time.deltaTime, 1)); //TODO: totally lame
    }
}
