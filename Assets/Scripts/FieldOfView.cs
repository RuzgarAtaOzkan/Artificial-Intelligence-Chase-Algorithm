using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldOfView : MonoBehaviour
{
    EnemyAI enemyAI;

    public float viewRadius;
    [Range(0, 360)]
    public float viewAngle;

    public LayerMask targetMask;
    public LayerMask obstacleMask;

    [HideInInspector]
    public List<Transform> visibleTargets = new List<Transform>();

    private float viewRadiusMultiplier = 1;

    private void Start()
    {
        enemyAI = GetComponent<EnemyAI>();
        StartCoroutine("FindTargetsWithDelay", .2f);
    }

    IEnumerator FindTargetsWithDelay(float updateTime)
    {
        while (true)
        {
            FindVisibleTargets();
            GameObject[] targets = GameObject.FindGameObjectsWithTag("Player"); // warning, there has to be a player with Player tag
            foreach (GameObject target in targets)
            {
                // check if player one of the 3 danger layer in view Radius, divide the viewRadius into 3, multiple the value that we decrease in control awareness
                if (Vector3.Distance(target.transform.position, transform.position) <= viewRadius)
                {
                    float distanceToPlayer = Vector3.Distance(target.transform.position, transform.position);
                    float threePartOfDistance = (viewRadius / 3);
                    Debug.Log(threePartOfDistance);
                    if (distanceToPlayer < viewRadius && distanceToPlayer > threePartOfDistance * 2)
                    {
                        viewRadiusMultiplier = 1;
                    }
                    else if (distanceToPlayer < (threePartOfDistance * 2) && distanceToPlayer > threePartOfDistance)
                    {
                        viewRadiusMultiplier = 1.2f;
                    }
                    else if (distanceToPlayer < threePartOfDistance)
                    {
                        viewRadiusMultiplier = 1.4f;
                    }
                }
            }
            yield return new WaitForSeconds(updateTime);
        }
    }

    void FindVisibleTargets()
    {
        visibleTargets.Clear();
        Collider[] targetsInViewRadius = Physics.OverlapSphere(transform.position, viewRadius, targetMask);
        for (int i = 0; i < targetsInViewRadius.Length; i++)
        {
            Transform target = targetsInViewRadius[i].transform;
            Vector3 dirToTarget = (target.position - transform.position).normalized;
            if (Vector3.Angle(transform.forward, dirToTarget) < viewAngle / 2)
            {
                float distToTarget = Vector3.Distance(transform.position, target.position);

                if (!Physics.Raycast(transform.position, dirToTarget, distToTarget, obstacleMask))
                {
                    visibleTargets.Add(target);
                    enemyAI.ControlAwareness(viewRadiusMultiplier);
                }
            }
        }
    }

    public Vector3 DirFromAngle(float angleInDegrees, bool isAngleGlobal)
    {
        if (!isAngleGlobal)
        {
            angleInDegrees += transform.eulerAngles.y;
        }
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }
}
