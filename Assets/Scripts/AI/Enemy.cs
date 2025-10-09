using System;
using System.Linq;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class Enemy : MonoBehaviour
{

    public GameObject TargetPlayer;
    public float Speed;
    public int ViewDistance;
    public LayerMask VisbleCheckMask;

    public Transform[] WanderLocations;

    Vector3 TargetLocation;
    int priority = 0;
    /*priority is mesured by how import the curent locaton is
    0 is nothing
    1 is wander
    between 1-5 is noise
    5 is chasing the player
    */


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    void FixedUpdate()
    {
        //Checks if it can see the player
        Vector3 difVector = TargetPlayer.transform.position - transform.position;
        if (Physics.Raycast(transform.position, difVector, out RaycastHit hit, ViewDistance, VisbleCheckMask))
        {
            if (hit.collider.gameObject.CompareTag("Player"))
            {
                print("I see you");
                TargetLocation = hit.transform.position;
                priority = 5;
            }
            else if (priority == 5)
            {
                priority = 1;
            }
        }

        if (priority == 0)
        {
            SetRandomWander();
        }




        //Debug
        Debug.DrawRay(transform.position, (difVector.normalized) * ViewDistance, Color.red);
        Debug.DrawLine(new Vector3(TargetLocation.x, TargetLocation.y - 10, TargetLocation.z), new Vector3(TargetLocation.x, TargetLocation.y + 10, TargetLocation.z), Color.blue);
    }

    void SetRandomWander()
    {
        int indexlocation = UnityEngine.Random.Range(0, WanderLocations.Count());
        TargetLocation = WanderLocations[indexlocation].position;
        priority = 1;
    }
}
