using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    // 주석처리한건 리지드바디없이 한 코드
    // 현재는 리지드바디 달았습니다.
    [SerializeField] private StoryController storyController;
    [SerializeField] private Camera playerCamera;
    [SerializeField] private AudioClip walkSFX;

    [Header("Movement")]
    private float moveSpeed = 4.0f;
    private Vector2 curMovementInput;

    //[Header("Gravity")]
    //private float gravity = -8.0f;
    //private Vector3 velocity;
    private bool isGrounded;

    [Header("Look")]
    private float minXLook = -50.0f;
    private float maxXLook = 70.0f;
    private float camCurXRot;
    private float lookSensitivity = 0.2f;
    private Vector2 mouseDelta;

    [Header("Interaction")]
    [SerializeField] private LayerMask otherLayerMask;
    [SerializeField] private LayerMask interactionLayerMask;
    private float maxInteractionDistance = 3f;
    private GameObject currentInteractable;

    [Header("Jump")]
    private float jumpForce = 5.0f;

    private Rigidbody rb;

    private float footstepThreshold = 0.3f;
    private float footstepRate = 0.5f;
    private float footStepTime;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }
    private void FixedUpdate()
    {
        Move();
        //Gravity();
        CheckGrounded();
        CameraLook();

        if (rb.velocity.magnitude > footstepThreshold)
        {
            if (Time.time - footStepTime > footstepRate)
            {
                footStepTime = Time.time;
                GameManager.Instance.PlaySFX(walkSFX);
            }
        }
    }


    public void OnMoveInput(InputAction.CallbackContext context)
    {
        if(context.phase == InputActionPhase.Performed && !GameManager.Instance.IsOnDialogue && GameManager.Instance.IsOnAnime == false)
        {
            curMovementInput = context.ReadValue<Vector2>();
        }
        else if(context.phase == InputActionPhase.Canceled)
        {
            curMovementInput = Vector2.zero;
        }
    }

    private void Move()
    {
        Vector3 dir = (transform.forward * curMovementInput.y + transform.right * curMovementInput.x).normalized;
        //dir *= moveSpeed * Time.deltaTime;
        //transform.position += dir;
        rb.velocity = new Vector3(dir.x * moveSpeed, rb.velocity.y, dir.z * moveSpeed);
    }

    // 중력 계산
    //private void Gravity()
    //{
    //    if(!isGrounded)
    //    {
    //        // 바닥에 닿아 있지 않을 때 중력 적용
    //        velocity.y += gravity * Time.deltaTime;
    //    }
    //    // 속도를 위치에 적용
    //    transform.position += velocity * Time.deltaTime;
    //}

    private void CheckGrounded()
    {
        isGrounded = Physics.Raycast(transform.position, Vector3.down, 1.1f);
        //if(isGrounded && velocity.y < 0)
        //{
        //    velocity.y = 0;
        //}
    }

    public void OnLookInput(InputAction.CallbackContext context)
    {
        if (!GameManager.Instance.IsOnDialogue)
        {
            // 마우스 이동량
            mouseDelta = context.ReadValue<Vector2>();
        }
    }

    void CameraLook()
    {
        // 상하 회전
        camCurXRot += mouseDelta.y * lookSensitivity;
        camCurXRot = Mathf.Clamp(camCurXRot, minXLook, maxXLook);
        playerCamera.transform.localEulerAngles = new Vector3(-camCurXRot, 0, 0);

        // 좌우 회전
        transform.eulerAngles += new Vector3(0, mouseDelta.x * lookSensitivity, 0);
    }

    private void CheckInteraction()
    {
        Ray ray = playerCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, maxInteractionDistance))
        {
            GameObject hitObject = hit.collider.gameObject;

            if (hit.collider.gameObject.layer == 12)
            {
                GameManager.Instance.Player.OnSave?.Invoke();
                Destroy(hit.collider.gameObject);
            }

            if (hit.collider.gameObject.layer != interactionLayerMask)
            {
                currentInteractable = null;
            }
            if (hit.collider.gameObject.layer == 10)
            {
                if (hitObject != currentInteractable)
                {
                    currentInteractable = hitObject;
                    storyController.CollectKnife();
                }
            }
        }
    }

    public void OnInteractInput(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            CheckInteraction();
        }
    }

    public void OnJumpInput(InputAction.CallbackContext context)
    {
        if(isGrounded && context.phase == InputActionPhase.Started && !GameManager.Instance.IsOnDialogue && GameManager.Instance.IsOnAnime == false)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }   
    
    // 스페이스바 눌러서 넘기기
    public void OnSpacebarClick(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started && GameManager.Instance.IsOnDialogue && !GameManager.Instance.IsOnAnime)
        {
            DialogueManager.Instance.OnClickNextBtn();
        }
    }
}
