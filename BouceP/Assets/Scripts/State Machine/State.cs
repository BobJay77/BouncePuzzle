using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class State
{
    protected GameSystem GameSystem;


    public State(GameSystem gameSystem)
    {
        GameSystem = gameSystem;
    }

    public virtual void OnUpdate()
    {
    }

    public virtual void OnFixedUpdate()
    {

    }

    public virtual IEnumerator OnEnter()
    {
        yield break;
    }
    public virtual IEnumerator OnExit()
    {
        yield break;
    }

    public virtual IEnumerator ShootRoutine()
    {
        yield break;
    }
}
