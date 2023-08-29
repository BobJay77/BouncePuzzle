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

    public virtual IEnumerator OnEnter()
    {
        yield break;
    }

    public virtual IEnumerator OnUpdate()
    {
        yield break;
    }

    public virtual IEnumerator OnExit()
    {
        yield break;
    }
}
