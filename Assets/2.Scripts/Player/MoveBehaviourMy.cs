using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveBehaviourMy : BaseBehaviour
{
    public float walkSpeed = 1.0f;
    public float runSpeed = 2.0f;
    public float speedDampTime = 0.1f;
    private float speed;

    public float jumpHeight = 1.5f;
    public float jumpForce = 10.0f;
    
    private int jumpParanHash;
    private int groundedParamHash;
    private int horizontalParamHash; // 애니메이터 관련 가로축 값
    private int verticalParamHash; // 애니메이터 관련 세로축 값
    private bool isJump;
    private Vector3 colliderExtents;
    
    private CapsuleCollider capsuleCollider;
    private Transform playerTransform;
    
    void Start()
    {
        playerTransform = transform;
        colliderExtents = GetComponent<Collider>().bounds.extents;
        capsuleCollider = GetComponent<CapsuleCollider>();
        jumpParanHash = Animator.StringToHash(AnimatorKey.Jump);
        groundedParamHash = Animator.StringToHash(AnimatorKey.Grounded);
        behaviourController.GetAnimator.SetBool(groundedParamHash, true);
        
        behaviourController.SubScribeBehaviour(this);
        behaviourController.RegisterDefaultBehaviour(this.behaviourCode);
    }
    
    public bool IsGrounded()
    {
        Ray ray = new Ray(playerTransform.position + Vector3.up * 2 * colliderExtents.x, Vector3.down);
        return Physics.SphereCast(ray, colliderExtents.x, colliderExtents.x + 0.2f);
    }
    
    Vector3 Rotating(float horizontal, float vertical)
    {
        Vector3 cameraForward = behaviourController.playerCamera.TransformDirection(Vector3.forward);
        Vector3 cameraRight = behaviourController.playerCamera.TransformDirection(Vector3.right);
        cameraForward.y = 0;

        Vector3 targetDirection = cameraForward * vertical + cameraRight * horizontal;
        if (targetDirection != Vector3.zero)
        {
            Quaternion targetRot = Quaternion.LookRotation(targetDirection);
            Quaternion rot = Quaternion.Slerp(behaviourController.GetRigidbody.rotation,
                targetRot, behaviourController.turnSmoothing);
            behaviourController.GetRigidbody.MoveRotation(rot);
        }

        return targetDirection;
    }

    void Moving(float horizontal, float vertical)
    {
        Rotating(horizontal, vertical);

        if (behaviourController.IsRun())
        {
            speed = runSpeed;
        }
        else
        {
            speed = Vector3.ClampMagnitude(new Vector3(horizontal, 0, vertical), 1.0f).magnitude;
        }
        behaviourController.GetAnimator.SetFloat(speedParamHash, speed, speedDampTime, Time.deltaTime);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
