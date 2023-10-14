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

                other.gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
                GameSystem.instance.blackHoleShot = true;
                GameSystem.instance.SetState(GameSystem.instance.playerTurnState);
                gameObject.GetComponent<Collider>().enabled = false;
                //Set a gamesystem bool that checks if it has gone tru a black hole then set the playerturn state
            }
        }
    }

    public void ResetBlackHoles()
    {
        var blackholes = GameObject.FindObjectsOfType<BlackHole>();

        foreach (var blackhole in blackholes)
        {
            blackhole.GetComponent<Collider>().enabled = true;
        }
    }
}
