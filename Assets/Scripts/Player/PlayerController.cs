using UnityEngine;

public class PlayerController : MonoBehaviour
{
  public enum States {
        Idle,
        Walk,
        Run,
        Jump,
        Fall,
        Roll,
        SwordSlash
    }

    [Header("Properties")]
    [SerializeField] private States _currentState = States.Idle;
    [SerializeField] private float _movementSpeed;
    [SerializeField] private float _runSpeed;
    [SerializeField] private float _jumpForce;
    
    [Header("Game Feel")]
    [SerializeField] private float _coyoteTime = 0.1f;
    [SerializeField] private float _jumpBufferTime = 0.1f;
    [SerializeField] private float _rollTime = 1.0f;

    [Header("Gravity")]
    [SerializeField] private float _gravityScale = 1;
    [SerializeField] private float _fallGravityScale = 1;

    [Header("Model")]
    [SerializeField] private Transform _playerModel;
    

    private bool _isGrounded = false;
    private bool _isJumping = false;
    private bool _isRunning = false;
    private bool _isRolling = false;
    private bool _isAttacking = false;

    private bool _registerInput = true;

    private bool _jumpPressed = false;
    private bool _rollPressed = false;
    private bool _attackPressed = false;

    private float _currentScale;
    private float _gravity = Mathf.Abs(Physics.gravity.y);

    private float _coyoteTimeCounter = 0;
    private float _rollCounter = 0;
    private float _attackCounter = 0;
    private float _lastJumpPressed;
    
    private Vector3 _movement;

    private Animator _playerAnimator;
    private Rigidbody _rb;

    private void Awake() 
    {
        _rb = GetComponent<Rigidbody>();
        _playerAnimator = _playerModel.GetComponent<Animator>();
    }

    void Update()
    {
        _movement = new Vector3(Input.GetAxisRaw("Vertical"), 0, -Input.GetAxisRaw("Horizontal"));

        if (_movement != Vector3.zero)
          _playerModel.forward = Vector3.Slerp(_playerModel.forward, _movement, Time.deltaTime * 10);

        _jumpPressed = Input.GetButtonDown("Fire2"); // PS4 - Cross - Jump
        _rollPressed = Input.GetButtonDown("Fire3"); // PS4 - Circle - Roll
        _attackPressed = Input.GetButtonDown("Fire1"); // PS4 - Square - Attack

        if (_jumpPressed)
            _lastJumpPressed = Time.time;

        if (!_isGrounded)
        {
            _coyoteTimeCounter += Time.deltaTime;
        }
        else
        {
            _coyoteTimeCounter = 0;
        }

        switch(_currentState)
        {
            case States.Idle:
                Idle();
                break;
            case States.Walk:
                Walk();
                break;
            case States.Run:
                Run();
                break;
            case States.Jump:
                Jump();
                break;
            case States.Fall:
                Fall();
                break;
            case States.Roll:
                Roll();
                break;
            case States.SwordSlash:
                Attack();
                break;
        }

        _playerAnimator.SetFloat("Velocity", _rb.velocity.magnitude);
    }

    private void FixedUpdate() 
    {
        SetVelocity();

        switch(_currentState)
        {
            case States.Jump:
                PhysicalJump();
                break;
            case States.Roll:
                _rb.AddForce(_playerModel.transform.forward * 0.1f, ForceMode.Impulse);
                break;
        }

        // Set gravity
        _rb.AddForce(-transform.up * 100 * (_gravity * _currentScale), ForceMode.Force);
    }

    private void PhysicalJump()
    {
        if (_isJumping) return;
        _isJumping = true;

        _rb.AddForce(transform.up * _jumpForce, ForceMode.VelocityChange);
    }

    #region Player States
    private void Idle()
    {
        _currentScale = _gravityScale;
        _rb.velocity = Vector3.zero;

        _isJumping = false;
        _isAttacking = false;
        _registerInput = true;
        
        CheckJump();

        if (_attackPressed && !_isAttacking) 
            SetState(States.SwordSlash);
      
        if (_movement.magnitude > 0.1f)
            SetState(States.Walk);
    }

    private void CheckJump()
    {
        if (!JumpInputPressed()) return;
        SetState(States.Jump);
    }

    private void Walk()
    {
        _currentScale = _gravityScale;

        _isJumping = false;
        _isRunning = false;
        _registerInput = true;

        CheckJump();

        if (_attackPressed && !_isAttacking) 
            SetState(States.SwordSlash);

        if (_rollPressed && _isGrounded)
            SetState(States.Roll);
          
        if (_movement.magnitude < 0.05f)
        {
            SetState(States.Idle);
        }
    }

    private void Jump()
    {
        _currentScale = _gravityScale;
        _registerInput = true;
        _isAttacking = false;

        if (_rb.velocity.y < 0)
            SetState(States.Fall);
    }

    private void Run()
    {
        _isJumping = false;
        _isRunning = true;
        _isAttacking = false;
        _currentScale = _gravityScale;
        _registerInput = true;

        if (JumpInputPressed())
        {
            SetState(States.Jump);
        }
    }

    private void Attack()
    {
        if (_isAttacking && _attackCounter == 0) {
            _attackCounter += Time.deltaTime;

            if (_attackCounter >= 0.6f)
            {
                _attackCounter = 0;
                _isAttacking = false;
                _playerAnimator.SetLayerWeight(1,0);
                SetState((_isRunning) ? States.Walk : States.Idle);
            }
            return;
        }

        _isAttacking = true;

        _currentScale = _gravityScale;
        _isJumping = false;
        _isRunning = false;
        _registerInput = true;

        _playerAnimator.SetTrigger("Attack");
        _playerAnimator.SetLayerWeight(1,1);
    }

    private void Fall()
    {
        _currentScale = _fallGravityScale;
        _isAttacking = false;
        _registerInput = true;
        
        if (_isGrounded)// && _rb.velocity.y <= 0)
            SetState((_isRunning) ? States.Run : States.Idle);
    }

    private void Roll()
    {
        if (_isRolling) 
        {
            _registerInput = false;

            _rollCounter += Time.deltaTime;

            if (_rollCounter >= _rollTime)
            {
                _rollCounter = 0;
                _isRolling = false;
                SetState((_isRunning) ? States.Run : States.Idle);
            }
            return;   
        }

        _isRolling = true;
        _playerAnimator.SetTrigger("Roll");
        //_playerAnimator.SetLayerWeight(1,0);

        _currentScale = _gravityScale;
        _isJumping = false;
        _isRunning = false;
        _isAttacking = false;
    }

    #endregion

    private bool JumpInputPressed()
    {
        bool hasBufferedJump = _isGrounded && _lastJumpPressed + _jumpBufferTime > Time.time;
        bool hasCoyoteTime = _isGrounded || (_coyoteTimeCounter > 0.03f && _coyoteTimeCounter < _coyoteTime);

        return (_jumpPressed && hasCoyoteTime || hasBufferedJump) ? true : false;
    }

    private void SetVelocity()
    {
        if (!_registerInput) return;

        Vector3 dir = transform.forward * _movement.z + transform.right * _movement.x;
        Vector3 rbVelocity = dir.normalized * _movementSpeed;
        rbVelocity.y = _rb.velocity.y;
        
        _rb.velocity = rbVelocity;
    }

    private void OnTriggerEnter(Collider other) 
    {
        if (!other.CompareTag("Floor")) return;
        _isGrounded = true;
    }

    private void OnTriggerExit(Collider other) 
    {
        if (!other.CompareTag("Floor")) return;
        _isGrounded = false;
    }

    private void SetState(States nextState)
    {
        _currentState = nextState;
    }
}
