//using System.Collections;
//using UnityEngine;

//public class MoveObjects : MonoBehaviour
//{
//    public string draggingTag;  // Tag of objects that can be dragged.
//    public Camera cam;          // Reference to the camera used for raycasting.

//    // Variables for tracking touch input and dragging state.
//    private Vector3 dis;
//    private float posX;
//    private float posY;
//    private bool touched = false;  // Is an object currently touched?
//    private bool dragging = false; // Is an object currently being dragged?

//    // Variables to keep track of the object being dragged and its rigidbody.
//    private Transform toDrag;
//    private Rigidbody toDragRigidbody;
//    private Vector3 previousPosition;

//    void FixedUpdate()
//    {
//        // Check if there is more than one touch input.
//        if (Input.touchCount != 1)
//        {
//            dragging = false;
//            touched = false;

//            // If there is a rigidbody associated with the object being dragged, reset its properties.
//            if (toDragRigidbody)
//            {
//                SetFreeProperties(toDragRigidbody);
//            }
//            return;
//        }

//        Touch touch = Input.touches[0];
//        Vector3 pos = touch.position;

//        if (touch.phase == TouchPhase.Began)
//        {
//            // When a touch begins, check if it hits an object with the specified tag.
//            RaycastHit hit;
//            Ray ray = cam.ScreenPointToRay(pos);

//            GameSystem.instance.playerTurnState.initialClickPos = GameSystem.instance.playerBall.transform.position;
//            // Reset velocity
//            GameSystem.instance.playerBall.GetComponent<Rigidbody>().velocity = Vector3.zero;

//            Vector3 direction = touch.position - (Vector2)GameSystem.instance.playerTurnState.initialClickPos;
//            float distance = direction.magnitude;
//            if (Physics.Raycast(GameSystem.instance.playerTurnState.initialClickPos, direction.normalized, out hit, distance, LayerMask.GetMask("Wall")))
//                return;

//            GameSystem.instance.actionText.text = "Drag the ball back and let go to move it.";
//            GameSystem.instance.playerTurnState.shootMode = true;

//            if (Physics.Raycast(ray, out hit) && hit.collider.tag == draggingTag)
//            {
//                toDrag = hit.transform;
//                previousPosition = toDrag.position;
//                toDragRigidbody = toDrag.GetComponent<Rigidbody>();

//                // Calculate the initial displacement from touch position to object's position.
//                dis = cam.WorldToScreenPoint(previousPosition);
//                posX = Input.GetTouch(0).position.x - dis.x;
//                posY = Input.GetTouch(0).position.y - dis.y;

//                // Set properties for dragging the object.
//                SetDraggingProperties(toDragRigidbody);

//                touched = true;
//            }
//        }

//        if (touched && touch.phase == TouchPhase.Moved)
//        {
//            dragging = true;

//            // Calculate the new position for the object based on touch input.
//            float posXNow = Input.GetTouch(0).position.x - posX;
//            float posYNow = Input.GetTouch(0).position.y - posY;
//            Vector3 curPos = new Vector3(posXNow, posYNow, dis.z);

//            Vector3 worldPos = cam.ScreenToWorldPoint(curPos) - previousPosition;
//            worldPos = new Vector3(worldPos.x, worldPos.y, 0.0f);

//            // Apply velocity to the rigidbody to move the object.
//            toDragRigidbody.velocity = worldPos / (Time.deltaTime * 30);

//            previousPosition = toDrag.position;
//        }

//        if (dragging && (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled))
//        {
//            // When touch ends or is canceled, reset dragging state and object properties.
//            dragging = false;
//            touched = false;
//            previousPosition = new Vector3(0.0f, 0.0f, 0.0f);
//            SetFreeProperties(toDragRigidbody);

//            GameSystem.instance.ghostBall.SetActive(false);

//            GameSystem.instance.playerTurnState.shootMode = false;

//            GameSystem.instance.TriggerVFX(GameSystem.instance.muzzlePrefab);

//            GameSystem.instance.playerTurnState.addForce = true;

//            GameSystem.instance.playerTurnState.trajectory.ShowOrNot(false);

//            GameSystem.instance.actionText.text = "";
//        }
//    }

//    // Set properties for dragging the object.
//    private void SetDraggingProperties(Rigidbody rb)
//    {
//        rb.useGravity = false;
//        rb.drag = 3;
//    }

//    // Set properties for when the object is not being dragged.
//    private void SetFreeProperties(Rigidbody rb)
//    {
//        rb.useGravity = false;
//        rb.drag = 0;
//    }
//}

using System.Collections;
using UnityEngine;

public class MoveObjects : MonoBehaviour
{

    public string draggingTag;
    public Camera cam;

    private Vector3 dis;
    private float posX;
    private float posY;

    private bool touched = false;
    private bool dragging = false;

    private Transform toDrag;
    private Rigidbody toDragRigidbody;
    private Vector3 previousPosition;

    void FixedUpdate()
    {

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

        if (touch.phase == TouchPhase.Began)
        {
            RaycastHit hit;
            Ray ray = cam.ScreenPointToRay(pos);

            if (Physics.Raycast(ray, out hit) && hit.collider.tag == draggingTag)
            {
                toDrag = hit.transform;
                previousPosition = toDrag.position;
                toDragRigidbody = toDrag.GetComponent<Rigidbody>();

                dis = cam.WorldToScreenPoint(previousPosition);
                posX = Input.GetTouch(0).position.x - dis.x;
                posY = Input.GetTouch(0).position.y - dis.y;

                SetDraggingProperties(toDragRigidbody);

                touched = true;
            }
        }

        if (touched && touch.phase == TouchPhase.Moved)
        {
            dragging = true;

            float posXNow = Input.GetTouch(0).position.x - posX;
            float posYNow = Input.GetTouch(0).position.y - posY;
            Vector3 curPos = new Vector3(posXNow, posYNow, dis.z);

            Vector3 worldPos = cam.ScreenToWorldPoint(curPos) - previousPosition;
            worldPos = new Vector3(worldPos.x, worldPos.y, 0.0f);

            toDragRigidbody.velocity = worldPos / (Time.deltaTime * 10);

            previousPosition = toDrag.position;
        }

        if (dragging && (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled))
        {
            dragging = false;
            touched = false;
            previousPosition = new Vector3(0.0f, 0.0f, 0.0f);
            SetFreeProperties(toDragRigidbody);
        }

    }

    private void SetDraggingProperties(Rigidbody rb)
    {
        rb.useGravity = false;
        rb.drag = 20;
    }

    private void SetFreeProperties(Rigidbody rb)
    {
        rb.useGravity = false;
        rb.drag = 5;
    }
}
