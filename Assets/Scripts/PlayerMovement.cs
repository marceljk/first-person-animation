using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerMovement : MonoBehaviour
{
    public CharacterController controller;
    public float speed = 12f;
    public float gravity = -9.81f;
    public float jumpHeight = 2f;

    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    Vector3 velocity;
    bool isGrounded, rollAnimation = false;

    [SerializeField] Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
            if (rollAnimation)
            {
                rollAnimation = false;
                animator.SetTrigger("Roll");
            }
        }

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.forward * z;
        animator.SetFloat("Walk", move.magnitude);
        transform.Rotate(Vector3.up, x * speed * 10 * Time.deltaTime);


        controller.Move(speed * Time.deltaTime * move);
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        if (Input.GetButtonDown("Jump") && move.magnitude > 0)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            animator.SetTrigger("RunningJump");

        } else if (Input.GetButtonDown("Jump") && isGrounded)
        {
            animator.SetTrigger("Jump");
        }

        if (Input.GetKeyDown(KeyCode.LeftControl) && move.magnitude > 0)
        {
            animator.SetTrigger("Slide");
        }

        Ray ray = new(groundCheck.position, Vector3.down);
        if (!Physics.Raycast(ray, out RaycastHit raycastHit, 2.5f) && !isGrounded)
        {
            rollAnimation = true;
        }

        if(Input.GetKeyDown(KeyCode.R))
        {
            rollAnimation = true;
        }
    }
}