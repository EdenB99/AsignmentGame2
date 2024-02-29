using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Windows;
using static UnityEngine.InputSystem.InputAction;

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
    /// ���, ����ġ�� �߰��Ǵ� Ƚ��ī��Ʈ
    /// </summary>
    public int outOfLine = 0;

    /// <summary>
    /// ȸ�� �ӵ�
    /// </summary>
    public float rotationSpeed = 20f;
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

    //���� ����Ʈ ��ġ
    public Transform stpoint;
    public Vector3 startpoint;
    private void Awake()
    {
        carRigidBody = GetComponent<Rigidbody>();
        FindWheelColliders();
        inputActions = new();
    }
    private void Start()
    {
        startpoint = stpoint.position;
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
        inputActions.Player.Acceleration.canceled+= Acceleration;
        inputActions.Player.Dlft.performed += Dlft;
        inputActions.Player.Dlft.canceled += Dlft;
        inputActions.Player.Reset.performed += Reset;
    }

    private void Reset(CallbackContext context)
    {
        gameObject.transform.position = startpoint;
        outOfLine++;
    }

    private void OnDisable()
    {
          EndinputData();   
    }
    public void EndinputData()
    {
        inputActions.Player.Reset.performed -= Reset;
        inputActions.Player.Dlft.canceled -= Dlft;
        inputActions.Player.Dlft.performed -= Dlft;
        inputActions.Player.Acceleration.canceled -= Acceleration;
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
        RePos();
    }

    private void RePos()
    {
        if (transform.position.y > 12f)
        {
            transform.position -= new Vector3(0, 3, 0);
            currentSpeed = 10f;
            transform.rotation = Quaternion.Euler(0, 0, 0);
            outOfLine++;
        }
        else if (transform.position.y < -12f)
        {
            transform.position -= new Vector3(0, 3, 0);
            currentSpeed = 10f;
            transform.rotation = Quaternion.Euler(0, 0, 0);
            outOfLine++;

        } else if (transform.position.y < -300f)
        {
            gameObject.transform.Translate(startpoint, Space.World);
            currentSpeed = 10f;
            transform.rotation = Quaternion.Euler(0, 0, 0);
            outOfLine++;
        }
            
        if (transform.rotation.z >= 180f)
        {
            transform.position += new Vector3(0, 3, 0);
            currentSpeed = 10f;
            transform.rotation = Quaternion.Euler(0, 0, 0);
            outOfLine++;
        }
        
    }
    private float beforerotationAccel = 0f;
    bool isDlft = false;

    private void Dlft(InputAction.CallbackContext context)
    {
        
        if (context.performed) 
        {
            isDlft = true;
            Debug.Log("dlftOn");
            
        } else if (context.canceled)
        {
            isDlft = false;
            Debug.Log("dlftOff");
            rotationAcceleration = beforerotationAccel;
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
        if (context.performed)
        {
            if (!isDlft)
            {
                currentSpeed += (moveDirection  * acceleration)+30*Time.deltaTime;
            }
        } else if (context.canceled)
        {

        }
        
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
        {   if (!isDlft)
            {
                // ���� ������ ��
                currentSpeed += moveDirection * acceleration * Time.deltaTime;
                // �ִ� �ӵ� ����
                currentSpeed = Mathf.Clamp(currentSpeed, 0f, maxSpeed);
            } else
            {
                currentSpeed -= acceleration * 10 * Time.deltaTime;
                beforerotationAccel = rotationAcceleration;
                rotationAcceleration = 10f;
            }
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
        
      if (currentSpeed >= 100f)
        {
            acceleration = 7f;
            rotationChange(1f);
        } else if (currentSpeed >= 70f)
        {
            acceleration = 5f;
            rotationChange(1.5f);
        } else if (currentSpeed >= 50f)
        {
            acceleration = 3f;
            rotationChange(2f);
        } else if (currentSpeed >= 30f)
        {
            acceleration = 2f;
            rotationChange(2.5f);
        } else
        {
            acceleration = 1f;
            rotationChange(3f);
        }
    }
    void rotationChange(float rotaionAccel)
    {
        if (!isDlft)
        {
            rotationAcceleration = rotaionAccel;
        }
    }


    void Rotate()
    {
        float minRotationSpeed = 20f; // �ּ� ȸ�� �ӵ�
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
