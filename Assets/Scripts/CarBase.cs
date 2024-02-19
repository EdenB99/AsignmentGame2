using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Windows;

public class CarBase : MonoBehaviour
{
    /// <summary>
    /// 최대 가속도
    /// </summary>
    public float maxSpeed = 30f;
    /// <summary>
    /// 가속도
    /// </summary>
    public float acceleration = 1f;
    /// <summary>
    /// 현재 속도
    /// </summary>
    public  float currentSpeed = 0f;
    /// <summary>
    /// 이동 방향(1 : 전진, -1 : 후진, 0 : 정지)
    /// </summary>
    private float moveDirection = 0.0f;


    /// <summary>
    /// 회전 속도
    /// </summary>
    public float rotationSpeed = 40f;
    /// <summary>
    /// 회전 가속도
    /// </summary>
    public float rotationAcceleration = 5f;
    /// <summary>
    /// 현재 회전 속도
    /// </summary>
    public float currentRotationSpeed = 40f;
    /// <summary>
    /// 회전방향(1 : 우회전, -1 : 좌회전, 0 : 정지)
    /// </summary>
    private float rotateDirection = 0.0f;
    
    private Rigidbody carRigidBody;
    private WheelCollider[] wheelColliders;
    PlayerInput inputActions;
    private void Awake()
    {
        carRigidBody = GetComponent<Rigidbody>();
        FindWheelColliders();
        inputActions = new();
    }
    private void OnEnable()
    {
        inputActions.Player.Enable();
        inputActions.Player.Move.performed += OnMoveInput;
        inputActions.Player.Acceleration.performed += Acceleration;
        inputActions.Player.Dlft.performed += Dlft;
        inputActions.Player.Dlft.canceled += Dlft;
    }

  

    private void OnDisable()
    {
        inputActions.Player.Dlft.canceled -= Dlft;
        inputActions.Player.Dlft.performed -= Dlft;
        inputActions.Player.Acceleration.performed -= Acceleration;
        inputActions.Player.Move.performed -= OnMoveInput;
        inputActions.Player.Disable();
    }
    void FindWheelColliders()
    {
        // 자식 오브젝트 중에서 Wheel Collider를 가진 자식 오브젝트들 찾기
        wheelColliders = GetComponentsInChildren<WheelCollider>();
    }
    private void Update()
    {
        Move();
        Rotate();
    }
    

    private void Dlft(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (rotateDirection > 0f)
            {
                // 우회전 중인 경우
                RotateCar(45f, 20f);
            }
            else if (rotateDirection < 0f)
            {
                // 좌회전 중인 경우
                RotateCar(-45f, 20f);
            }
        }
        else if (context.canceled)
        {
            // 원래 형태로 복귀
            RotateCar(0f, 5f);
        }
    }
    private void RotateCar(float targetAngle, float targetSpeed)
    {

        while (currentRotationSpeed < targetSpeed)
        {
            // 회전 속도 점진적으로 변경
            rotationSpeed = Mathf.MoveTowards(currentRotationSpeed, targetSpeed, rotationAcceleration * Time.deltaTime);
            currentRotationSpeed = rotationSpeed;

            // 자동차 회전
            float rotation = targetAngle * rotationSpeed * Time.deltaTime;
            Quaternion deltaRotation = Quaternion.Euler(Vector3.up * rotation);
            carRigidBody.MoveRotation(carRigidBody.rotation * deltaRotation);
        }
    }

    private void Acceleration(InputAction.CallbackContext context)
    {
        throw new NotImplementedException();
    }

    private void OnMoveInput(InputAction.CallbackContext context)
    {
        SetInput(context.ReadValue<Vector2>(), !context.canceled);
        
    }

    private void SetInput(Vector2 input, bool v)
    {
        rotateDirection = input.x;
        moveDirection = input.y;
    }
    private void Move()
    {
        if (moveDirection > 0f)
        {
            // 전진 상태일 때
            currentSpeed += moveDirection * acceleration * Time.deltaTime;
            // 최대 속도 제한
            currentSpeed = Mathf.Clamp(currentSpeed, 0f, maxSpeed);

        }
        else if (moveDirection < 0f)
        {
            // 후진 상태일 때
            currentSpeed += moveDirection * acceleration * Time.deltaTime;
            if (currentSpeed < -maxSpeed)
            {
                currentSpeed = -maxSpeed;
            }
        }
        else
        {
            // 정지 상태일 때 천천히 감속
            currentSpeed = Mathf.MoveTowards(currentSpeed, 0f, acceleration/2 * Time.deltaTime);
        }

        
        // 전진 또는 후진
        Vector3 forwardMovement = transform.forward * currentSpeed * Time.deltaTime;
        carRigidBody.MovePosition(carRigidBody.position + forwardMovement);
    }
    void Rotate()
    {
        float minRotationSpeed = 40f; // 최소 회전 속도
        float maxRotationSpeed = 80f; // 최대 회전 속도
        float rotateDirection = inputActions.Player.Move.ReadValue<Vector2>().x;

        // 회전 속도 점진적으로 변경
        float targetRotationSpeed = Mathf.Lerp(minRotationSpeed, maxRotationSpeed, Mathf.Abs(rotateDirection));
        float rotationSpeed = Mathf.MoveTowards(currentRotationSpeed, targetRotationSpeed, rotationAcceleration * Time.deltaTime);
        currentRotationSpeed = rotationSpeed;
        

        float rotation = rotateDirection * rotationSpeed * Time.deltaTime;
        Quaternion deltaRotation = Quaternion.Euler(Vector3.up * rotation);
        carRigidBody.MoveRotation(carRigidBody.rotation * deltaRotation);

        // 각 Wheel Collider의 회전 갱신
        foreach (WheelCollider wheelCollider in wheelColliders)
        {
            wheelCollider.steerAngle = rotateDirection * rotationSpeed;
            wheelCollider.motorTorque = moveDirection * acceleration;
        }


    }
}
