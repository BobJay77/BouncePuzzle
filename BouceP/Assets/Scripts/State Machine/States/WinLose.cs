using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinLose : State //rename to win lose
{
    private bool won;
    public WinLose(GameSystem gameSystem, bool winorlose = false) : base(gameSystem)
    {
        won = winorlose;
        gameSystem.TurnOnNextButton(won);
    }

    public void Won(bool winorlose = false)
    {
        won = winorlose;
    }

    public override void OnUpdate()
    {
        if (won) 
        {
            //sceneSwitch and start game
            GameSystem.actionText.text = "Won";
        }

        else
        {
            GameSystem.actionText.text = "Lost";
        }
        //if we win and press next game or restart level
        GameSystem.SetState(new StartGame(GameSystem));
    }
}
