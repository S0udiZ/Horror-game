using System;
using System.Linq;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{

    [Header("Vision")]
    [SerializeField] private GameObject TargetPlayer;
    [SerializeField] private int ViewDistance;
    [SerializeField] private LayerMask VisbleCheckMask;

    [Header("Path Finding")]
    [SerializeField] private float NewLocationDistance;
    [SerializeField] private Transform[] WanderLocations;

    NavMeshAgent agent;
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
                agent.SetDestination(hit.transform.position);
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



        //Debug
        Debug.DrawRay(transform.position, (difVector.normalized) * ViewDistance, Color.red);
        Debug.DrawLine(new Vector3(agent.destination.x, agent.destination.y - 10, agent.destination.z), new Vector3(agent.destination.x, agent.destination.y + 10, agent.destination.z), Color.blue);
        
    }

    void SetRandomWander()
    {
        if (WanderLocations.Length <= 0)
        {
            Debug.LogError("Wander locations missing");
            return;
        }
        int indexlocation = UnityEngine.Random.Range(0, WanderLocations.Length);
        agent.SetDestination(WanderLocations[indexlocation].position);
        priority = 1;
    }

    public void SendSound(Vector3 postion, int priority)
    {
        if (priority > this.priority)
        {
           agent.SetDestination(postion);
            this.priority = priority; 
        }
    }
}
