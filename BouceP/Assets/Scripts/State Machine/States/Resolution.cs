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

            // Updating UI text for number of bounces
            if (GameSystem.bouncesText.text != GameSystem.bouncesGoal.ToString())
                GameSystem.bouncesText.text = "Bounces: " + GameSystem.bouncesGoal.ToString();

            // If the number of bounces are achieved the goal is ready for the win condition
            if (GameSystem.bouncesGoal == 0)
                GameSystem.bouncesText.text = "Goal Ready";
        }
        else
        {
            // Number of bounces condition must be met exactly not over or under to win, otherwise it is a loss
            if (GameSystem.currentBounces == GameSystem.CurrentLevelInfo.numOfBouncesToWin && GameSystem.hitGoal)
            {
                GameSystem.bouncesText.text = "";
                GameSystem.actionText.text  = "You Won!";
                Debug.Log("You Won!");

                // Change to winLose state
                GameSystem.SetState(GameSystem.winLoseState.Won(true));
            }
            else
            {
                GameSystem.bouncesText.text = "";
                GameSystem.actionText.text  = "You Lost!";
                Debug.Log("You Lost!");

                // Change to winLose state
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
