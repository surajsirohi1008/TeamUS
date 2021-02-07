using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scr_Movement_Controller : MonoBehaviour
{
    private enum State { Driving, Steering, Drifting };
    [SerializeField] private State MyState;

    [Header("General Settings")]
    [SerializeField] private float drivingVelocity;

    [Header("Rotation Settings")]
    [SerializeField] [Range(0f, 10f)] private float timeToMaxDrift;
    [SerializeField] [Range(0f, 1f)] private float driftPercentTreshold;
    [Space(15)]
    [SerializeField] private float maxRadius;
    [SerializeField] private AnimationCurve radiusCurve;
    [Space(15)]
    [SerializeField] [Range(0f, 5f)] private float maxRotationsPerSecond;
    [SerializeField] private AnimationCurve rotationCurve;
    
    [Header("Components")]
    [SerializeField] private TrailRenderer trailRenderer;
    private Rigidbody rb;

    //other parameters
    private float rotationStartTime;
    private float input;

    [Header("Debug")]
    public float velocity;
    [SerializeField] [Range(0f, 1f)] private float driftPercent;//used for calculations
    public int driftingDirection;
    public float actualRadius;
    public float actualRotationsPerSecond;
    public float startRotationsPerSecond;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        trailRenderer.emitting = false;
        rb.maxAngularVelocity = 100f;
        driftPercent = 0;

    }
    void Update()
    {
        input = Input.GetAxisRaw("Horizontal");//Get Input
        //Debug
        velocity = rb.velocity.magnitude;//Debug velocity
        actualRotationsPerSecond = rb.angularVelocity.magnitude / (2 * Mathf.PI);//Debug rotations per second
        actualRadius = MyState != State.Driving ? rb.velocity.magnitude / rb.angularVelocity.magnitude : 0;//Debug radius


        switch (MyState)
        {
            case State.Driving:
                if (input != 0) { DrivingToSteering(); break; }//Switch to Drifting
                Driving();
                break;
            case State.Steering:
                if (input == 0 && !Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.LeftArrow)) { AnyToDriving(); break; }//Switch to Driving
                if (driftPercent >= driftPercentTreshold) { SteeringToDrifting(); break; }//Switch to Driving
                Rotating();
                break;
            case State.Drifting:
                if (input == 0 && !Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.LeftArrow)) { AnyToDriving(); break; }//Switch to Driving
                if (driftPercent <= driftPercentTreshold) { DriftingToSteering(); break; }
                Rotating();
                break;
        }
    }
    private void Driving()
    { rb.velocity = transform.forward * drivingVelocity; }
    private void Rotating()//Steering and Drifting
    {
        //Handle Drift direction
        if (driftingDirection != (int)input && input != 0)
        {
            driftingDirection = (int)input;//change direction
            if (driftPercent >= driftPercentTreshold)
            {
                float driftPercentSetback = .3f;
                rotationStartTime = Time.time - timeToMaxDrift * (driftPercent - driftPercentSetback);//Setback drift percent
            }
        }

        //Lerp angular velocity and velocity values
        float rotationLifeTime = Time.time - rotationStartTime;
        driftPercent = Mathf.Clamp(rotationLifeTime / timeToMaxDrift, 0f, 1f);

        //Calculate rotation values
        float startRotationsPerSecondInRad = drivingVelocity / maxRadius * radiusCurve.Evaluate(0);
        startRotationsPerSecond = startRotationsPerSecondInRad / (2 * Mathf.PI);//debug
        float maxRotationsPerSecondInRad = maxRotationsPerSecond* rotationCurve.Evaluate(1) * 2 * Mathf.PI;

        //Lerp and set values
        // rb.angularVelocity = transform.up * Mathf.Lerp(startRotationsPerSecondInRad, maxRotationsPerSecondInRad, driftPercent) * driftingDirection;
        rb.angularVelocity = transform.up * Mathf.Lerp(startRotationsPerSecondInRad, maxRotationsPerSecondInRad, rotationCurve.Evaluate(driftPercent)) * driftingDirection;
        rb.velocity = transform.forward * (maxRadius * radiusCurve.Evaluate(driftPercent) * rb.angularVelocity.magnitude);//set velocity to keep desired turn radius 
    }
    private void DrivingToSteering()//Transition to Steering 
    {//Set up variables
        driftingDirection = (int)input;
        rotationStartTime = Time.time;
        MyState = State.Steering;
    }
    private void DriftingToSteering()//Transition to Steering 
    {//Set up variables
        trailRenderer.emitting = false;
        MyState = State.Steering;
    }
    private void SteeringToDrifting()//Transition to Drifting 
    {//Set up variables
        trailRenderer.emitting = true;
        MyState = State.Drifting;
    }
    private void AnyToDriving()//Transition to Driving 
    {//Set up variables
        driftPercent = 0;
        trailRenderer.emitting = false;
        MyState = State.Driving;
    }
}
