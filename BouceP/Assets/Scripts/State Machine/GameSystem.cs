using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSystem : StateMachine
{
    private void Start()
    {
        SetState(new Begin(this));
    }

    private void Update()
    {
        State.OnUpdate();
    }
}
