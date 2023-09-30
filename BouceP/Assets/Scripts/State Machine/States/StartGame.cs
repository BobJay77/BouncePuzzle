using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartGame : State
{
    public StartGame(GameSystem gameSystem) : base(gameSystem)
    {
    }

    public override IEnumerator OnEnter()
    {

        yield return new WaitForSeconds(2f);
        
        GameSystem.bouncesGoal = GameSystem.CurrentLevelInfo.numOfBouncesToWin;
        GameSystem.bouncesText.text = "Bounces: " + GameSystem.bouncesGoal.ToString();

        GameSystem.playerBall = GameSystem.SpawnPrefab(GameSystem.playerBall);
        GameSystem.ghostBall = GameSystem.SpawnPrefab(GameSystem.ghostBall);
        GameSystem.playerBall.transform.position = GameObject.FindGameObjectWithTag("StartingPosition").transform.position;
        GameSystem.ghostBall.transform.position = GameObject.FindGameObjectWithTag("StartingPosition").transform.position;
        //GameSystem.ghostBall.SetActive(false);

        GameSystem.SetState(GameSystem.playerTurnState);

        
    }
}
