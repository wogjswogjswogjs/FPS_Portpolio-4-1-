using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// 현재 동작, 기본 동작, 오버라이딩 동작, 잠긴 동작, 마우스 이동값,
/// 땅에 서있는지, BaseBehaviour를 상속받은 동작들을 업데이트 시켜준다.
/// </summary>
public class BehaviourController : MonoBehaviour
{
    private List<BaseBehaviour> behaviours; // 동작들
    private List<BaseBehaviour> overrideBehaviours; // 우선시 되는 동작
    private int currentBehaviourCode; // 현재 동작 해시코드
    private int defaultBehaviourCode; // 기본 동작 해시코드
    private int LockedBehaviourCode; // 잠긴 동작 해시코드
    
    // 캐싱.
    public Transform playerCamera;
    private Transform playerTransform;
    private Animator playerAnimator;
    private Rigidbody playerRigidbody;
    private ThirdPersonCamera thirdPersonCameraScript;
    
    // 
    private float h; // horizontal axis
    private float v; // vertical axis
    public float turnSmoothing = 1.0f; // 카메라를 향하도록 움직일때 회전속도
    private bool changedFOV; // 달리기 동작이 카메라 시야각이 변경되었을때 저장됐는지.
    public float runFOV = 80.0f; // 달리기 시야각.
    private Vector3 lastDirection; // 마지막 향했던 방향
    private bool isRun; // 현재 달리는중인가?
    private int horizontalParamHash; // 애니메이터 관련 가로축 값
    private int verticalParamHash; // 애니메이터 관련 세로축 값
    private int groundedParamHash; // 애니메이터 지상에 있는가
    private Vector3 colliderExtents; // 땅과의 충돌체크를 위한 충돌체 영역.

    public float GetHorizontal { get => h; }

    public float GetVertical { get => v; }

    public ThirdPersonCamera GetCameraScript { get => thirdPersonCameraScript; }

    public Rigidbody GetRigidbody { get => playerRigidbody; }

    public Animator GetAnimator { get => playerAnimator; }

    public int GetDefaultBehaviour { get => defaultBehaviourCode; }


    private void Awake()
    {
        behaviours = new List<BaseBehaviour>();
        overrideBehaviours = new List<BaseBehaviour>();

        playerTransform = GetComponent<Transform>();
        playerAnimator = GetComponent<Animator>();
        playerRigidbody = GetComponent<Rigidbody>();
        thirdPersonCameraScript = playerCamera.GetComponent<ThirdPersonCamera>();
        
        horizontalParamHash = Animator.StringToHash(AnimatorKey.Horizontal);
        verticalParamHash = Animator.StringToHash(AnimatorKey.Vertical);
        
        // Grounded?
        groundedParamHash = Animator.StringToHash(AnimatorKey.Grounded);
        colliderExtents = GetComponent<Collider>().bounds.extents;
    }

    public bool IsMoving()
    {
        return Mathf.Abs(h) > Mathf.Epsilon || Mathf.Abs(v) > Mathf.Epsilon;
    }

    public bool IsHorizontalMoving()
    {
        return Mathf.Abs(h) > Mathf.Epsilon;
    }

    public bool CanRun()
    {
        foreach (BaseBehaviour behaviour in behaviours)
        {
            if (!behaviour.AllowRun)
            {
                return false;
            }
        }

        foreach (BaseBehaviour overrideBehaviour in overrideBehaviours)
        {
            if (!overrideBehaviour.AllowRun)
            {
                return false;
            }
        }

        return true;
    }
    
    
    public bool IsRun()
    {
        return isRun && IsMoving() && CanRun();
    }

    public bool IsGrounded()
    {
        Ray ray = new Ray(playerTransform.position + Vector3.up * 2 * colliderExtents.x, Vector3.down);
        return Physics.SphereCast(ray, colliderExtents.x, colliderExtents.x + 0.2f);
    }

    private void Update()
    {
        h = Input.GetAxis("Horizontal");
        v = Input.GetAxis("Vertical");
        isRun = Input.GetButton(ButtonName.Run);
        if (IsRun())
        {
            changedFOV = true;
            thirdPersonCameraScript.SetFOV(runFOV);
        }
        else if(changedFOV)
        {
            thirdPersonCameraScript.ResetFOV();
        }
        
        playerAnimator.SetFloat(horizontalParamHash, h,0.1f, Time.deltaTime);
        playerAnimator.SetFloat(verticalParamHash, v, 0.1f, Time.deltaTime);
        playerAnimator.SetBool(groundedParamHash, IsGrounded());
        
    }
    /// <summary>
    /// RigidBody를 사용할 때, 캐릭터가 틀어지는걸 방지하기위해
    /// y값을 0 으로 Repositioning해주는 함수.
    /// </summary>
    public void Repositioning()
    {
        if (lastDirection != Vector3.zero)
        {
            lastDirection.y = 0.0f;
            Quaternion targetRotation = Quaternion.LookRotation(lastDirection);
            Quaternion newRotation = Quaternion.Slerp(playerRigidbody.rotation, targetRotation
            ,turnSmoothing);
            playerRigidbody.MoveRotation(newRotation);
        }
    }

