using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinLose : State //rename to win lose
{
    private bool won;
    public WinLose(GameSystem gameSystem, bool win = false) : base(gameSystem)
    {
        won = win;
        gameSystem.TurnOnNextButton(won);
    }

    public WinLose Won(bool winorlose = false)
    {
        won = winorlose;

        return this;
    }

    public override void OnUpdate()
    {
        if (won) 
        {
            //sceneSwitch and start game
            
        }

        else
        {
            
        }
        //if we win and press next game or restart level
        GameSystem.SetState(GameSystem.startGameState);
    }
}
