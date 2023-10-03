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

        GameSystem.projectilePrefabSceneCopy = GameSystem.SpawnPrefab(GameSystem.loadedProjectilePrefab, GameObject.FindGameObjectWithTag("Ball").transform.position);
        GameSystem.projectilePrefabSceneCopy.transform.SetParent(GameObject.FindGameObjectWithTag("Ball").transform);

        //GameSystem.playerBallSceneCopy = GameSystem.SpawnPrefab(GameSystem.playerBall, GameObject.FindGameObjectWithTag("StartingPosition").transform.position);
        GameSystem.ghostBallSceneCopy = GameSystem.SpawnPrefab(GameSystem.ghostBall, GameObject.FindGameObjectWithTag("Ball").transform.position);
        GameSystem.ghostBallSceneCopy.transform.position = GameObject.FindGameObjectWithTag("Ball").transform.position;
        //GameSystem.ghostBall.SetActive(false);

        GameSystem.SetState(GameSystem.playerTurnState);

        
    }
}
