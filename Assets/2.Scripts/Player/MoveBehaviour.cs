using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 이동과 점프 동작을 담당하는 컴포넌트
/// 충돌처리에 대한 기능도 포함
/// 기본 동작으로써 작동
/// </summary>
public class MoveBehaviour : BaseBehaviour
{
    public float walkSpeed = 0.15f;
    public float runSpeed = 1.0f;
    public float sprintSpeed = 2.0f;
    public float speedDampTime = 0.1f;

    public float jumpHeight = 1.5f;
    public float jumpForce = 10f; // 점프 관성
    public float speed, speedSeeker;
    private int jumpParamHash;
    private int groundedParamHash;
    private bool isJump;
    private bool isColliding;
    private CapsuleCollider capsuleCollider;
    private Transform playerTransform;
    
    
    void Start()
    {
        playerTransform = transform;
        capsuleCollider = GetComponent<CapsuleCollider>();
        jumpParamHash = Animator.StringToHash(AnimatorKey.Jump);
        groundedParamHash = Animator.StringToHash(AnimatorKey.Grounded);
        behaviourController.GetAnimator.SetBool(groundedParamHash, true);
        
        behaviourController.SubScribeBehaviour(this);
        behaviourController.RegisterDefaultBehaviour(this.behaviourCode);
        speedSeeker = runSpeed;
    }

    Vector3 Rotating(float horizontal, float vertical)
    {
        Vector3 forward = behaviourController.playerCamera.TransformDirection(Vector3.forward);
        Vector3 right = behaviourController.playerCamera.TransformDirection(Vector3.right);
        forward.y = 0;
        forward = forward.normalized;
        //Vector3 right = new Vector3(forward.z, 0.0f, -forward.x);

        Vector3 targetDirection = Vector3.zero; 
        targetDirection = forward * vertical + right * horizontal;

        if (behaviourController.IsMoving() && targetDirection != Vector3.zero)
        {
            Quaternion targetRot = Quaternion.LookRotation(targetDirection);
            
            Quaternion rot = Quaternion.Slerp(behaviourController.GetRigidbody.rotation,
                targetRot, behaviourController.turnSmoothing);
            behaviourController.GetRigidbody.MoveRotation(rot);
            behaviourController.SetLastDirection(targetDirection);
        }

        if (!(Mathf.Abs(horizontal) > 0.9f || Mathf.Abs(vertical) > 0.9f))
        {
            behaviourController.Repositioning();
        }

        return targetDirection;
    }

    void Moveing(float horizontal, float vertical)
    {
        if (behaviourController.IsGrounded())
        {
            behaviourController.GetRigidbody.useGravity = true;
        }

        Rotating(horizontal, vertical);
        
        if (behaviourController.IsRun())
        {
            speed = sprintSpeed;
        }
        else
        {
            speed = Vector3.ClampMagnitude(new Vector3(horizontal, 0, vertical),1.0f).magnitude;
        }
        behaviourController.GetAnimator.SetFloat(speedParamHash, speed, speedDampTime, Time.deltaTime);
    }

    /*private void OnCollisionStay(Collision collision)
    {
        // 경사면에 있을 경우, 미끄러진다.
        isColliding = true;
        if (behaviourController.IsCurrentBehaviour(GetBehaviourCode)&&
            collision.GetContact(0).normal.y <= 0.1f)
        {
            float vel = behaviourController.GetAnimator.velocity.magnitude;
            Vector3 tangentMove = Vector3.ProjectOnPlane(playerTransform.forward,
                collision.GetContact(0).normal).normalized * vel;
            behaviourController.GetRigidbody.AddForce(tangentMove, ForceMode.VelocityChange);
        }
    }*/

    private void OnCollisionExit(Collision collision)
    {
        isColliding = false;
    }

    void Jumping()
    {
        if (!isJump && !behaviourController.GetAnimator.GetBool(jumpParamHash) &&
            behaviourController.IsGrounded())
        {
            behaviourController.LockBehaviour(behaviourCode);
            behaviourController.GetAnimator.SetBool(jumpParamHash, true);
            if (behaviourController.GetAnimator.GetFloat(speedParamHash) > 0.1f)
            {
                capsuleCollider.material.dynamicFriction = 0f;
                capsuleCollider.material.staticFriction = 0f;
                float velocity = 2.0f * Mathf.Abs(Physics.gravity.y) * jumpHeight;
                velocity = Mathf.Sqrt(velocity);
                behaviourController.GetRigidbody.AddForce(Vector3.up * velocity, ForceMode.VelocityChange);
            }
        }
        else if(behaviourController.GetAnimator.GetBool(jumpParamHash))
        {
            if (!behaviourController.IsGrounded() && !isColliding &&
                behaviourController.GetTempLockStatus())
            {
                behaviourController.GetRigidbody.AddForce(playerTransform.forward * jumpForce *
                                                          Physics.gravity.magnitude * sprintSpeed, ForceMode.Acceleration);
            }

            if (behaviourController.GetRigidbody.velocity.y < 0.0f && behaviourController.IsGrounded())
            {
                behaviourController.GetAnimator.SetBool(groundedParamHash, true);
                capsuleCollider.material.dynamicFriction = 0.6f;
                capsuleCollider.material.staticFriction = 0.6f;
                isJump = false;
                behaviourController.GetAnimator.SetBool(jumpParamHash, false);
                behaviourController.UnLockBehaviour(behaviourCode);
            }
        }
    }


    void Update()
    {
        if (!isJump && Input.GetButtonDown(ButtonName.Jump) &&
            behaviourController.IsCurrentBehaviour(behaviourCode) &&
            !behaviourController.isOverriding())
        {
            isJump = true;
        }
    }

    public override void BehaviourFixedUpdate()
    {
        Moveing(behaviourController.GetHorizontal, behaviourController.GetVertical);
        Jumping();
    }
}
