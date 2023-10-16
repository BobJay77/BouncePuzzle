using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackHole : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (GameSystem.instance.GetState().ToString() != "PlayerTurn")
        {
            if (other.transform.tag == "Projectile")
            {
                other.transform.position = new Vector3(transform.position.x, transform.position.y, -0.2f);

                //other.transform.position = transform.position;

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
            gameObject.GetComponent<Collider>().enabled = false;
            this.gameObject.GetComponent<ScaleToScreenSize>().enabled = false;
            LeanTween.scale(gameObject, new Vector3(0, 0, 0), 1f);
        }
    }

    public void ResetBlackHoles()
    {
        var blackholes = GameObject.FindObjectsOfType<BlackHole>();

        foreach (var blackhole in blackholes)
        {

            LeanTween.scale(blackhole.gameObject, new Vector3(1, 1, 1), 1f).setOnComplete(delegate () { blackhole.gameObject.GetComponent<ScaleToScreenSize>().enabled = true; });
            
            blackhole.GetComponent<Collider>().enabled = true;
        }
    }
}
