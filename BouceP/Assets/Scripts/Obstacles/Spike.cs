using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spike : MonoBehaviour
{
    [SerializeField] private GameSystem gameSystem;
    // Start is called before the first frame update
    void Start()
    {
        gameSystem = GameSystem.instance;
    }


    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Ball")
        {
            gameSystem.actionText.text = "Hit Spike";
            gameSystem.SetState(gameSystem.winLoseState.Won(false));
        }
    }
}
