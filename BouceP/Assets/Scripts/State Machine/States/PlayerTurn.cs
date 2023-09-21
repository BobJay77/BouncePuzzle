using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerTurn : State
{

    [SerializeField] float maxDragDistance = 3;
    public Vector3 initialClickPos;
    public bool shootMode = false;


    private RaycastHit hit;
    public PlayerTurn(GameSystem gameSystem) : base(gameSystem)
    {

    }


    public override IEnumerator OnEnter()
    {
        GameSystem.playerBall.GetComponent<MeshRenderer>().enabled = false;
        GameSystem.ghostBall.SetActive(true);
        GameSystem.actionText.text = "Drag the ball back and let go to move it.";
        GameSystem.roundEnded = false;
        GameSystem.hitGoal = false;

        yield break;
    }

    public override void OnUpdate()
    {
        if (!shootMode)
        {
            if (Input.GetMouseButtonDown(0))
            {
                initialClickPos = GameSystem.playerBall.transform.position;

                // Reset velocity
                GameSystem.playerBall.GetComponent<Rigidbody>().velocity = Vector3.zero;
                
                Vector3 direction = GameSystem.mousePosition - initialClickPos;
                float distance = direction.magnitude;

                if (Physics.Raycast(initialClickPos, direction.normalized, out hit, distance, LayerMask.GetMask("Wall")))
                    return;



                shootMode = true;
                GameSystem.playerBall.GetComponent<MeshRenderer>().enabled = true;

            }
            else
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.GetMask("Wall")))
                {
                    return;
                }

                if (Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.GetMask("redZone")))
                {
                    RedZone tempRedZone = hit.transform.gameObject.GetComponent<RedZone>();
                    Color newcolor = tempRedZone.redZoneMaterial.color;
                    tempRedZone.timer = 0;
                    newcolor.a = 1f;
                    tempRedZone.redZoneMaterial.color = newcolor;
                    GameSystem.actionText.text = "Cannot Place the ball in the redzone";
                    return;
                }

                GameSystem.actionText.text = "Drag the ball back and let go to move it.";
            }

            

            // When hovering with a mouse, position the ghostBall to cursor
            GameSystem.playerBall.transform.position = GameSystem.mousePosition;
            GameSystem.ghostBall.transform.position = GameSystem.mousePosition;
        }

        else
        {
            UpdateBallPos();

            if (Input.GetMouseButtonUp(0))
            {
                GameSystem.ghostBall.SetActive(false);

                shootMode = false;

                float distance = Vector3.Distance(GameSystem.playerBall.transform.position, initialClickPos);

                if (distance < 0.5)
                    distance = 0.5f;

                GameSystem.playerBall.GetComponent<Rigidbody>().AddForce((initialClickPos - GameSystem.playerBall.transform.position).normalized *
                                                               distance *
                                                               GameSystem.multiplier *
                                                               Time.deltaTime);

                GameSystem.actionText.text = "";

                GameSystem.SetState(GameSystem.resolutionState);
            }
        }
    }

    public override IEnumerator OnExit()
    {

        yield return null;
    }

    public void UpdateBallPos()
    {
        Vector3 direction = GameSystem.mousePosition - initialClickPos;
        float distance = direction.magnitude;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.GetMask("Wall")))
            return;

        if (distance > maxDragDistance)
        {
            // Clamp to maximum distance
            direction = direction.normalized * maxDragDistance;
            GameSystem.playerBall.transform.position = initialClickPos + direction;
        }
        else
        {
            // Move the ball to mouse position
            GameSystem.playerBall.transform.position = GameSystem.mousePosition;
        }

        // The ghost ball is in the inital click position to show the center of the click
        GameSystem.ghostBall.transform.position = initialClickPos;
    }
}