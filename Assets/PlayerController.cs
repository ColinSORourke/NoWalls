using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed = 1.0f;
    public float camAngle = 0.0f;
    public GameObject firstCam;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 moveVector = Quaternion.Euler(0.0f, -1.0f * (this.camAngle / Mathf.PI) * 180, 0.0f) * new Vector3(Input.GetAxis("Horizontal") * this.speed, 0.0f, Input.GetAxis("Vertical") * this.speed);
        this.GetComponent<CharacterController>().SimpleMove(moveVector);

        var camTransTwo = this.firstCam.GetComponent<Transform>();
        if (Input.GetKey(KeyCode.Space)){
            this.camAngle += 0.01f;
            camTransTwo.localRotation = Quaternion.Euler(new Vector3(10.0f, -1.0f * (this.camAngle / Mathf.PI) * 180, 0.0f));
        }
    }
}
