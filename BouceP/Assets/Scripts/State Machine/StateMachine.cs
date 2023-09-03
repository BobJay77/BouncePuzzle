using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class StateMachine : MonoBehaviour
{
    protected State State;

    public void SetState(State state)
    {
        if (State != null)
        {
            StartCoroutine(State.OnExit());
        }

        State = state;

        Debug.Log("Current State: " + State.ToString());

        StartCoroutine(State.OnEnter());
    }
}
