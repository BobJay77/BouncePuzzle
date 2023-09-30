using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerTurn : State
{

    [SerializeField] float maxDragDistance = 3;
    public Vector3 initialClickPos;
    public bool shootMode = false;
    public bool addForce = false;

    public Trajectory trajectory;

    public string draggingTag = "Ball";  // Tag of objects that can be dragged.

    //Double touch
    public float doubleTouchThreshold = 0.3f; // Adjust this value as needed

    private int touchCount = 0;
    private float lastTouchTime = 0f;


    // Variables for tracking touch input and dragging state.
    private Vector3 dis;
    private float posX;
    private float posY;
    private bool touched = false;  // Is an object currently touched?
    private bool dragging = false; // Is an object currently being dragged?

    // Variables to keep track of the object being dragged and its rigidbody.
    private Transform toDrag;
    private Rigidbody toDragRigidbody;
    private Vector3 previousPosition;


    private RaycastHit hit;
    public PlayerTurn(GameSystem gameSystem) : base(gameSystem)
    {

    }


    public override IEnumerator OnEnter()
    {
        trajectory = GameSystem.instance.ghostBall.GetComponent<Trajectory>();
        GameSystem.ghostBall.transform.position = GameSystem.playerBall.transform.position;
        GameSystem.ghostBall.SetActive(false);
        //GameSystem.playerBall.SetActive(true);
        GameSystem.actionText.text = "Drag the ball back and let go to move it.";
        GameSystem.roundEnded = false;
        GameSystem.hitGoal = false;

        yield break;
    }

    public override void OnUpdate()
    {
        // Check if touch does not equal 1
        if (Input.touchCount != 1)
        {
            dragging = false;
            touched = false;
            if (toDragRigidbody)
            {
                SetFreeProperties(toDragRigidbody);
            }
            return;
        }

        Touch touch = Input.touches[0];
        Vector3 pos = touch.position;

        // Touch begins
        if (touch.phase == TouchPhase.Began)
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(pos);

            if (Physics.Raycast(ray, out hit) && hit.collider.tag == draggingTag)
            {
                toDrag = hit.transform;
                previousPosition = toDrag.position;
                toDragRigidbody = toDrag.GetComponent<Rigidbody>();
                Debug.Log("touch detected!");
                dis = Camera.main.WorldToScreenPoint(previousPosition);
                posX = Input.GetTouch(0).position.x - dis.x;
                posY = Input.GetTouch(0).position.y - dis.y;

                SetDraggingProperties(toDragRigidbody);

                touched = true;

                float currentTime = Time.time;

                if (touchCount == 1 && currentTime - lastTouchTime <= doubleTouchThreshold)
                {
                    // Double touch detected
                    shootMode = true;
                    GameSystem.ghostBall.SetActive(true);
                    initialClickPos = GameSystem.mousePosition;
                    touchCount = 0; // Reset touchCount
                }
                else
                {
                    touchCount = 1;
                }

                lastTouchTime = currentTime;
            }
        }
        
        // Touch has been moved/dragged
        if (touched && touch.phase == TouchPhase.Moved)
        {

            dragging = true;

            previousPosition = toDrag.position;
            UpdateBallPos(touch);
        }

        //Touch has ended
        if (dragging && (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled))
        {
            dragging = false;
            touched = false;
            previousPosition = new Vector3(0.0f, 0.0f, 0.0f);
            SetFreeProperties(toDragRigidbody);


            if (shootMode)
            {
                GameSystem.ghostBall.SetActive(false);
                trajectory.ShowOrNot(false);

                shootMode = false;
                addForce = true;

                GameSystem.TriggerVFX(GameSystem.muzzlePrefab);

                GameSystem.actionText.text = "";


            }
        }

    }

    // Set properties for dragging the object.
    private void SetDraggingProperties(Rigidbody rb)
    {
        rb.useGravity = false;
        rb.drag = 3;
    }

    // Set properties for when the object is not being dragged.
    private void SetFreeProperties(Rigidbody rb)
    {
        rb.useGravity = false;
        rb.drag = 0;
    }

    public override void OnFixedUpdate()
    {
        if (addForce)
        {
            float distance = Vector3.Distance(GameSystem.playerBall.transform.position, initialClickPos);
            if (distance < 1)
                distance = 1f;

            GameSystem.playerBall.GetComponent<Rigidbody>().AddForce((initialClickPos - GameSystem.playerBall.transform.position).normalized *
                                                           distance * GameSystem.speedMultiplier, ForceMode.Impulse);

            addForce = false;

            GameSystem.SetState(GameSystem.resolutionState);
        }
    }

    public override IEnumerator OnExit()
    {

        yield return null;
    }

    public void UpdateBallPos(Touch touch)
    {
        Vector3 direction = GameSystem.mousePosition - initialClickPos;
        float distance = direction.magnitude;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.GetMask("Wall")))
            return;

        if (shootMode)
        {
            GameSystem.ghostBall.transform.position = initialClickPos;
            if (distance > maxDragDistance)
            {
                // Clamp to maximum distance
                distance = maxDragDistance;
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

            Vector3 linePos = GameSystem.playerBall.transform.position + ((direction.normalized) * (GameSystem.ghostBall.GetComponent<SphereCollider>().radius / 2));

            trajectory.SimulateTrajectory(GameSystem.ghostBall, linePos, -direction.normalized * distance * GameSystem.instance.speedMultiplier * 8);
        }
        else
        {
            GameSystem.playerBall.transform.position = GameSystem.mousePosition;
            GameSystem.ghostBall.transform.position = GameSystem.playerBall.transform.position;
        }
    }
}