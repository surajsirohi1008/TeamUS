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
    public int driftingDirection;
    [SerializeField] [Range(0f, 1f)] private float percentToMaxDrift;
    public float actualRadius;
    public float actualRotationsPerSecond;
    [SerializeField] private float startRotationsPerSecond;

    private float rotationStartTime;
    private float input;

    [Header("Rotation Settings")]
    [SerializeField] private float radius;
    [SerializeField] [Range(0f, 50f)] private float maxRotationsPerSecond;//translate this into an equation that uses radius to calculate the actial angular frquency in redians per second
    [SerializeField] [Range(0f, 10f)] private float timeToMaxRotationsPerSecond;
    [SerializeField] [Range(0f, 10f)] private float minRotationsPerSecondToDrift;


    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.maxAngularVelocity = 100f;
    }
    void Update()
    {
        input = Input.GetAxisRaw("Horizontal");//Get Input
        //Debug
        velocity = rb.velocity.magnitude;
        actualRotationsPerSecond = rb.angularVelocity.magnitude / (2 * Mathf.PI);


        switch (MyState)
        {
            case State.Driving:
                if (input != 0) { DrivingToSteering(); break; }//Switch to Drifting
                Driving();
                break;
            case State.Steering:
                if (input != driftingDirection) { AnyToDriving(); break; }//Switch to Driving
                if (rb.angularVelocity.magnitude >= Mathf.Abs(minRotationsPerSecondToDrift)) { MyState = State.Drifting; break; }//Switch to Driving
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

        //Lerp angular velocity and velocity values
        percentToMaxDrift = Mathf.Clamp(rotationLifeTime / timeToMaxRotationsPerSecond, 0f, 1f);

        //Calculate rotation values
        float startRotationsPerSecondInRad = startVelocity / radius;
        startRotationsPerSecond = startRotationsPerSecondInRad / (2 * Mathf.PI);//debug
        float maxRotationsPerSecondInRad = maxRotationsPerSecond * 2 * Mathf.PI;

        //Lerp and Set values
        rb.angularVelocity = transform.up * Mathf.Lerp(startRotationsPerSecondInRad, maxRotationsPerSecondInRad, percentToMaxDrift) * driftingDirection;
        rb.velocity = transform.forward * (radius * rb.angularVelocity.magnitude);
        //rb.velocity = transform.forward * Mathf.Lerp(startVelocity, radius * maxRotationsPerSecondInRad, percentToMaxDrift);

        actualRadius = rb.velocity.magnitude / rb.angularVelocity.magnitude;//Debug radius
    }
    private void DrivingToSteering()//Switch to Drifting 
    {//Set up variables
        driftingDirection = (int)input;
        rotationStartTime = Time.time;
        MyState = State.Steering;
    }
    private void AnyToDriving()//Switch to Driving 
    {//Set up variables
        MyState = State.Driving;
    }
}
