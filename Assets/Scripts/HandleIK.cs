using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandleIK : MonoBehaviour
{
    public Animator animator;
    public Vector3 leftHand;
    public Vector3 rightHand;
    public Vector3 head;
    public Vector3 newPosition;
    public bool IKClimbing = false;

    float weightIK = 0f;
    PlayerMovement playerMovement;
    // Start is called before the first frame update
    void Start()
    {
        playerMovement = gameObject.GetComponentInParent<PlayerMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        if (IKClimbing)
        {
            weightIK = 1f;
        }
        else
        {
            float t = Time.deltaTime / 0.2f;
            if (weightIK > 0 && weightIK - t > 0)
            {
                weightIK -= t;
            } else
            {
                weightIK = 0;
            }
        }
    }

    public void DisableIKClimbing()
    {
        IKClimbing = false;
    }

    public void UpdatePositionClimbing()
    {
        playerMovement.UpdatePositionClimbing(newPosition);
    }
    void OnAnimatorIK()
    {
        animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, weightIK);
        animator.SetIKPosition(AvatarIKGoal.LeftHand, leftHand);
        animator.SetIKPositionWeight(AvatarIKGoal.RightHand, weightIK);
        animator.SetIKPosition(AvatarIKGoal.RightHand, rightHand);
        animator.SetLookAtPosition(head);
        animator.SetLookAtWeight(weightIK);
    }
}
