using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class FollowerBehavior : MonoBehaviour {
    public Transform targetObject;

    private NavMeshAgent m_navAgent;
	// Use this for initialization
	void Start ()
    {
        m_navAgent = GetComponent<NavMeshAgent>();
	}
	
	// Update is called once per frame
	void Update ()
    {
        m_navAgent.SetDestination(targetObject.position);
	}
}
