using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Trajectory : MonoBehaviour
{
    private Rigidbody rb;
    [SerializeField] private Scene simulationScene;
    private PhysicsScene physicsScene;
    [SerializeField] private LineRenderer line;
    [SerializeField] private int maxPhysicsFrameIterations = 3;
    [SerializeField] private GameObject[] points;


    private void Start()
    {
        CreatePhysicsScene(); 
    }

    public void DestroyScene()
    {
        SceneManager.UnloadSceneAsync(simulationScene);
    }

    private void CreatePhysicsScene()
    {
        ShowOrNot(false);

        line = gameObject.GetComponent<LineRenderer>();

        //Creating a simulation scene on top and giving it the physics of the current scene
        simulationScene = SceneManager.CreateScene("Simulation", new CreateSceneParameters(LocalPhysicsMode.Physics3D));
        physicsScene = simulationScene.GetPhysicsScene();
    }

    public void SimulateTrajectory(GameObject ball, Vector3 pos, Vector3 velocity)
    {

        ShowOrNot(true);

        var ghostObj = Instantiate(ball, pos, Quaternion.identity);
        ghostObj.GetComponent<Renderer>().enabled = false; //doing this since we can see the new spawned objects in the simulation scene
        //ghostObj.GetComponent<LineRenderer>().enabled = false;

        SceneManager.MoveGameObjectToScene(ghostObj, simulationScene);


        rb = ghostObj.AddComponent<Rigidbody>();
        rb.useGravity = false;

        FakeFire(velocity);

        line.positionCount = maxPhysicsFrameIterations;

        for (int i = 0; i < maxPhysicsFrameIterations; i++)
        {
            physicsScene.Simulate(Time.fixedDeltaTime);
            line.SetPosition(i, ghostObj.transform.position);
        }

        line.enabled = false;

        Vector3[] positions = new Vector3[line.positionCount];
        line.GetPositions(positions);

        float totalLength = 0f;
        float currentPercentage = 0;

        for (int i = 1; i < positions.Length; i++)
        {
            totalLength += Vector3.Distance(positions[i - 1], positions[i]);
        }

        foreach(var point in points)
        {
            point.transform.position = line.GetPosition(0) + velocity.normalized * currentPercentage * totalLength;
            currentPercentage += 0.25f;
        }
        Destroy(ghostObj);
    }

    public void ShowOrNot(bool show)
    {
        foreach(var point in points)
        {
            point.SetActive(show);
        }
    }

    public void FakeFire(Vector3 velocity)
    {
        rb.AddForce(velocity, ForceMode.Impulse);
    }
}
