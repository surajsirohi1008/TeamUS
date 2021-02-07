using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scr_Movement_Controller : MonoBehaviour
{
    private enum State { Driving, Steering, Drifting };
    [SerializeField] private State MyState;

    [SerializeField] private float startVelocity;

    private Rigidbody rb;

    [Header("Debug")]
    public float velocity;
    public float angularFrequency;
    public int driftingDirection;
    public float percentToMaxAngularFrequency;

    private float rotationStartTime;
    private float input;

    [Header("Rotation Settings")]
    [SerializeField] private float radius;
    [SerializeField] [Range(0f, 10f)] private float startAngularfrequency;
    [SerializeField] [Range(0f, 50f)] private float maxAngularFrequency;
    [SerializeField] [Range(0f, 10f)] private float timeToMaxAngularFrequency;
    [SerializeField] [Range(0f, 10f)] private float minAngularFrequencyToDrift;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }
    void Update()
    {

        input = Input.GetAxisRaw("Horizontal");//Get Input

        //Debug
        velocity = rb.velocity.magnitude;


        switch (MyState)
        {
            case State.Driving:
                if (input != 0) { DrivingToSteering(); break; }//Switch to Drifting
                Driving();
                break;
            case State.Steering:
                if (input != driftingDirection) { AnyToDriving(); break; }//Switch to Driving
                if (Mathf.Abs(angularFrequency) >= Mathf.Abs(minAngularFrequencyToDrift)) { MyState = State.Drifting; break; }//Switch to Drifting//TODO///////////////////////////////////////////////////////////////////////////////////////////////////////////
                Rotating();
                break;
            case State.Drifting:
                // if (input != driftingDirection) { AnyToDriving(); break; }//Switch to Driving
                if (input == 0 && !Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.LeftArrow)) { AnyToDriving(); break; }//Switch to Driving
                Rotating();
                break;
        }
    }
    private void Driving()
    {
        rb.velocity = transform.forward * startVelocity;
    }
    private void Rotating()//Steering and Drifting
    {
        float rotationLifeTime = Time.time - rotationStartTime;
        //Handle Drift direction
        if (driftingDirection != (int)input && input != 0)
        { driftingDirection = (int)input; }
        //Handle Rotation
        percentToMaxAngularFrequency = rotationLifeTime / timeToMaxAngularFrequency;
        angularFrequency = Mathf.Lerp(startAngularfrequency, maxAngularFrequency, percentToMaxAngularFrequency) * driftingDirection;

        //Apply Forces
        rb.angularVelocity = transform.up * angularFrequency;
        rb.velocity = transform.forward * Mathf.Abs(angularFrequency) * radius;

    }
    private void DrivingToSteering()//Switch to Drifting 
    {//Set up variables
        driftingDirection = (int)input;
        rotationStartTime = Time.time;
        MyState = State.Steering;
    }
    private void FixedUpdate()
    {

    }
    private void AnyToDriving()//Switch to Driving 
    {//Set up variables
        angularFrequency = 0;
        MyState = State.Driving;
    }
}
