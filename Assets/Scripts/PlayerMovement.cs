using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerMovement : MonoBehaviour
{
    public CharacterController controller;
    public GameObject character;
    public float speed = 12f;
    public float gravity = -9.81f;
    public float jumpHeight = 1.5f;

    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    Vector3 velocity;
    bool isGrounded, rollAnimation = false;
    float climbingHeight = 1.8f;
    HandleIK handleAnimatorEvents;

    [SerializeField] Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        handleAnimatorEvents = character.GetComponent<HandleIK>();
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
        float sprint = Input.GetKey(KeyCode.LeftShift) ? 1f : 0.5f;
        animator.SetFloat("Walk", move.magnitude * sprint);
        transform.Rotate(Vector3.up, x * speed * 10 * Time.deltaTime);


        controller.Move(speed * sprint * Time.deltaTime * move);
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        if (Input.GetButtonDown("Jump") && move.magnitude > 0)
        {
            if (Climbing())
            {
                animator.SetTrigger("Climb");
            }
            else
            {
                velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
                animator.SetTrigger("RunningJump");
            }

        }
        else if (Input.GetButtonDown("Jump") && isGrounded)
        {
            if (Climbing())
            {
                animator.SetTrigger("Climb");
            }
            else
            {
                animator.SetTrigger("Jump");
            }

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

        if (Input.GetKeyDown(KeyCode.R))
        {
            rollAnimation = true;
        }
    }

    bool Climbing()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, 0.7f))
        {
            Bounds blockBounds = hit.collider.gameObject.GetComponent<Renderer>().bounds;
            float highestPoint = blockBounds.center.y + (blockBounds.size.y / 2f);
            float distanceToGround = highestPoint - groundCheck.position.y;

            if (distanceToGround > climbingHeight && distanceToGround < 2.5f)
            {
                Vector3 edgeOfCube = new Vector3(hit.point.x, highestPoint, hit.point.z);
                Vector3 hitNormal = hit.normal.normalized;

                Vector3 offsetDirection = Quaternion.Euler(0f, 90f, 0f) * hitNormal; // Compute the offset direction

                handleAnimatorEvents.leftHand = edgeOfCube + offsetDirection * 0.25f;
                handleAnimatorEvents.rightHand = edgeOfCube - offsetDirection * 0.25f;
                handleAnimatorEvents.head = edgeOfCube;

                Vector3 playerPos = transform.position;
                float spawnDistance = 0.7f;
                Vector3 spawnPos = playerPos + new Vector3(0, distanceToGround, 0) + transform.forward * spawnDistance;
                handleAnimatorEvents.newPosition = spawnPos;
                handleAnimatorEvents.IKClimbing = true;
                controller.enabled = false;

                Quaternion targetRotation = Quaternion.LookRotation(-hitNormal, Vector3.up);
                transform.rotation = targetRotation;
                transform.position = new Vector3(transform.position.x, transform.position.y + (distanceToGround - 2), transform.position.z);
                return true;
            }
        }
        return false;
    }

    public void UpdatePositionClimbing(Vector3 newPosition)
    {
        transform.position = newPosition;
        controller.enabled = true;
    }
}