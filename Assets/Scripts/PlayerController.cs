using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerController : MonoBehaviour
{

    public float speed = 7;
    public float smoothMoveTime = .1f;
    public float turnSpeed = 8;
    public static event Action OnGoalReached;

    private float angle;
    private float smoothInputMagnitude;
    private float smoothMoveVelocity;
    private Vector3 velocity;
    private bool isDisabled = false;

    Rigidbody myRigidBody;

    // Start is called before the first frame update
    void Start() {
        myRigidBody = GetComponent<Rigidbody>();
        Guard.OnPlayerSpotted += Disable;
    }

    void Disable() {
        isDisabled = true;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 inputDir = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;
        if (isDisabled) {
            inputDir = Vector3.zero;
        }
        float inputMagnitude = inputDir.magnitude;
        smoothInputMagnitude = Mathf.SmoothDamp(smoothInputMagnitude, inputMagnitude, ref smoothMoveVelocity, smoothMoveTime);

        float targetAngle = Mathf.Atan2(inputDir.x, inputDir.z) * Mathf.Rad2Deg;
        angle = Mathf.LerpAngle(angle, targetAngle, Time.deltaTime * turnSpeed * inputMagnitude);
        velocity = 
        transform.eulerAngles = Vector3.up * angle;
        velocity = transform.forward * speed * smoothInputMagnitude;
    }

    void FixedUpdate() {
        myRigidBody.MoveRotation(Quaternion.Euler(Vector3.up * angle));
        myRigidBody.MovePosition(myRigidBody.position + velocity * Time.fixedDeltaTime);
    }

    void OnDestroy() {
        Guard.OnPlayerSpotted -= Disable;
    }

    void OnTriggerEnter(Collider other) {
        if (other.tag == "Goal") {
            if (OnGoalReached != null) {
                OnGoalReached();
                isDisabled = true;
            }
        }
    }
}
