using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackHole : MonoBehaviour
{
    Vector3 originalScale = Vector3.one;
    private void OnTriggerEnter(Collider other)
    {
        if (GameSystem.instance.GetState().ToString() != "PlayerTurn")
        {
            if (other.transform.tag == "Projectile")
            {

                LeanTween.move(other.gameObject, transform.position, .1f).setOnComplete(delegate () 
                { GameSystem.instance.ghostBallSceneCopy.transform.position = transform.position; });

                int currentBouncesInLevel = GameSystem.instance.currentBounces;

                other.gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
                GameSystem.instance.blackHoleShot = true;
                GameSystem.instance.SetState(GameSystem.instance.playerTurnState);
                GameSystem.instance.currentBounces = currentBouncesInLevel;
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.transform.tag == "Projectile" && (GameSystem.instance.GetState().ToString() != "PlayerTurn" || GameSystem.instance.playerTurnState.shootMode))
        {
            GameSystem.instance.blackHoleShot = false;
            gameObject.GetComponent<Collider>().enabled = false;
            originalScale = gameObject.transform.localScale;
            LeanTween.scale(gameObject, new Vector3(0, 0, 0), 1f);
        }
    }

    public void ResetBlackHoles()
    {
        var blackholes = GameObject.FindObjectsOfType<BlackHole>();

        foreach (var blackhole in blackholes)
        {

            LeanTween.scale(blackhole.gameObject, originalScale, 1f);
            
            blackhole.GetComponent<Collider>().enabled = true;
        }
    }
}
