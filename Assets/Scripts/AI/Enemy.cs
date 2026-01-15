using System;
using System.Collections;
using System.Linq;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class Enemy : MonoBehaviour
{

    [Header("Vision")]
    [SerializeField] private GameObject TargetPlayer;
    [SerializeField] private int ViewDistance;
    [SerializeField] private LayerMask VisbleCheckMask;

    [Header("Path Finding")]
    [SerializeField] private float NewLocationDistance;
    [SerializeField] private Transform[] WanderLocations;
    [Header("Extra")]
    [SerializeField] private GameObject DisplayPlane;
    [SerializeField] private UnityEngine.Object DefaultScene;

    [Header("Chase mode")]
    [SerializeField] private float DefaultSpeed;
    [SerializeField] private float MaxSpeed;
    [SerializeField] private float IntimidateTime;
    [Header("Animation")]
    [SerializeField] private Animator animator;

    bool ChaseMode;
    NavMeshAgent agent;
    int priority = 0;
    Quaternion planeDefaultRot;




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
        planeDefaultRot = DisplayPlane.transform.rotation;
        agent.speed = DefaultSpeed;
    }

    void Update()
    {
        Vector3 lookdir = (TargetPlayer.transform.position - transform.position).normalized;
        Vector3 lookangles = Quaternion.LookRotation(lookdir).eulerAngles;
        DisplayPlane.transform.rotation = Quaternion.Euler(90, lookangles.y + 90, 90);

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
                if (!ChaseMode)
                {
                    StartCoroutine(WaitAndChase());
                    ChaseMode = true;
                }

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
            if (ChaseMode)
            {
                agent.speed = DefaultSpeed;
                ChaseMode = false;
                animator.speed = 1;
            }

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
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            print("Loading new scene");
            SceneManager.LoadScene(DefaultScene.name);
        }
    }

    IEnumerator WaitAndChase()
    {
        animator.SetBool("Waiting", true);
        agent.speed = 0;
        yield return new WaitForSeconds(IntimidateTime);
        agent.speed = MaxSpeed;
        animator.SetBool("Waiting", false);
        animator.speed = MaxSpeed / DefaultSpeed;
    }
}
