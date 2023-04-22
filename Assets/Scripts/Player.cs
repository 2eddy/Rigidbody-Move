using UnityEngine;

public class Player : MonoBehaviour
{
    public GameObject Bullet;
    public Transform Muzzle;

    public float AimHeight = 1.45f;
    public float MoveSpeed = 4.5f;
    public float AirSpeed = 2.5f;
    public float JumpForce = 50f;

    Animator _animator;
    Rigidbody _rigidbody;
    Camera _mainCamera;

    // Animator
    int _velocityXId;
    int _velocityZId;

    int _groundedId;
    int _initiateJumpId;
    int _jumpLoopId;

    Plane _plane;
    Vector3 _moveInput;
    Vector3 _currentLocal;
    Vector3 _lookDirection;

    bool _grounded;
    int _groundMask;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _rigidbody = GetComponent<Rigidbody>();
        _mainCamera = Camera.main;

        _plane = new Plane(Vector3.up, -AimHeight);

        _groundMask = 1 << LayerMask.NameToLayer("Ground");
    }

    private void Start()
    {
        _velocityXId = Animator.StringToHash("VelocityX");
        _velocityZId = Animator.StringToHash("VelocityZ");

        _groundedId = Animator.StringToHash("Grounded");
        _initiateJumpId = Animator.StringToHash("InitiateJump");
        _jumpLoopId = Animator.StringToHash("JumpLoop");
    }

    private void Update()
    {
        HandleTurnInput();
        HandleMoveInput();
        HandleShootInput();
        HandleJumpInput();
    }

    private void HandleTurnInput()
    {
        Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
        float enter;

        if (_plane.Raycast(ray, out enter))
        {
            Vector3 hitPoint = ray.GetPoint(enter);
            _lookDirection = hitPoint - new Vector3(transform.position.x, AimHeight, transform.position.z);
        }
    }

    private void HandleMoveInput()
    {
        _moveInput = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")).normalized;
        Vector3 local = transform.InverseTransformDirection(_moveInput);
        _currentLocal = Vector3.MoveTowards(_currentLocal, local, 5f * Time.deltaTime);

        _animator.SetFloat(_velocityXId, _currentLocal.x);
        _animator.SetFloat(_velocityZId, _currentLocal.z);
    }

    private void HandleShootInput()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            Instantiate(Bullet, Muzzle.position, Muzzle.rotation);
        }
    }

    private void HandleJumpInput()
    {
        if (Input.GetButtonDown("Jump") && _grounded)
        {
            _animator.SetTrigger(_initiateJumpId);
        }
    }

    private void FixedUpdate()
    {
        if (_lookDirection != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(_lookDirection);
            _rigidbody.rotation = lookRotation;
        }

        CheckGrounded();

        if (_animator.GetBool(_jumpLoopId))
        {
            _rigidbody.MovePosition(_rigidbody.position + _moveInput * AirSpeed * Time.fixedDeltaTime);
        }
    }

    private void OnJumpUp()
    {
        _rigidbody.AddForce(transform.up * JumpForce);
    }

    private void CheckGrounded()
    {
        _grounded = Physics.CheckSphere(transform.position, 0.5f, _groundMask);
        _animator.SetBool(_groundedId, _grounded);
    }
}
