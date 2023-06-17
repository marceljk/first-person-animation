using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLook : MonoBehaviour
{
    public float mouseSpeed = 1000f;
    public Transform playerCamera;

    [SerializeField] private float xRotation = 0;

    [SerializeField] Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        float mouseUpDown = Input.GetAxis("Mouse Y") * mouseSpeed * Time.deltaTime;

        AnimatorClipInfo[] animatorClipInfo = animator.GetCurrentAnimatorClipInfo(0);
        if (animatorClipInfo.Length == 1 && animatorClipInfo[0].clip.name == "Idle")
        {
            xRotation -= mouseUpDown;
            xRotation = Mathf.Clamp(xRotation, -25, 25);
        } else
        {
            xRotation = Mathf.Lerp(xRotation, 0f, 3 * Time.deltaTime);
        }
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    }
}
