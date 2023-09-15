using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lose : State
{
    public Lose(GameSystem gameSystem) : base(gameSystem)
    {
    }

    public override void OnUpdate()
    {
        //if we win and press next game or restart level
        GameSystem.SetState(GameSystem.startGameState);
    }
}
