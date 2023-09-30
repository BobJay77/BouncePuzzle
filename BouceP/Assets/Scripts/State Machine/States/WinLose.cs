using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinLose : State //rename to win lose
{
    private bool won;

    public WinLose(GameSystem gameSystem, bool win = false) : base(gameSystem)
    {
        won = win;
    }

    public WinLose Won(bool winorlose = false)
    {
        won = winorlose;
        GameSystem.TurnOnNextButton(won);
        return this;
    }

    public override IEnumerator OnEnter()
    {
        GameSystem.DestroyPrefab(GameSystem.playerBall);
        GameSystem.DestroyPrefab(GameSystem.ghostBall);

        if (won)
        {
            AudioManager.instance.PlayOneShotSound(GameSystem.winLoseSounds.audioGroup,
                                                   GameSystem.winLoseSounds.audioClip,
                                                   Camera.main.transform.position,
                                                   GameSystem.winLoseSounds.volume,
                                                   GameSystem.winLoseSounds.spatialBlend,
                                                   GameSystem.winLoseSounds.priority);

            if (GameSystem.instance.LevelInfos.Count >= GameSystem.instance.CurrentLevelInfo.levelID)
            {
                GameSystem.instance.LevelInfos[GameSystem.instance.CurrentLevelInfo.levelID].locked = false;
                GameSystem.instance.DataService.SaveData<List<LevelInfo>>("/levels.json", GameSystem.instance.LevelInfos, GameSystem.instance.EncryptionEnabled);
            }


            yield break;
        }
        else
        {
            // Fetch a sound from bank 2
            AudioClip clip = GameSystem.winLoseSounds[1];
            if (clip)
            {
                AudioManager.instance.PlayOneShotSound(GameSystem.winLoseSounds.audioGroup,
                                                       clip,
                                                       Camera.main.transform.position,
                                                       GameSystem.winLoseSounds.volume,
                                                       GameSystem.winLoseSounds.spatialBlend,
                                                       GameSystem.winLoseSounds.priority);
            }
        }
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
        //GameSystem.SetState(GameSystem.startGameState);
    }
}
