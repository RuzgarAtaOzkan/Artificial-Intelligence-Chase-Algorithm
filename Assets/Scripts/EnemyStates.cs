using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStates : MonoBehaviour
{
    public enum EnemyState
    {
        WalkToWaypoint,
        Suspicious,
        Alarm,
        Engage
    }

    private EnemyState state = EnemyState.WalkToWaypoint;

    public EnemyState CurrentState { get { return state; } set { state = value; } }
}
