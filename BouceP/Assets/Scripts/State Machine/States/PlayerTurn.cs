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
        yield break;
    }

    public override void OnUpdate()
    {
        if (!shootMode)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Debug.Log("Here2");
                initialClickPos = GameSystem.playerBall.transform.position;

                Vector3 direction = GameSystem.mousePosition - initialClickPos;
                float distance = direction.magnitude;

                if (Physics.Raycast(initialClickPos, direction.normalized, out hit, distance, LayerMask.GetMask("wall")))
                    return;



                shootMode = true;
                GameSystem.playerBall.GetComponent<MeshRenderer>().enabled = true;
            }

            else
            {
                Vector3 direction = GameSystem.mousePosition - initialClickPos;
                float distance = direction.magnitude;

                if (Physics.Raycast(initialClickPos, direction.normalized, out hit, distance, LayerMask.GetMask("redZone")))
                {
                    RedZone tempRedZone = hit.transform.gameObject.GetComponent<RedZone>();
                    Color newcolor = tempRedZone.redZoneMaterial.color;
                    Debug.Log(newcolor);
                    tempRedZone.timer = 0;
                    newcolor.a = 1f;
                    tempRedZone.redZoneMaterial.color = newcolor;
                    return;
                }
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

                GameSystem.playerBall.GetComponent<Rigidbody>().AddForce((initialClickPos - GameSystem.playerBall.transform.position) *
                                                               Vector3.Distance(GameSystem.playerBall.transform.position, initialClickPos) *
                                                               GameSystem.multiplier *
                                                               Time.deltaTime);

                GameSystem.actionText.text = "";

                GameSystem.SetState(new Resolution(GameSystem));
            }
        }
    }

    public void UpdateBallPos()
    {
        Vector3 direction = GameSystem.mousePosition - initialClickPos;
        float distance = direction.magnitude;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.GetMask("wall")))
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