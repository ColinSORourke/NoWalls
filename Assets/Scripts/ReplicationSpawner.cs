using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReplicationSpawner : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject prefab;
    public GameObject sceneCamera;
    public int replicationSpacing = 5;
    public static int replicationCount = 30;
    private static int halfRepVolume = 0;


    private Vector3 lastCameraForward;
    private Vector3 lastCameraPostion;
    private Vector3 unitForward = new Vector3(0, 0, 1);
    bool isInCurViewport = false;
    bool isInLastViewport = false;


    public GameObject[,,] arrayOfObjects;
    public ReplicationSpawner()
    {
        this.arrayOfObjects = new GameObject[replicationCount + 1, replicationCount + 1, replicationCount + 1];
        halfRepVolume = replicationCount * replicationSpacing / 2;
    }

    void PositionIsInViewport(Vector3 position, Vector3 cameraPosition, Vector3 cameraForward, out bool isInViewport)
    {
        // Vector3 viewportPosition = sceneCamera.GetComponent<Camera>().WorldToViewportPoint(position);
        // isInViewport = viewportPosition.x >= 0 && viewportPosition.x <= 1 && viewportPosition.y >= 0 && viewportPosition.y <= 1 && viewportPosition.z > 0;

        Vector3 positionRelativeToCamera = position - cameraPosition;
        float angle = Vector3.Angle(positionRelativeToCamera, cameraForward);
        isInViewport = (angle < 20) && (positionRelativeToCamera.magnitude < 100);
    }

    void Start()
    {
        Vector3 cameraPosition = sceneCamera.transform.position;
        Vector3 cameraForward = sceneCamera.transform.forward;

        //instaniate this object at a 3d grid of positions
        for (int i = 0; i < replicationCount; ++i)
        {
            for (int j = 0; j < replicationCount; ++j)
            {
                for (int k = 0; k < replicationCount; ++k)
                {
                    if (i == 0 && j == 0 && k == 0)
                    {
                        continue;
                    }
                    Vector3 newPosition = new Vector3(i * replicationSpacing - halfRepVolume, j * replicationSpacing - halfRepVolume, k * replicationSpacing - halfRepVolume);
                    print(i + ", " + j + ", " + k + " | " + arrayOfObjects.GetUpperBound(0) + " | " + newPosition);
                    PositionIsInViewport(newPosition, cameraPosition, cameraForward, out isInCurViewport);
                    if (isInCurViewport)
                    {
                        GameObject newObject = Instantiate(prefab, newPosition, Quaternion.identity);
                        newObject.transform.parent = transform;


                        arrayOfObjects[i, j, k] = newObject;
                    }
                }
            }
        }
        lastCameraPostion = cameraPosition;
        lastCameraForward = cameraForward;
    }

    // Update is called once per frame


    void Update()
    {
        Vector3 cameraPosition = sceneCamera.transform.position;
        Vector3 cameraForward = sceneCamera.transform.forward;
        if (cameraPosition == lastCameraPostion && cameraForward == lastCameraForward)
        {
            return;
        }
        else
        {
            print(cameraForward);
        }
        //instaniate this object at a 3d grid of positions
        for (int i = 0; i < replicationCount; ++i)
        {
            for (int j = 0; j < replicationCount; ++j)
            {
                for (int k = 0; k < replicationCount; ++k)
                {
                    if (i == 0 && j == 0 && k == 0)
                    {
                        continue;
                    }

                    Vector3 newPosition = new Vector3(i * replicationSpacing - halfRepVolume, j * replicationSpacing - halfRepVolume, k * replicationSpacing - halfRepVolume);
                    PositionIsInViewport(newPosition, cameraPosition, cameraForward, out isInCurViewport);
                    PositionIsInViewport(newPosition, lastCameraPostion, lastCameraForward, out isInLastViewport);

                    if (isInCurViewport && !isInLastViewport)
                    {
                        GameObject newObject = Instantiate(prefab, newPosition, Quaternion.identity);
                        newObject.transform.parent = transform;
                        arrayOfObjects[i, j, k] = newObject;
                    }
                    else if (!isInCurViewport && isInLastViewport)
                    {
                        Destroy(arrayOfObjects[i, j, k]);
                    }

                }
            }
        }
        lastCameraPostion = sceneCamera.transform.position;
        lastCameraForward = sceneCamera.transform.forward;
    }
}
