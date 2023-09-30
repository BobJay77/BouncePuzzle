using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

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
        trajectory = GameSystem.instance.ghostBallSceneCopy.GetComponent<Trajectory>();
        GameSystem.ghostBallSceneCopy.transform.position = GameSystem.playerBallSceneCopy.transform.position;
        GameSystem.ghostBallSceneCopy.SetActive(false);
        GameSystem.playerBallSceneCopy.SetActive(true);
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
                    GameSystem.ghostBallSceneCopy.SetActive(true);
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
                GameSystem.ghostBallSceneCopy.SetActive(false);
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
            float distance = Vector3.Distance(GameSystem.playerBallSceneCopy.transform.position, initialClickPos);
            if (distance < 1)
                distance = 1f;

            GameSystem.playerBallSceneCopy.GetComponent<Rigidbody>().AddForce((initialClickPos - GameSystem.playerBallSceneCopy.transform.position).normalized *
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
            GameSystem.ghostBallSceneCopy.transform.position = initialClickPos;
            if (distance > maxDragDistance)
            {
                // Clamp to maximum distance
                distance = maxDragDistance;
                direction = direction.normalized * maxDragDistance;
                GameSystem.playerBallSceneCopy.transform.position = initialClickPos + direction;
            }
            else
            {
                // Move the ball to mouse position
                GameSystem.playerBallSceneCopy.transform.position = GameSystem.mousePosition;
            }

            // The ghost ball is in the inital click position to show the center of the click
            GameSystem.ghostBallSceneCopy.transform.position = initialClickPos;

            Vector3 linePos = GameSystem.playerBallSceneCopy.transform.position + ((direction.normalized) * (GameSystem.ghostBallSceneCopy.GetComponent<SphereCollider>().radius / 2));

            trajectory.SimulateTrajectory(GameSystem.ghostBallSceneCopy, linePos, -direction.normalized * distance * GameSystem.instance.speedMultiplier * 8);
        }
        else
        {
            GameSystem.playerBallSceneCopy.transform.position = GameSystem.mousePosition;
            GameSystem.ghostBallSceneCopy.transform.position = GameSystem.playerBallSceneCopy.transform.position;
        }
    }
}