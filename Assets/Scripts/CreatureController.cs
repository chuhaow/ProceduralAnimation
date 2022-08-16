﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatureController : MonoBehaviour
{

    [Header("Movement")]
    [SerializeField] private Vector3 velocity;
    
    [SerializeField] private float speed;
    [SerializeField] private float rotationSpeed;
    private float moveInputFactor = 5f;

    [SerializeField] private float movX;
    [SerializeField] private float movY;

    [Header("Legs")]
    [SerializeField] private ProceduralLegAni[] legs;
    private int index;

    [Header("Mics")]
    private bool dynamicGait = false;
    private float timeBetweenSteps = 0.25f;
    private float lastStep = 0;
    private Rigidbody rb;
    private Vector3 velo;
    // Start is called before the first frame update
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        move();
    }

    private void move()
    {
        velocity = Vector3.MoveTowards(velocity, new Vector3(Input.GetAxis("Vertical"), 0f, Input.GetAxis("Horizontal")).normalized,1f);
        transform.position += velocity * speed * Time.deltaTime;
        velo = velocity;
        if(Time.time > lastStep + (timeBetweenSteps/legs.Length) && legs != null)
        {
            if(legs[index] == null)
            {
                return ;
            }
            legs[index].StepDur = Mathf.Min(0.5f, timeBetweenSteps / 2f);
            legs[index].WorldVelocity = velocity*0.1f;
            legs[index].Step();
            legs[index].LastStep = Time.time;
            index = (index + 1) % legs.Length;
        }
    }
}