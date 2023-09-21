using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class StateMachine : MonoBehaviour
{
    protected State State = null;

    public void SetState(State state)
    {
        if (State != null)
        {
            StartCoroutine(State.OnExit());
        }

        State = state;

        if(State.ToString() != "WinLose")
            Debug.Log("Current State: " + State.ToString());

        StartCoroutine(State.OnEnter());
    }

    public State GetState()
    {
        if (State != null)
        {
            return State;
        }
        else
            return null; 
    }
}