    private void FixedUpdate()
    {
        bool isAnyBehaviourActive = false;
        if (LockedBehaviourCode > 0 || overrideBehaviours.Count == 0)
        {
            foreach (BaseBehaviour behaviour in behaviours)
            {
                if (behaviour.isActiveAndEnabled && currentBehaviourCode == behaviour.GetBehaviourCode)
                {
                    isAnyBehaviourActive = true;
                    behaviour.BehaviourFixedUpdate();
                }
            }
        }
        else
        {
            foreach (BaseBehaviour behaviour in overrideBehaviours)
            {
                behaviour.BehaviourFixedUpdate();
            }
        }

        if (!isAnyBehaviourActive && overrideBehaviours.Count == 0)
        {
            playerRigidbody.useGravity = true;
            Repositioning();
        }
    }

    private void LateUpdate()
    {
        if (LockedBehaviourCode > 0 || overrideBehaviours.Count == 0)
        {
            foreach (BaseBehaviour behaviour in behaviours)
            {
                if (behaviour.isActiveAndEnabled && currentBehaviourCode == behaviour.GetBehaviourCode)
                {
                    behaviour.BehaviourLateUpdate();
                }
            }
        }
        else
        {
            foreach (BaseBehaviour behaviour in overrideBehaviours)
            {
                behaviour.BehaviourLateUpdate();
            }
        }
    }

    public Vector3 GetLastDirection()
    {
        return lastDirection;
    }

    public void SetLastDirection(Vector3 direction)
    {
        lastDirection = direction;
    }

    public void SubScribeBehaviour(BaseBehaviour behaviour)
    {
        behaviours.Add(behaviour);
    }

    public void RegisterDefaultBehaviour(int behaviourCode)
    {
        defaultBehaviourCode = behaviourCode;
        currentBehaviourCode = behaviourCode;
    }

    public void RegisterBehaviour(int behaviourCode)
    {
        if (currentBehaviourCode == defaultBehaviourCode)
        {
            currentBehaviourCode = behaviourCode;
        }
    }

    public void UnregisterBehaviour(int behaviourCode)
    {
        if (currentBehaviourCode == behaviourCode)
        {
            currentBehaviourCode = defaultBehaviourCode;
        }
    }

    public bool RegisterOverrideBehaviour(BaseBehaviour behaviour)
    {
        if (!overrideBehaviours.Contains(behaviour))
        {
            if (overrideBehaviours.Count == 0)
            {
                foreach (BaseBehaviour behaviour1 in behaviours)
                {
                    if (behaviour1.isActiveAndEnabled && currentBehaviourCode == behaviour1.GetBehaviourCode)
                    {
                        behaviour1.OnOverride();
                        break;
                    }
                }
            }
            overrideBehaviours.Add(behaviour);
            return true;
        }

        return false;
    }

    public bool UnregisterOverrideBehaviour(BaseBehaviour behaviour)
    {
        if (overrideBehaviours.Contains(behaviour))
        {
            overrideBehaviours.Remove(behaviour);
            return true;
        }

        return false;
    }

    public bool isOverriding(BaseBehaviour behaviour = null)
    {
        if (behaviour == null)
        {
            return overrideBehaviours.Count > 0;
        }

        return overrideBehaviours.Contains(behaviour);
    }

    public bool IsCurrentBehaviour(int behaviourCode)
    {
        return this.currentBehaviourCode == behaviourCode;
    }

    public bool GetTempLockStatus(int behaviourCode = 0)
    {
        return LockedBehaviourCode != 0 && LockedBehaviourCode != behaviourCode;
    }

    public void LockBehaviour(int behaviourCode)
    {
        if (LockedBehaviourCode == 0)
        {
            LockedBehaviourCode = behaviourCode;
        }
    }

    public void UnLockBehaviour(int behaviourCode)
    {
        if (LockedBehaviourCode == behaviourCode)
        {
            LockedBehaviourCode = 0;
        }
    }
}











/// <summary>
/// Behaviour의 기반 클래스
/// </summary>
public abstract class BaseBehaviour : MonoBehaviour
{
    protected int speedParamHash;
    protected BehaviourController behaviourController;
    protected int behaviourCode;
    protected bool canRun;

    private void Awake()
    {
        behaviourController = GetComponent<BehaviourController>();
        speedParamHash = Animator.StringToHash(AnimatorKey.Speed);
        canRun = true;
        
        // 동작 타입을 해시코드로 가지고 있다가 추후에 구별용으로 사용.
        behaviourCode = this.GetType().GetHashCode();
    }

    public int GetBehaviourCode
    {
        get => behaviourCode;
    }

    public bool AllowRun
    {
        get => canRun;
    }

    public virtual void BehaviourLateUpdate()
    {
        
    }

    public virtual void BehaviourFixedUpdate()
    {
        
    }

    public virtual void OnOverride()
    {
        
    }
}
