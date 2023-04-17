using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using Cursor = UnityEngine.UIElements.Cursor;


public class ThirdPersonCamera : MonoBehaviour
{
    // 기본
    public Transform cameraTransform;
    public Camera myCamera;
    public Transform player;
    public Vector3 camPosOffset = new Vector3(0.4f,0.5f,-2.0f); // 충돌 처리용으로 사용.
    public Vector3 camPivotOffset = new Vector3(0.0f,1.0f,0.0f); // 시선 이동에 사용.
    
    // 에임 관련
    public float camRate = 10.0f; // 카메라 반응속도.
    public float horizontalAimingSpeed = 5.0f; // 수평 회전 속도
    public float verticalAimingSpeed = 5.0f; // 수직 회전 속도
    public float maxVerticalAngle = 60.0f; // 수직 최대 각도
    public float minVerticalAngle = -60.0f; // 수직 최소 각도

    // 반동 관련
    public float recoilAngleBounce; // 사격 반동 값
    public float recoilAngleRecovery = 5.0f; // 사격 반동 회복 값
    private float targetMaxVerticalAngle; // 카메라가 최대 각도로 갔을 때, 반동 때문에 더 올라가는 각도를 제어하기 위함
    
    // 이동 수치
    private float angleH = 0.0f; // 마우스 H이동값
    private float angleV = 0.0f; // 마우스 V이동값
    
    // 충돌 관련
    private Vector3 PlayerToCameraVec; // 플레이어 -> 카메라 벡터
    private Vector3 CameraToPlayerVec; // 카메라 -> 플레이어 벡터
    
    // 보간용
    private Vector3 targetCamPosOffset; // 타겟으로 카메라 이동시 보간용 벡터
    private Vector3 targetCamPivotOffset; // 타겟으로 카메라 회전시 보간용 벡터
    private Vector3 smoothCamPosOffset;
    private Vector3 smoothCamPivotOffset;
    
    // 시야값 관련
    private float defaultFOV; // 기본 시야 값
    private float targetFOV; // 타겟 시야 값

    private void Awake()
    {
        myCamera = GetComponent<Camera>();
        
        //카메라 기본 포지션 세팅
        cameraTransform.position = player.position + Quaternion.identity * camPosOffset + Quaternion.identity * camPivotOffset;
        cameraTransform.rotation = Quaternion.identity;
        
        //카메라와 플레이어간의 상대 벡터, 충돌체크에 사용
        PlayerToCameraVec = cameraTransform.position - player.position;
        
        //기본 세팅
        defaultFOV = myCamera.fieldOfView;
        angleH = player.eulerAngles.y;

        

        ResetTargetOffsets();
        ResetFOV();
        ResetMaxVerticalAngle();
    }
    
    public void ResetFOV()
    {
        targetFOV = defaultFOV;
    }

    public void ResetMaxVerticalAngle()
    {
        targetMaxVerticalAngle = maxVerticalAngle;
    }

    public void BounceVertical(float degree)
    {
        recoilAngleBounce = degree;
    }
    public void ResetTargetOffsets()
    {
        targetCamPosOffset = camPosOffset;
        targetCamPivotOffset = camPivotOffset;
    }

    public void SetTargetOffset(Vector3 newPivotOffset, Vector3 newPosOffset)
    {
        targetCamPosOffset = newPivotOffset;
        targetCamPivotOffset = newPosOffset;
    }

    public void SetFOV(float customFOV)
    {
        targetFOV = customFOV;
    }

    bool CameraToPlayerCollCheck(Vector3 checkPos, float playerHeight)
    {
        // 카메라 -> 플레이어 사이에 걸리는게 있으면 false 없으면 true
        Vector3 target = player.position + (Vector3.up * playerHeight);
        if (Physics.SphereCast(checkPos, 0.2f, target - checkPos, 
                out RaycastHit hit, CameraToPlayerVec.magnitude))
        {
            if (hit.transform != player && !hit.transform.GetComponent<Collider>().isTrigger)
            {
                return false;
            }
        }

        return true;
    }

    bool PlayerToCameraCollCheck(Vector3 checkPos, float playerHeight, float maxDistance)
    {
        Vector3 playerPos = this.player.position + (Vector3.up * playerHeight);
        if (Physics.SphereCast(playerPos, 0.2f, checkPos - playerPos,
                out RaycastHit hit, maxDistance))
        {
            if (hit.transform != player && hit.transform != transform &&
                !hit.transform.GetComponent<Collider>().isTrigger)
            {
                return false;
            }
        }

        return true;
    }

    bool DoubleCollCheck(Vector3 checkPos, float maxDistance)
    {
        float playerHeight = player.GetComponent<CapsuleCollider>().height * 0.75f;
        return CameraToPlayerCollCheck(checkPos, playerHeight) &&
               PlayerToCameraCollCheck(checkPos, playerHeight, maxDistance);
    }

    private void Update()
    {
        // 마우스 이동 값
        angleH += Mathf.Clamp(Input.GetAxis("Mouse X"), -1.0f, 1.0f) * horizontalAimingSpeed;
        angleV += Mathf.Clamp(Input.GetAxis("Mouse Y"), -1.0f, 1.0f) * verticalAimingSpeed;
        
        // 수직 이동 제한
        angleV = Mathf.Clamp(angleV, minVerticalAngle, maxVerticalAngle);
        
        // 수직 카메라 바운스
        angleV = Mathf.LerpAngle(angleV, angleV + recoilAngleBounce, 10.0f * Time.deltaTime);
        
        // 카메라 회전
        Quaternion camYRotation = Quaternion.Euler(0.0f,angleH,0.0f);
        Quaternion aimRotation = Quaternion.Euler(-angleV, angleH,0.0f);
        cameraTransform.rotation = aimRotation;
        
        // set FOV
        myCamera.fieldOfView = Mathf.Lerp(myCamera.fieldOfView, targetFOV, 10.0f * Time.deltaTime);

        Vector3 baseTempPosition = player.position + camYRotation * targetCamPivotOffset;
        Vector3 noCollisionOffset = targetCamPosOffset;
        for (float zOffset = targetCamPosOffset.z; zOffset <= 0.0f; zOffset += 0.5f)
        {
            noCollisionOffset.z = zOffset;
            if (DoubleCollCheck(baseTempPosition + aimRotation * noCollisionOffset, Mathf.Abs(zOffset))
                || zOffset == 0.0f)
            {
                break;
            }
        }

        smoothCamPivotOffset = Vector3.Lerp(smoothCamPivotOffset, targetCamPivotOffset, camRate * Time.deltaTime);
        smoothCamPosOffset = Vector3.Lerp(smoothCamPosOffset, noCollisionOffset, camRate * Time.deltaTime);

        cameraTransform.position = player.position + camYRotation * smoothCamPivotOffset
                                                   + aimRotation * smoothCamPosOffset;
        
        if (recoilAngleBounce > 0.0f)
        {
            recoilAngleBounce -= recoilAngleRecovery * Time.deltaTime;
        }
        else if (recoilAngleBounce < 0.0f)
        {
            recoilAngleBounce += recoilAngleRecovery * Time.deltaTime;
        }

    }
}
