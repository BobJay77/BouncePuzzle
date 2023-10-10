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
        if (GameSystem.CurrentLevelInfo.hasTutorial)
            GameSystem.actionTextParent.SetActive(true);

        yield return new WaitForSeconds(2f);

        if (GameSystem.CurrentLevelInfo.hasTutorial)
            GameSystem.actionTextParent.SetActive(false);

        GameSystem.bouncesGoal = GameSystem.CurrentLevelInfo.numOfBouncesToWin;
        GameSystem.bouncesText.text = "Bounces: " + GameSystem.bouncesGoal.ToString();

        // Instantiate projectile prefab
        GameSystem.projectilePrefabSceneCopy = GameSystem.SpawnPrefab(GameSystem.loadedProjectilePrefab, GameObject.FindGameObjectWithTag("Ball").transform.position);
        GameSystem.projectilePrefabSceneCopy.transform.SetParent(GameObject.FindGameObjectWithTag("Ball").transform);

        // Instantiate ghost ball prefab
        GameSystem.ghostBallSceneCopy = GameSystem.SpawnPrefab(GameSystem.ghostBall, GameObject.FindGameObjectWithTag("Ball").transform.position);
        GameSystem.ghostBallSceneCopy.transform.position = GameObject.FindGameObjectWithTag("Ball").transform.position;
        //GameSystem.ghostBall.SetActive(false);

        // Change state to player turn state
        GameSystem.SetState(GameSystem.playerTurnState);
    }
}
