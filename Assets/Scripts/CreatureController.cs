using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatureController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float speed;
    [SerializeField] private float rotationSpeed;
    private float moveInputFactor = 5f;
    [SerializeField] private Vector3 velocity;
    [SerializeField] private float movX;
    [SerializeField] private float movY;
    private Rigidbody rb;
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
        velocity = Vector3.MoveTowards(velocity, new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical")).normalized,1f);
        transform.position += velocity * speed * Time.deltaTime;
    }
}
