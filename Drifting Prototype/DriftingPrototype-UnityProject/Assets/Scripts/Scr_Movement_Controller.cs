﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scr_Movement_Controller : MonoBehaviour
{
    private enum State { NoInput, Steering, Drifting };
    [SerializeField] private State MyState;

    [Header("General Settings")]
    [SerializeField] private float drivingVelocity;

    [Header("Rotation Settings")]
    [SerializeField] [Range(0f, 10f)] private float timeToMaxDrift;
    [SerializeField] [Range(0f, 1f)] private float driftPercentTreshold;
    [Space(15)]
    [SerializeField] private float maxRadius;
    [SerializeField] private AnimationCurve radiusCurve;
    //[Space(15)]
    //[SerializeField] [Range(0f, 5f)] private float maxRotationsPerSecond;
    //[SerializeField] private AnimationCurve rotationCurve;
    [Space(15)]
    [SerializeField] [Range(0f, 100f)] private float maxDriftingVelocity;
    [SerializeField] private AnimationCurve dirftingVelocityCurve;


    [Header("Components")]
    [SerializeField] private TrailRenderer trailRenderer;
    [SerializeField] private GameObject body;
    private Rigidbody rb;

    //other parameters
    private float rotationStartTime;
    private float input;
    private Quaternion lastDriftBodyRotation;
    public float momentumVelocity = 0;

    [Header("Debug")]
    public float velocity;
    [SerializeField] [Range(0f, 1f)] private float driftPercent;//used for calculations
    public int driftingDirection;
    public float actualRadius;
    public float actualRotationsPerSecond;
    public float startRotationsPerSecond;
    public float percentTest1, percentTest2;

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
        actualRadius = MyState != State.NoInput ? rb.velocity.magnitude / rb.angularVelocity.magnitude : 0;//Debug radius


        switch (MyState)
        {
            case State.NoInput:
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
    {
        //rb.velocity = transform.forward * drivingVelocity; 
        if (momentumVelocity > 0)
        { momentumVelocity -= 5 * Time.deltaTime; }
        else { momentumVelocity = 0; }

        rb.velocity = transform.forward * momentumVelocity;
        percentTest2 = 1 - rb.velocity.magnitude / maxDriftingVelocity;
        body.transform.localRotation = Quaternion.Lerp(lastDriftBodyRotation, Quaternion.Euler(0, 0, 0), percentTest2);
    }
    private void Rotating()//Steering and Drifting
    {
        //Handle Drift direction
        if (driftingDirection != (int)input && input != 0)
        {
            driftingDirection = (int)input;//change direction
            if (driftPercent >= driftPercentTreshold)
            {
                //float driftPercentSetback = .3f;
                //rotationStartTime = Time.time - timeToMaxDrift * (driftPercent - driftPercentSetback);//Setback drift percent
            }
        }

        //Lerp angular velocity and velocity values
        float rotationLifeTime = Time.time - rotationStartTime;
        driftPercent = Mathf.Clamp(rotationLifeTime / timeToMaxDrift, 0f, 1f);

        //rb.velocity = transform.forward * Mathf.Lerp(drivingVelocity, maxDriftingVelocity, dirftingVelocityCurve.Evaluate(driftPercent));
        rb.velocity = transform.forward * Mathf.Lerp(0, maxDriftingVelocity, dirftingVelocityCurve.Evaluate(driftPercent));
        float angularFrequency = rb.velocity.magnitude / (radiusCurve.Evaluate(driftPercent) * maxRadius);
        rb.angularVelocity = transform.up * angularFrequency * driftingDirection;


        percentTest1 = rb.velocity.magnitude / maxDriftingVelocity;
        body.transform.localRotation = Quaternion.Lerp(Quaternion.Euler(0, 0, 0), Quaternion.Euler(0, 40 * driftingDirection, 0), percentTest1);
        lastDriftBodyRotation = body.transform.localRotation;


    }
    private void DrivingToSteering()//Transition to Steering 
    {//Set up variables
        driftingDirection = (int)input;
        //rotationStartTime = Time.time;
        rotationStartTime = Time.time - timeToMaxDrift * (rb.velocity.magnitude / maxDriftingVelocity);//Setback drift percent
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
        momentumVelocity = rb.velocity.magnitude;
        driftPercent = 0;
        trailRenderer.emitting = false;
        MyState = State.NoInput;
    }
}
