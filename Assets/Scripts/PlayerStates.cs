using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStates : MonoBehaviour
{
    public enum PlayerState
    {
        Stand,
        Crouch,
        StandAndFlashlight,
        StandAndGun,
        CrouchAndFlashLight,
        CrouchAndGun
    }

    [SerializeField] private PlayerState state = PlayerState.Stand;

    public PlayerState GetCurrentState { get { return state; } set { state = value; } }
}
