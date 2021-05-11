using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [Tooltip("Max movement speed")]
    private Animator _animator;
    private Rigidbody _rigidbody;
    private CharacterController _characterController;
    public float moveSpeed = GameConstants.k_playerSpeed;
    public float jumpSpeed = GameConstants.k_jumpSpeed;
    private bool jumpBool = false;
    private float deltaY = GameConstants.k_gravity;

    [Header("Rotation")]
    [Tooltip("Rotation speed for moving the camera")]
    public float rotationSpeed = 5f;
    
    [Header("References")]
    [Tooltip("Reference to the main camera used for the player")]
    public Camera _camera;
    private float _cameraVerticalAngle;
    private float _cameraHorizontalAngle;
    
    private float hAxis;                                      // Horizontal Axis.
    private float vAxis;                                      // Vertical Axis.
    
    public Rigidbody GetRigidBody { get { return _rigidbody; } }
    public Animator GetAnim { get { return _animator; } }
    public CharacterController GetCharacterController { get { return _characterController; } }
    
    public string jumpButton = "Jump";              // Default jump button.
    public float jumpHeight = 1.5f;                 // Default jump height.
    public float jumpIntertialForce = 10f;          // Default horizontal inertial force when jumping.
    // Start is called before the first frame update
    void Start()
    {
        _animator = GetComponent<Animator> ();
        _rigidbody = GetComponent<Rigidbody> ();
        _characterController = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (_characterController.isGrounded && jumpBool)
        {
            jumpBool = false;
            _animator.SetBool("Jump", false);
        }
        
        // get the right moving direction
        float yAngle = transform.rotation.eulerAngles.y;
        Vector3 movement = new Vector3(Input.GetAxisRaw(GameConstants.k_AxisNameHorizontal), 0f, Input.GetAxisRaw(GameConstants.k_AxisNameVertical));
        
        // adjust magnitude when moving slope
        if (movement.z * movement.x != 0f)
        {
            movement *= 0.7f;
        }
        
        // ==== 加速 Press Shift to sprint ====
        if (Input.GetButton(GameConstants.k_ButtonNameSprint) && !jumpBool)
        {
            movement *= 2;
            //_animator.SetBool("Run", true);
        } else {
            //_animator.SetBool("Run", false);
        }
        // ====
        
        // 根据camera调整前进方向
        Vector3 movementRot = Quaternion.AngleAxis(yAngle, Vector3.up) * movement;
        
        
        
        //Add gravity
        if (!_characterController.isGrounded)
        {
            deltaY += GameConstants.k_gravity;
            movementRot.y += deltaY;
        }

        // Jump
        if (Input.GetButton(GameConstants.k_ButtonNameJump) && _characterController.isGrounded)
        {
            print("Jump");
            _animator.SetBool("Jump", true);
            deltaY = jumpSpeed;
            jumpBool = true;
        }


        _characterController.Move(Time.deltaTime * moveSpeed * movementRot);
        
        // Movement animation
        //movement.y = 0;
        _animator.SetFloat("SpeedX", movement.x);
        _animator.SetFloat("SpeedY", movement.z);
        //print(movement);
        

    }

    private void Update()
    {
        // horizontal character rotation
        {
            // 根据Y轴旋转
            transform.Rotate(new Vector3(0f, (Input.GetAxisRaw(GameConstants.k_MouseAxisNameHorizontal) * rotationSpeed), 0f), Space.Self);
        }

        // vertical camera rotation
        {
            // add vertical inputs to the camera's vertical angle
            _cameraVerticalAngle = Input.GetAxisRaw(GameConstants.k_MouseAxisNameVertical) * rotationSpeed * 0.4f;

            // Clamp旋转值
            _cameraVerticalAngle = Mathf.Clamp(_cameraVerticalAngle, -60f, 60f);

            float yAngle = transform.rotation.eulerAngles.y;
            var axis = Quaternion.AngleAxis(yAngle, Vector3.up) * Vector3.right;
            // 以player为origin旋转
            _camera.transform.RotateAround(transform.position + Vector3.up, axis, -_cameraVerticalAngle);
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }

}
