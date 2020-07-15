using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class EnemyAI : MonoBehaviour
{
    NavMeshAgent navMeshAgent;
    WaypointAlgorithm waypointAlgorithm;
    ChaseAlgorithm chaseAlgorithm;
    EnemyStates enemyStates;
    PlayerStates playerStates;

    public LayerMask obstacleMask;

    public float awareness = 0;
    [Range(0, 5)] public float enemyChaseSpeed;

    private bool eyeChecking = false;

    private void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        waypointAlgorithm = GetComponent<WaypointAlgorithm>();
        chaseAlgorithm = GetComponent<ChaseAlgorithm>();
        enemyStates = GetComponent<EnemyStates>();
        playerStates = FindObjectOfType<PlayerStates>();
        StartWaypointAlgorithm();
        StartCoroutine(UpdateAlgorithms(0.2f));
    }

    private void Update() // update the awareness here, update the search algorithms in updateDelay
    {
        if (awareness < 5 || awareness > 40)
        {
            eyeChecking = false;
            navMeshAgent.isStopped = false;
        }
        // if awareness below 5 and waypoint algorithm did not started start the algorithm and not overload the algorithm
        if (awareness < 5 && !waypointAlgorithm.isWaypointAlgorithmStarted)
        {
            waypointAlgorithm.isWaypointAlgorithmStarted = true;
        }
        // even if the awareness above the 20 and below 40, once we enter the if statement the eyeChecking will not be false until awareness is below 5
        if (awareness > 20 && awareness < 40 || eyeChecking)
        {
            LookAtPlayer(playerStates.transform.position);
            navMeshAgent.isStopped = true;
            eyeChecking = true;
        }
    }

    public void TriggerAwareness(float triggerValue)
    {
        awareness += triggerValue;
        awareness = Mathf.Clamp(awareness, 0, 95);
        GameObject.Find("Text").GetComponent<Text>().text = "Enemy Awareness: " +  awareness.ToString();
    }

    public float ControlAwareness(float viewRadiusMultiplier)
    {
        switch (playerStates.GetCurrentState)
        {
            case PlayerStates.PlayerState.Stand:
                TriggerAwareness(4f * viewRadiusMultiplier);
                Debug.Log("Standing, Triggering awareness with: " + 2f);
                Debug.Log("decrease value is multiplied with: " + viewRadiusMultiplier);
                
                break;
            case PlayerStates.PlayerState.Crouch:
                TriggerAwareness(2f * viewRadiusMultiplier);
                Debug.Log("Crouching, Triggering awareness with: " + 0f);
                break;
            case PlayerStates.PlayerState.StandAndFlashlight:
                TriggerAwareness(3.5f * viewRadiusMultiplier);
                Debug.Log("Standing and Flashlight, Triggering awareness with: " + 2f);
                break;
            case PlayerStates.PlayerState.StandAndGun:
                TriggerAwareness(4f * viewRadiusMultiplier);
                Debug.Log("Standing and Gun, Triggering awareness with: " + 5f);
                break;
            case PlayerStates.PlayerState.CrouchAndFlashLight:
                TriggerAwareness(2.5f * viewRadiusMultiplier);
                Debug.Log("Crouching and Flashlight, Triggering awareness with: " + 1f);
                break;
            case PlayerStates.PlayerState.CrouchAndGun:
                TriggerAwareness(3f * viewRadiusMultiplier);
                Debug.Log("Crouching and Gun, Triggering awareness with: " + 2f);
                break;
        }
        return awareness;
    }

    public IEnumerator UpdateAlgorithms(float updateTime)
    {
        while (true)
        {
            Debug.DrawLine(transform.position, playerStates.transform.position, Color.red);
            float distToTarget = Vector3.Distance(transform.position, playerStates.transform.position);
            TriggerAwareness(-1f);
            if (awareness < 20 && !waypointAlgorithm.isWaypointAlgorithmStarted)
            {
                StartWaypointAlgorithm();
            }
            else if (awareness > 20 && awareness < 40)
            {
                // I dont know just leaving the if statement
            }
            else if (awareness > 40 && awareness < 60 && !chaseAlgorithm.isChaseAlgorithmStarted && !Physics.Raycast(transform.position, playerStates.transform.position, distToTarget, obstacleMask))
            {
                ProcessChaseAlgorithm();
            }
            else if (awareness > 80)
            {
                Debug.Log("You got detected, Game is Over");
            }
            yield return new WaitForSeconds(updateTime);
        }
    }

    private void LookAtPlayer(Vector3 targetToLook)
    {
        Vector3 rotationCoordinates = (targetToLook - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(rotationCoordinates, Vector3.up);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 1.8f);
    }

    public void StartWaypointAlgorithm()
    {
        navMeshAgent.isStopped = false;
        chaseAlgorithm.isChaseAlgorithmStarted = false;
        waypointAlgorithm.isWaypointAlgorithmStarted = true; // we have to set the boolen to true first and then start the coroutine
        StartCoroutine(waypointAlgorithm.InitWaypointAlgorithm()); // start the waypoint algorithm
        enemyStates.CurrentState = EnemyStates.EnemyState.WalkToWaypoint;
    }

    public void ProcessChaseAlgorithm()
    {
        navMeshAgent.isStopped = false;
        waypointAlgorithm.isWaypointAlgorithmStarted = false;
        chaseAlgorithm.isChaseAlgorithmStarted = true; // we have to set the boolen to true first and then start the coroutine
        StartCoroutine(chaseAlgorithm.ChaseTarget()); // start the chase algorithm
        navMeshAgent.speed = 1.8f;
        enemyStates.CurrentState = EnemyStates.EnemyState.Suspicious;
    }
}
