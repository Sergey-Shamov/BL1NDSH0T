using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCManager : MonoBehaviour
{
    #region Data Types
    public struct LandNPCMovementOrder
    {
        public LandNPCMovementOrder(Vector3 waypointLoc, int waypointNum, EnumRouteFollowerDirections moveDirection)
        {
            waypoint = waypointLoc;
            waypointId = waypointNum;
            direction = moveDirection;
        }

        public Vector3 waypoint;
        public int waypointId;
        public EnumRouteFollowerDirections direction;
    }

    public struct DroneNpcMovementOrder
    {
        public DroneNpcMovementOrder(EnumMoveAroundDirections dir, float sp, float len)
        {
            direction = dir;
            speed = sp;
            moveLength = len;
        }

        public EnumMoveAroundDirections direction;
        public float speed;
        public float moveLength;
    }

    public enum EnumRouteFollowerDirections
    {
        Forward, 
        Backward
    }

    public enum EnumMoveAroundDirections
    {
        Clockwise,
        CounterClockwise
    }
    #endregion Data Types

    #region Public Properties

    public Transform player;
    [Header("Land NPC Settings")]
    public GameObject landNpcPrefab;
    [Tooltip("Collection of paths for land NPCs. Paths cannot change during game.")]
    public GameObject[] landNpcRoutes;
    public int landDamage = 20;
    public int landReward = 40;
    public float landSpeed = 5;

    [Header("Drone NPC Settings")]
    public GameObject droneNpcPrefab;
    public int droneDamage = 20;
    public int droneReward = 100;
    public float droneMinSpeed = .5f;
    public float droneMaxSpeed = 1;
    public float droneMinAltitude = 1;
    public float droneMaxAltitude = 6;
    public float droneMinDistance = 8;
    public float droneMaxDistance = 14;

    #endregion Public Properties

    private Vector3[][] m_routesWaypoints;
    private int m_routesCount;
    private bool m_isGameOver;
    private bool m_isActive;

    void Start ()
    {
        if ((null != landNpcRoutes) && (null != landNpcPrefab))
        {
            // TODO: add path validation
            m_routesCount = landNpcRoutes.Length;
            m_routesWaypoints = new Vector3[m_routesCount][];
            for (int i = 0; i < m_routesCount; ++i)
            {
                Transform route = landNpcRoutes[i].transform;
                int waypointCount = route.childCount;
                m_routesWaypoints[i] = new Vector3[waypointCount];
                for (int j = 0; j < waypointCount; ++j)
                {
                    m_routesWaypoints[i][j] = route.GetChild(j).position;
                }
            }

            m_isActive = true;
        }
	}
	
	void Update ()
    {
	}

    public LandNPCMovementOrder GetLandNpcNextWaypoint(int pathId, int currWaypoint, EnumRouteFollowerDirections direction)
    {
        if (!m_isActive)
            return new LandNPCMovementOrder();

        Vector3 waypoint;
        switch (direction)
        {
            case EnumRouteFollowerDirections.Forward:
                currWaypoint++;
                if (currWaypoint >= m_routesWaypoints[pathId].Length)
                    currWaypoint = 0;
                break;
            case EnumRouteFollowerDirections.Backward:
                currWaypoint--;
                if (currWaypoint < 0)
                    currWaypoint = m_routesWaypoints[pathId].Length - 1;
                break;
        }
        waypoint = m_routesWaypoints[pathId][currWaypoint];
        return new LandNPCMovementOrder(waypoint, currWaypoint, direction);
    }

    public void SpawnLandNpcAtRandom()
    {
        // 1. Select a route
        int routeId = Random.Range(0, m_routesCount - 1);
        // 2. Select a spawn waypoint and direction
        int spawnWaypointId = Random.Range(0, m_routesWaypoints[routeId].Length - 1);
        Vector3 spawnWaypoint = m_routesWaypoints[routeId][spawnWaypointId];
        EnumRouteFollowerDirections direction = Random.value > .5 ? EnumRouteFollowerDirections.Forward : EnumRouteFollowerDirections.Backward;
        // 3. Spawn NPC
        LandEnemyBehavior npcBehavior = Instantiate(landNpcPrefab, spawnWaypoint, Quaternion.identity).GetComponent<LandEnemyBehavior>();
        // 4. Assign route parameters
        npcBehavior.SetInitialRouteValues(routeId, spawnWaypointId, spawnWaypoint, direction);
        npcBehavior.damage = landDamage;
        npcBehavior.rewardPoints = landReward;
        npcBehavior.moveSpeed = landSpeed;
        npcBehavior.player = player;
        npcBehavior.Initialize();
        
    }

    public DroneNpcMovementOrder GetDroneNpcNextMove()
    {
        return new DroneNpcMovementOrder(Random.value > .5 ? EnumMoveAroundDirections.Clockwise : EnumMoveAroundDirections.CounterClockwise,
            Random.Range(6f, 12),
            Random.Range(.1f, 4));
    }

    public void SpawnDroneNpcAtRandom()
    {
        float x, z;
        do
        {
            x = Random.value;
            z = Random.value;
        } while ((x == 0)&&(z == 0));
        Vector3 spawnVector = new Vector3(x, 0, z).normalized * Random.Range(droneMinDistance, droneMaxDistance);
        spawnVector.y = Random.Range(droneMinAltitude, droneMaxAltitude);

        DroneEnemyBehavior npc = Instantiate(droneNpcPrefab, player.position + spawnVector, Quaternion.identity).GetComponent<DroneEnemyBehavior>();
        npc.player = player;
        npc.damage = droneDamage;
        npc.rewardPoints = droneReward;
    }

    #region Npc destruction logic
    // For now we don't use object pools,
    // so to destroy all remaining enemies at the end of the game,
    // we have enemy behaviors checking for end game flag.

    public void DestroyAllNpc()
    {
        m_isGameOver = true;
    }

    public void NewGame()
    {
        m_isGameOver = false;
    }

    public bool IsGameOver()
    {
        return m_isGameOver;
    }
    #endregion Npc destruction logic
}
