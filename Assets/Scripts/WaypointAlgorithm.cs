using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class WaypointAlgorithm : MonoBehaviour
{
    public string agentClass;
    public bool choseRandomWaypoints = false;
    [Range(0, 15)] public float mainStayTimeInSeconds = 10f;
    public float agentSpeed = 3.5f;
    [HideInInspector] public bool isWaypointAlgorithmStarted = false;

    [HideInInspector] public NavMeshAgent navMeshAgent;
    Transform[] destinations;

    private int previousRandomNumber; // to avoid overwrite the same waypoint while choosing random


    private void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
    }

    void Start()
    {

    }

    private Transform[] FindClassWaypoints(string gameObjectToFindClassesIn) // will return an empty destinations if it cant find the agentClass gameObject
    {
        GameObject waypoints = GameObject.Find(gameObjectToFindClassesIn);
        foreach (Transform aiClass in waypoints.transform)
        {
            if (aiClass.gameObject.name == agentClass || aiClass.gameObject.name.Replace(" ", string.Empty) == agentClass || aiClass.gameObject.name == agentClass.Replace(" ", string.Empty))
            {
                destinations = aiClass.GetComponentsInChildren<Transform>();
            }
        }
        if (destinations == null)
        { 
            Debug.LogWarning(agentClass + " could not be found, will return empty destinations"); 
        }

        return destinations;
    }

    public IEnumerator InitWaypointAlgorithm()
    {
        FindClassWaypoints("Waypoints");
        int waypointIndex = 1;
        if (destinations != null)
        {
            while (true) // todo, will put a certain bool after
            {
                if (!isWaypointAlgorithmStarted) 
                {
                    /* do nothing */
                    
                    yield return new WaitForSeconds(1f);
                }
                else
                {
                    //waypoint algorithm
                    navMeshAgent.speed = agentSpeed;
                    Debug.Log("(Waypoint algorithm enumerator) agent speed is set to: " + navMeshAgent.speed);
                    if (!choseRandomWaypoints)
                    {
                        navMeshAgent.SetDestination(destinations[waypointIndex].position);
                        float totalUpdateTime = CalculateTotalUpdateTime(waypointIndex);
                        int destinationsLength = destinations.Length - 1;
                        waypointIndex++;
                        if (waypointIndex > destinationsLength) { waypointIndex = 1; }
                        yield return new WaitForSeconds(totalUpdateTime);
                    }
                    else if (choseRandomWaypoints)
                    {
                        waypointIndex = GenerateRandomWaypointIndex();
                        navMeshAgent.SetDestination(destinations[waypointIndex].position);
                        float totalUpdateTime = CalculateTotalUpdateTime(waypointIndex);
                        
                        yield return new WaitForSeconds(totalUpdateTime);
                    }
                }
            }
        }
    }
    
    private int GenerateRandomWaypointIndex()
    {
        int waypointIndex;
        int randomNumber = Mathf.RoundToInt(UnityEngine.Random.Range(1, destinations.Length));
        bool avoidOverwrite = true;
        while (avoidOverwrite)
        {
            if (randomNumber == previousRandomNumber)
            {
                randomNumber = Mathf.RoundToInt(UnityEngine.Random.Range(1, destinations.Length));
            }
            else if (randomNumber != previousRandomNumber)
            {
                avoidOverwrite = false;
            }
        }
        previousRandomNumber = randomNumber;
        waypointIndex = randomNumber;
        return waypointIndex;
    }

    private float CalculateTotalUpdateTime(int waypointIndex)
    {
        float distanceBetweenAgentAndDestination = Vector3.Distance(transform.position, destinations[waypointIndex].position);
        float arriveTime = distanceBetweenAgentAndDestination / navMeshAgent.speed;
        float currentWaypointStayTime = destinations[waypointIndex].GetComponent<Waypoint>().stayTimeForThisWaypoint;
        float totalUpdateTime = arriveTime + mainStayTimeInSeconds + currentWaypointStayTime;
        return totalUpdateTime;
    }
}
