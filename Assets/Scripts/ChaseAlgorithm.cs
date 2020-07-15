using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ChaseAlgorithm : MonoBehaviour
{
    FieldOfView fieldOfView;
    [HideInInspector] public Transform target;
    [HideInInspector] public NavMeshAgent navMeshAgent;
    [HideInInspector] public bool isChaseAlgorithmStarted = false;

    void Start()
    {
        fieldOfView = GetComponent<FieldOfView>();
        target = GameObject.FindWithTag("Player").transform;
        navMeshAgent = GetComponent<NavMeshAgent>();
    }

    public IEnumerator ChaseTarget()
    {
        while (true)
        {
            if (!isChaseAlgorithmStarted) 
            {
                /* do nothing */
                yield return new WaitForSeconds(1f);
            }
            else
            {
                Debug.Log("EnemyAI is suspicious coming to last point it saw the player !!!");
                navMeshAgent.SetDestination(target.position);
                float distanceBetweenAgentAndTarget = Vector3.Distance(transform.position, target.position);
                float arriveTime = distanceBetweenAgentAndTarget / navMeshAgent.speed;
                float searchTime = -1.5f;
                float totalStayTime = arriveTime + searchTime;
                yield return new WaitForSeconds(totalStayTime);
            }
        }
    }
}
