using System;
using System.Linq;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{

    public GameObject TargetPlayer;
    public int ViewDistance;
    public float NewLocationDistance;
    public LayerMask VisbleCheckMask;

    public Transform[] WanderLocations;

    NavMeshAgent agent;

    Vector3 TargetLocation;
    int priority = 0;

    /*priority is measured by how import the curent locaton is
    0 is nothing
    1 is wander
    between 1-5 is noise
    5 is chasing the player
    */


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    void FixedUpdate()
    {

        //Checks if it can see the player
        Vector3 difVector = TargetPlayer.transform.position - transform.position;
        if (Physics.Raycast(transform.position, difVector, out RaycastHit hit, ViewDistance, VisbleCheckMask))
        {
            if (hit.collider.gameObject.CompareTag("Player"))
            {
                SetTarget(hit.transform.position);
                priority = 5;
            }
            else if (priority == 5)
            {
                priority = 1;
            }
        }


        //Checks if goal has been reached
        if (agent.remainingDistance <= NewLocationDistance)
        {
            priority = 0;
        }

        if (priority == 0)
        {
            SetRandomWander();
        }

        agent.SetDestination(TargetLocation);


        //Debug
        Debug.DrawRay(transform.position, (difVector.normalized) * ViewDistance, Color.red);
        Debug.DrawLine(new Vector3(TargetLocation.x, TargetLocation.y - 10, TargetLocation.z), new Vector3(TargetLocation.x, TargetLocation.y + 10, TargetLocation.z), Color.blue);
        
    }

    void SetRandomWander()
    {
        if (WanderLocations.Length <= 0)
        {
            Debug.LogError("Wander locations missing");
            return;
        }
        int indexlocation = UnityEngine.Random.Range(0, WanderLocations.Length);
        SetTarget(WanderLocations[indexlocation].position);
        priority = 1;
    }

    void SetTarget(Vector3 TargetLocation) => agent.SetDestination(TargetLocation);

    public void SendSound(Vector3 postion, int priority)
    {
        SetTarget(postion);
        this.priority = priority;
    }
}
