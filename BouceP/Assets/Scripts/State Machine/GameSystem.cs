using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameSystem : StateMachine
{
    public GameObject playerBall;
    public GameObject ghostBall;
    public GameObject winOrLoseParent;

    public TMP_Text actionText;
   
    public Vector3 mousePosition;
    public int bounces;
    public static GameSystem instance;

    [SerializeField] public float multiplier;


    private void Awake()
    {
        if (instance == null)
            instance = this;

        else
            Destroy(gameObject);
    }

    private void Start()
    {
        SetState(new StartGame(this));
    }

    private void Update()
    {
        mousePosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -Camera.main.transform.position.z));
        State.OnUpdate();
    }

    public void TurnOnNextButton(bool on)
    {
        winOrLoseParent.SetActive(true);
       
        if (on)
        {
            winOrLoseParent.GetComponent<WinLoseUIParent>().next.gameObject.SetActive(true);
        }
    } 
}