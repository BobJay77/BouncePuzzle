using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSystem : StateMachine
{
    public GameObject playerBall;
    public GameObject ghostBall;
    public Vector3 mousePosition;

    [SerializeField] public float multiplier;

    private void Start()
    {
        SetState(new StartGame(this));
    }

    private void Update()
    {
        mousePosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -Camera.main.transform.position.z));
        State.OnUpdate();
    }
}