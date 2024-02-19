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
    /// �ִ� ���ӵ�
    /// </summary>
    public float maxSpeed = 30f;
    /// <summary>
    /// ���ӵ�
    /// </summary>
    public float acceleration = 1f;
    /// <summary>
    /// ���� �ӵ�
    /// </summary>
    public  float currentSpeed = 0f;
    /// <summary>
    /// �̵� ����(1 : ����, -1 : ����, 0 : ����)
    /// </summary>
    private float moveDirection = 0.0f;


    /// <summary>
    /// ȸ�� �ӵ�
    /// </summary>
    public float rotationSpeed = 40f;
    /// <summary>
    /// ȸ�� ���ӵ�
    /// </summary>
    public float rotationAcceleration = 5f;
    /// <summary>
    /// ���� ȸ�� �ӵ�
    /// </summary>
    public float currentRotationSpeed = 40f;
    /// <summary>
    /// ȸ������(1 : ��ȸ��, -1 : ��ȸ��, 0 : ����)
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
        // �ڽ� ������Ʈ �߿��� Wheel Collider�� ���� �ڽ� ������Ʈ�� ã��
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
                // ��ȸ�� ���� ���
                RotateCar(45f, 20f);
            }
            else if (rotateDirection < 0f)
            {
                // ��ȸ�� ���� ���
                RotateCar(-45f, 20f);
            }
        }
        else if (context.canceled)
        {
            // ���� ���·� ����
            RotateCar(0f, 5f);
        }
    }
    private void RotateCar(float targetAngle, float targetSpeed)
    {

        while (currentRotationSpeed < targetSpeed)
        {
            // ȸ�� �ӵ� ���������� ����
            rotationSpeed = Mathf.MoveTowards(currentRotationSpeed, targetSpeed, rotationAcceleration * Time.deltaTime);
            currentRotationSpeed = rotationSpeed;

            // �ڵ��� ȸ��
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
            // ���� ������ ��
            currentSpeed += moveDirection * acceleration * Time.deltaTime;
            // �ִ� �ӵ� ����
            currentSpeed = Mathf.Clamp(currentSpeed, 0f, maxSpeed);

        }
        else if (moveDirection < 0f)
        {
            // ���� ������ ��
            currentSpeed += moveDirection * acceleration * Time.deltaTime;
            if (currentSpeed < -maxSpeed)
            {
                currentSpeed = -maxSpeed;
            }
        }
        else
        {
            // ���� ������ �� õõ�� ����
            currentSpeed = Mathf.MoveTowards(currentSpeed, 0f, acceleration/2 * Time.deltaTime);
        }

        
        // ���� �Ǵ� ����
        Vector3 forwardMovement = transform.forward * currentSpeed * Time.deltaTime;
        carRigidBody.MovePosition(carRigidBody.position + forwardMovement);
    }
    void Rotate()
    {
        float minRotationSpeed = 40f; // �ּ� ȸ�� �ӵ�
        float maxRotationSpeed = 80f; // �ִ� ȸ�� �ӵ�
        float rotateDirection = inputActions.Player.Move.ReadValue<Vector2>().x;

        // ȸ�� �ӵ� ���������� ����
        float targetRotationSpeed = Mathf.Lerp(minRotationSpeed, maxRotationSpeed, Mathf.Abs(rotateDirection));
        float rotationSpeed = Mathf.MoveTowards(currentRotationSpeed, targetRotationSpeed, rotationAcceleration * Time.deltaTime);
        currentRotationSpeed = rotationSpeed;
        

        float rotation = rotateDirection * rotationSpeed * Time.deltaTime;
        Quaternion deltaRotation = Quaternion.Euler(Vector3.up * rotation);
        carRigidBody.MoveRotation(carRigidBody.rotation * deltaRotation);

        // �� Wheel Collider�� ȸ�� ����
        foreach (WheelCollider wheelCollider in wheelColliders)
        {
            wheelCollider.steerAngle = rotateDirection * rotationSpeed;
            wheelCollider.motorTorque = moveDirection * acceleration;
        }


    }
}
