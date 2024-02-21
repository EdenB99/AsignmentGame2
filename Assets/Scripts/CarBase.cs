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
    public float maxSpeed = 300f;
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
       
    }
    public void startMove()
    {
        carRigidBody.AddForce(new Vector3());
    }
    public void StartinputData()
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
        Gear();
        posY();
    }

    private void posY()
    {
       if (transform.position.y > 12f) 
        {
            transform.position -= new Vector3(0, 3, 0);
            currentSpeed = 10f;
            transform.rotation = Quaternion.Euler(0, 0, 0);
        } else if (transform.position.y < -12f)
        {
            transform.position -= new Vector3(0, 3, 0);
            currentSpeed = 10f;
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }
        if (transform.rotation.z > 100f)
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);
        } else if (transform.rotation.z < -100f)
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }
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
            if (currentSpeed >= 0f)
            {
                currentSpeed += moveDirection * acceleration*2 * Time.deltaTime;
            }else
            {
                currentSpeed += moveDirection * acceleration/2 * Time.deltaTime;
            }
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
    void Gear()
    {
        
      if (currentSpeed >= 150f)
        {
            acceleration = 7f;
            rotationAcceleration = 1f;
        } else if (currentSpeed >= 100f)
        {
            acceleration = 5f;
            rotationAcceleration = 2f;
        } else if (currentSpeed >= 70f)
        {
            acceleration = 3f;
            rotationAcceleration = 3f;
        } else if (currentSpeed >= 50f)
        {
            acceleration = 2f;
            rotationAcceleration = 4f;
        } else
        {
            acceleration = 1f;
            rotationAcceleration = 5f;
        }
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
