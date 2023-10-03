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
        // Destroy the prefab copies
        GameSystem.DestroyPrefab(GameSystem.projectilePrefabSceneCopy);
        GameSystem.DestroyPrefab(GameSystem.ghostBallSceneCopy);

        // Check if the round is won or last
        if (won)
        {
            // Play audio from the Win bank
            AudioManager.instance.PlayOneShotSound(GameSystem.winLoseSounds.audioGroup,
                                                   GameSystem.winLoseSounds.audioClip,
                                                   Camera.main.transform.position,
                                                   GameSystem.winLoseSounds.volume,
                                                   GameSystem.winLoseSounds.spatialBlend,
                                                   GameSystem.winLoseSounds.priority);

            // Unlock next level and save the data in the Account settings
            if (GameSystem.LevelInfos.Count >= GameSystem.CurrentLevelInfo.levelID)
            {
                GameSystem.LevelInfos[GameSystem.CurrentLevelInfo.levelID].locked = false;
                GameSystem.DataService.SaveData<List<LevelInfo>>("/levels.json", GameSystem.LevelInfos, GameSystem.EncryptionEnabled);

                if (GameSystem.LevelInfos[GameSystem.CurrentLevelInfo.levelID].unlockSkinOnLevel > 0) 
                {
                    GameSystem.AccountSettings.Skins[GameSystem.LevelInfos[GameSystem.CurrentLevelInfo.levelID].unlockSkinOnLevel].isLocked = false;
                    GameSystem.DataService.SaveData<AccountSettings>("/acc.json", GameSystem.AccountSettings, GameSystem.EncryptionEnabled);
                }
            }


            yield break;
        }
        else
        {
            // Fetch a sound from bank 2
            AudioClip clip = GameSystem.winLoseSounds[1];

            if (clip)
            {
                // Play audio from the Lost bank
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
