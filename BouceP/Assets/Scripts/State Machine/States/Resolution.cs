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
        //yield return new WaitForSeconds(.1f);

        yield return null;
    }

    public override void OnUpdate()
    {
        // Check if round has ended already
        if (!GameSystem.roundEnded)
        {
            // Check if the current # of bounces exceed the goal # of bounces
            if (GameSystem.currentBounces > GameSystem.CurrentLevelInfo.numOfBouncesToWin)
            {
                GameSystem.roundEnded = true;
                return;
            }

            if (GameSystem.bouncesText.text != GameSystem.bouncesGoal.ToString())
                GameSystem.bouncesText.text = "Bounces: " + GameSystem.bouncesGoal.ToString();

            if (GameSystem.bouncesGoal == 0)
                GameSystem.bouncesText.text = "Goal Ready";
        }
        else
        {
            if (GameSystem.currentBounces == GameSystem.CurrentLevelInfo.numOfBouncesToWin && GameSystem.hitGoal)
            {
                GameSystem.bouncesText.text = "";
                GameSystem.actionText.text  = "You Won!";
                Debug.Log("You Won!");

                GameSystem.SetState(GameSystem.winLoseState.Won(true));
            }
            else
            {
                GameSystem.bouncesText.text = "";
                GameSystem.actionText.text  = "You Lost!";
                Debug.Log("You Lost!");

                GameSystem.SetState(GameSystem.winLoseState.Won(false));
            }  
        }
    }

    public override IEnumerator OnExit()
    {
        // Reset velocity
        GameSystem.projectilePrefabSceneCopy.GetComponent<Rigidbody>().velocity = Vector3.zero;
        
        // Reset current number of bounces
        GameSystem.currentBounces = 0;

        yield return null;
    }
}
