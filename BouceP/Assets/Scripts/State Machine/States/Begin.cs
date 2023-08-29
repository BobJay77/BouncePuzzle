using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Begin : State
{
    public Begin(GameSystem gameSystem) : base(gameSystem)
    {
    }

    public override IEnumerator OnEnter()
    {
        yield return new WaitForSeconds(2f);

        GameSystem.SetState(new PlayerTurn(GameSystem));
    }
}
