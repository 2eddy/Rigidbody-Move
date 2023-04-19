using UnityEngine;

public class Player : MonoBehaviour
{
    public GameObject Bullet;
    public Transform Muzzle;

    public float AimHeight = 1.45f;
    public float MoveSpeed = 4.5f;

    Animator _animator;
    Rigidbody _rigidbody;
    Camera _mainCamera;

    // Animator
    int _velocityXId;
    int _velocityZId;

    Plane _plane;
    Vector3 _currentLocal;
    Vector3 _lookDirection;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _rigidbody = GetComponent<Rigidbody>();
        _mainCamera = Camera.main;

        _plane = new Plane(Vector3.up, -AimHeight);
    }

    private void Start()
    {
        _velocityXId = Animator.StringToHash("VelocityX");
        _velocityZId = Animator.StringToHash("VelocityZ");
    }

    private void Update()
    {
        HandleTurnInput();
        HandleMoveInput();
        HandleShootInput();
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
        Vector3 moveInput = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")).normalized;
        Vector3 local = transform.InverseTransformDirection(moveInput);
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

    private void FixedUpdate()
    {
        if (_lookDirection != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(_lookDirection);
            _rigidbody.rotation = lookRotation;
        }
    }
}
