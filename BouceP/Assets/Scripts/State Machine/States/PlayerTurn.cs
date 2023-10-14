using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerTurn : State
{

    [SerializeField] float maxDragDistance = 3;

    public Vector3  initialClickPos;
    public bool     shootMode       = false;
    public bool     addForce        = false;

    public Trajectory trajectory;

    // Variables for tracking touch input and dragging state.
    public  string   draggingTag              = "Projectile";  // Tag of objects that can be dragged.
    public  float    doubleTouchThreshold     = 0.3f;          // Adjust this value as needed
    private bool     dragging                 = false;         // Is an object currently being dragged?
    private bool     touched                  = false;         // Is an object currently touched?
    private int      touchCount               = 0;
    private float    lastTouchTime            = 0f;
    private Vector3  dis;
    private float    posX;
    private float    posY;

    // Variables to keep track of the object being dragged and its rigidbody.
    private Transform   toDrag;
    private Rigidbody   toDragRigidbody;
    private Vector3     previousPosition;

    

    private RaycastHit hit;
    public PlayerTurn(GameSystem gameSystem) : base(gameSystem)
    {

    }

    public override IEnumerator OnEnter()
    {
        trajectory = GameSystem.instance.ghostBallSceneCopy.GetComponent<Trajectory>();
        GameSystem.ghostBallSceneCopy.transform.position = GameSystem.projectilePrefabSceneCopy.transform.position;
        GameSystem.ghostBallSceneCopy.SetActive(false);
        GameSystem.projectilePrefabSceneCopy.SetActive(true);

        if (GameSystem.bouncesGoal >= 0) 
        GameSystem.roundEnded = false;

        else if(GameSystem.bouncesGoal < 0)
            GameSystem.roundEnded = true;

        GameSystem.hitGoal = false;

        // Play projectile loop sfx
        var ballSFX = AudioManager.instance.ballSFXSource;
        ballSFX.clip = AudioManager.instance.projectilesLoopSFX[GameSystem.CurrentSkinIndex];
        if (ballSFX.clip != null) 
            ballSFX.Play();

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

            if (Physics.Raycast(ray, out hit, Mathf.Infinity) && hit.collider.tag == draggingTag) 
            {
                toDrag = hit.transform;
                    previousPosition = toDrag.position;
                    toDragRigidbody = toDrag.GetComponent<Rigidbody>();

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
            previousPosition = Vector3.zero;
            SetFreeProperties(toDragRigidbody);

            GameSystem.projectilePrefabSceneCopy.GetComponent<Rigidbody>().velocity = previousPosition;


            if (shootMode)
            {
                GameSystem.ghostBallSceneCopy.SetActive(false);
                trajectory.ShowOrNot(false);

                shootMode = false;
                addForce = true;

                GameSystem.TriggerVFX(GameSystem.loadedMuzzlePrefab);

                AudioClip clip = AudioManager.instance.muzzlesSFX[GameSystem.instance.CurrentSkinIndex];

                if (clip == null) return;

                AudioManager.instance.PlayOneShotSound(AudioManager.instance.muzzlesSFX.audioGroup,
                                                       clip,
                                                       Camera.main.transform.position,
                                                       AudioManager.instance.muzzlesSFX.volume,
                                                       AudioManager.instance.muzzlesSFX.spatialBlend,
                                                       AudioManager.instance.muzzlesSFX.priority);
            }
        }
    }

    // Set properties for dragging the object.
    private void SetDraggingProperties(Rigidbody rb)
    {
        rb.useGravity = false;
        rb.drag = 10;
        rb.angularDrag = 0;
    }

    // Set properties for when the object is not being dragged.
    private void SetFreeProperties(Rigidbody rb)
    {
        rb.useGravity = false;
        rb.drag = 0;
        rb.angularDrag = 0;
    }

    public override void OnFixedUpdate()
    {
        // Add a force impulse to player when addForce is true
        if (addForce)
        {
            GameSystem.blackHoleShot = false;
            var redzones = GameObject.FindObjectsOfType<RedZone>();

            foreach (var r in redzones)
            {
                r.GetComponent<BoxCollider>().isTrigger = true;
            }

            GameSystem.projectilePrefabSceneCopy.GetComponent<SphereCollider>().material.bounceCombine = PhysicMaterialCombine.Maximum;

            float distance = Vector3.Distance(GameSystem.projectilePrefabSceneCopy.transform.position, initialClickPos);
            if (distance < 1)
                distance = 1f;

            GameSystem.projectilePrefabSceneCopy.GetComponent<Rigidbody>().AddForce((GameSystem.ghostBallSceneCopy.transform.position - GameSystem.projectilePrefabSceneCopy.transform.position).normalized *
                                                           distance * GameSystem.speedMultiplier, ForceMode.Impulse);

            addForce = false;

            // Change to resolution state
            GameSystem.SetState(GameSystem.resolutionState);
        }
    }

    public override IEnumerator OnExit()
    {
        yield return null;
    }

    public void UpdateBallPos(Touch touch)
    {
        GameSystem.projectilePrefabSceneCopy.GetComponent<SphereCollider>().material.bounceCombine = PhysicMaterialCombine.Multiply;

        Vector3 mouseToInitialClickDirection = GameSystem.mousePosition - initialClickPos;
        Vector3 mouseToPlayerBallDirection = GameSystem.mousePosition - GameSystem.projectilePrefabSceneCopy.transform.position;

        if (shootMode)
        {
            GameSystem.ghostBallSceneCopy.transform.position = initialClickPos;

            Vector3 ghostToMouseDirection = GameSystem.mousePosition - GameSystem.ghostBallSceneCopy.transform.position;
            float ghostToPlayerBallDistance = ghostToMouseDirection.magnitude;

            if (ghostToPlayerBallDistance > maxDragDistance)
            {
                // Clamp to maximum distance
                 ghostToPlayerBallDistance = maxDragDistance;

                ghostToMouseDirection.Normalize();
                ghostToMouseDirection *= ghostToPlayerBallDistance;

                Vector3 newPosition = GameSystem.ghostBallSceneCopy.transform.position + ghostToMouseDirection;

                //GameSystem.projectilePrefabSceneCopy.transform.position = newPosition;
                GameSystem.projectilePrefabSceneCopy.GetComponent<Rigidbody>().velocity = (newPosition - GameSystem.projectilePrefabSceneCopy.transform.position) * 30;
            }

            else
            {
                // Move the ball to mouse position
                GameSystem.projectilePrefabSceneCopy.GetComponent<Rigidbody>().velocity = (mouseToPlayerBallDirection) * 10;
            }

            // The ghost ball is in the inital click position to show the center of the click
            GameSystem.ghostBallSceneCopy.transform.position = initialClickPos;

            Vector3 newDir = GameSystem.ghostBallSceneCopy.transform.position - GameSystem.projectilePrefabSceneCopy.transform.position;

            Vector3 linePos = GameSystem.projectilePrefabSceneCopy.transform.position + ((newDir.normalized) * (GameSystem.ghostBallSceneCopy.GetComponent<SphereCollider>().radius / 50));

            trajectory.SimulateTrajectory(GameSystem.ghostBallSceneCopy, linePos, newDir.normalized * newDir.magnitude * GameSystem.instance.speedMultiplier * 8);
        }
        else if(!GameSystem.blackHoleShot && !shootMode)
        {
            GameSystem.projectilePrefabSceneCopy.GetComponent<Rigidbody>().velocity = (mouseToPlayerBallDirection) * 10;
            GameSystem.ghostBallSceneCopy.transform.position = GameSystem.projectilePrefabSceneCopy.transform.position;
        }
    }
}