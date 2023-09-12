using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Resolution : State
{
    public Resolution(GameSystem gameSystem) : base(gameSystem)
    {

    }

    public override IEnumerator OnEnter()
    {
        yield return new WaitForSeconds(2f);

        GameSystem.SetState(new WinLose(GameSystem, true));//pass in bool for win or lose condition
    }

    public override void OnUpdate()
    {
        //// if we win countbounce == maxbouce
        //GameSystem.SetState(new WinLose(GameSystem, true));

        ////else
        //GameSystem.SetState(new WinLose(GameSystem));
    }

    public override IEnumerator OnExit()
    {
        GameSystem.playerBall.GetComponent<Rigidbody>().velocity = Vector3.zero;
        yield return null;
    }
}
