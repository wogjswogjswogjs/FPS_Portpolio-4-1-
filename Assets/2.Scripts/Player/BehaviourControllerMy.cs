using System;
using System.Collections;
using System.Collections.Generic;
using System.Security;
using UnityEngine;

public class BehaviourControllerMy : MonoBehaviour
{
    private List<BaseBehaviour> behaviours; // 동작들
    private int currentBehaviourCode;
    private int defaultBehaviourCode;
    private int LockedBehaviourCode;
    
    // 캐싱
    public Transform playerCamera;
    private Transform playerTransform;
    private Animator playerAnimator;
    private Rigidbody playerRigidbody;
    private ThirdPersonCamera thirdPersonCamera;

    private float horizontalInput; // horizontal axis
    private float verticalInput; // vertical axis
    public float cameraTurnSmoothing = 1.0f; // 카메라를 향하도록 움직일때 회전속도
    public float runFOV = 80.0f; // 달리기 시야각.
    
    // 프로퍼티 
    public float GetHorizontalInput { get => horizontalInput; }
    public float GetVerticalInput { get => verticalInput; }
    public ThirdPersonCamera GetCameraScript { get => thirdPersonCamera; }
    public Rigidbody GetRigidbody { get => playerRigidbody; }
    public Animator GetAnimator { get => playerAnimator; }

    private bool isRun; // 현재 달리는중인가?
    private void Awake()
    {
        behaviours = new List<BaseBehaviour>();
        
        playerTransform = GetComponent<Transform>();
        playerAnimator = GetComponent<Animator>();
        playerRigidbody = GetComponent<Rigidbody>();
        thirdPersonCamera = playerCamera.GetComponent<ThirdPersonCamera>();
    }
    
    public bool IsRun()
    {
        return isRun && IsMoving() && CanRun();
    }
    public bool IsMoving()
    {
        return Mathf.Abs(horizontalInput) > Mathf.Epsilon || Mathf.Abs(verticalInput) > Mathf.Epsilon;
    }
    
    public bool IsHorizontalMoving()
    {
        return Mathf.Abs(horizontalInput) > Mathf.Epsilon;
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
        return true;
    }

    private void Update()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");
        isRun = Input.GetButton(ButtonName.Run);

        if (IsRun())
        {
            thirdPersonCamera.SetFOV(runFOV);
        }
        else
        {
            thirdPersonCamera.ResetFOV();
        }
    }

    private void FixedUpdate()
    {
        foreach (BaseBehaviour behaviour in behaviours)
        {
            if (behaviour.isActiveAndEnabled && currentBehaviourCode == behaviour.GetBehaviourCode)
            {
                behaviour.BehaviourFixedUpdate();
            }
        }
    }

    private void LateUpdate()
    {
        foreach (BaseBehaviour behaviour in behaviours)
        {
            if (behaviour.isActiveAndEnabled && currentBehaviourCode == behaviour.GetBehaviourCode)
            {
                behaviour.BehaviourLateUpdate();
            }
        }
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
    
    public void RegisterLockBehaviour(int behaviourCode)
    {
        if (LockedBehaviourCode == 0)
        {
            LockedBehaviourCode = behaviourCode;
        }
    }
    
    public void UnRegisterLockBehaviour(int behaviourCode)
    {
        if (LockedBehaviourCode == behaviourCode)
        {
            LockedBehaviourCode = 0;
        }
    }
    
    public bool IsCurrentBehaviour(int behaviourCode)
    {
        return this.currentBehaviourCode == behaviourCode;
    }

    public abstract class BaseBehaviour : MonoBehaviour
    {
        protected BehaviourController behaviourController;
        protected int behaviourCode;
        protected bool canSprint;
        public int GetBehaviourCode { get => behaviourCode; }
        public bool AllowRun { get => canSprint; }
        private void Awake()
        {
            behaviourController = GetComponent<BehaviourController>();
            behaviourCode = this.GetType().GetHashCode();
        }
        
        public virtual void BehaviourLateUpdate()
        {
        
        }

        public virtual void BehaviourFixedUpdate()
        {
        
        }
        
    }
}
