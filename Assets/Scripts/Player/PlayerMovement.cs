using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
  [Header("Properties")]
  [SerializeField] private float _movementSpeed;
  [SerializeField] private float _runSpeed;
  [SerializeField] private float _jumpForce;
  [SerializeField] private float _minJumpForceScale = 0.3f;

  [Header("Game Feel")]
  [SerializeField] private float _coyoteTime = 0.1f;
  [SerializeField] private float _jumpBufferTime = 0.1f;

  [Header("Gravity")]
  [SerializeField] private float _normalGravityScale = 1;
  [SerializeField] private float _fallGravityScale = 1;

  [Header("Model")]
  [SerializeField] private Transform _playerModel;

  [Header("Ground detection")]
  [SerializeField] private LayerMask _floorMask;

  private float _gravity = Mathf.Abs(Physics.gravity.y);
  private float _gravityScale;

  private Vector3 _movement;

  private bool _jumpPressed = false;
  private bool _jumpReleased = false;
  private bool _rollPressed = false;
  private bool _attackPressed = false;
  private bool _lockRotation = false;

  private bool _isGrounded = false;

  private float _coyoteTimeCounter = 0;
  private float _lastJumpPressed;

  private Animator _playerAnimator;
  private Rigidbody _rb;

  private void Awake() 
  {
      _rb = GetComponent<Rigidbody>();
      _playerAnimator = _playerModel.GetComponent<Animator>();
  }

  private void Start() 
  {
      _gravityScale = _normalGravityScale;
  }

  private void Update() 
  {
      _movement = new Vector3(Input.GetAxisRaw("Vertical"), 0, -Input.GetAxisRaw("Horizontal"));

      if (_movement != Vector3.zero && !_lockRotation)
            _playerModel.forward = Vector3.Slerp(_playerModel.forward, _movement, Time.deltaTime * 10);

      _isGrounded = GroundChecker();

      _jumpPressed = Input.GetButtonDown("Fire2"); // PS4 - Cross - Jump
      _jumpReleased = Input.GetButtonUp("Fire2");
      _rollPressed = Input.GetButtonDown("Fire3"); // PS4 - Circle - Roll
      _attackPressed = Input.GetButtonDown("Fire1"); // PS4 - Square - Attack
      
      if (_jumpPressed)
        _lastJumpPressed = Time.time;

      if (!_isGrounded)
        _coyoteTimeCounter += Time.deltaTime;
      else
        _coyoteTimeCounter = 0;
  }

  private bool GroundChecker()
  {
      RaycastHit hit;

      //Debug.DrawRay(transform.position + new Vector3(0,0.08f,0),-transform.up * 0.15f, currentColor);
      if (Physics.Raycast(transform.position + new Vector3(0,0.08f,0), -transform.up , out hit, 0.15f, _floorMask)) 
      {
          return true;
      }

      return false;
  }

  private void FixedUpdate() 
  {
      ApplyGravity();
  }

  public void ApplyInputMovement()
  {
      Vector3 dir = transform.forward * _movement.z + transform.right * _movement.x;
      Vector3 rbVelocity = dir.normalized * _movementSpeed;
      rbVelocity.y = _rb.velocity.y;
      
      _rb.velocity = rbVelocity;
  }

  public void Jump()
  {
    _rb.AddForce(transform.up * _jumpForce, ForceMode.VelocityChange);
  }

  public void HalfJump()
  {
    _rb.AddForce(-transform.up * (_jumpForce * _minJumpForceScale), ForceMode.VelocityChange);
  }

  public void Roll()
  {
    _rb.AddForce(_playerModel.transform.forward * 0.15f, ForceMode.Impulse);
  }

  private void ApplyGravity()
  {
    _rb.AddForce(-transform.up * 100 * (_gravity * _gravityScale), ForceMode.Force);
  }

  public void SetGravityScale(bool isFalling = false)
  {
    _gravityScale = (isFalling) ? _fallGravityScale : _normalGravityScale;
  }

  public void SetAnimation(string animationName, float value = 0, bool isTrigger = true)
  {
    if (isTrigger)
      _playerAnimator.SetTrigger(animationName);
    else
      _playerAnimator.SetFloat(animationName, value);

    //_playerAnimator.SetLayerWeight(1,1);
  }

  public void SetLayerWeight(int value)
  {
    _playerAnimator.SetLayerWeight(1, value);
  }

  public bool AnimationEnded(int defaultLayer = 0)
  {
    AnimatorStateInfo animStateInfo = _playerAnimator.GetCurrentAnimatorStateInfo(defaultLayer); // Layer 0 (Base Layer)    
    return (animStateInfo.normalizedTime > 0.5f) ? true : false;
  }

  public bool OnJumpPressed()
  {
    if (InCoyoteTime && JumpBuffered)
      return true;

    return false;
  }

  public void ToggleRotationLocked()
  {
    _lockRotation = !_lockRotation;
  }

  public bool JumpPressed
  {
    get { return _jumpPressed; }
  }

  public bool JumpReleased
  {
    get { return _jumpReleased; }
  }

  public bool AttackPressed
  {
    get { return _attackPressed; }
  }

  public bool RollPressed
  {
    get { return _rollPressed; }
  }

  public bool JumpBuffered
  {
    //print(_lastJumpPressed + " - " + (_lastJumpPressed + _jumpBufferTime) + " = " + (_lastJumpPressed + _jumpBufferTime > Time.time));
    get 
    {  
      return _lastJumpPressed > 0 && _lastJumpPressed + _jumpBufferTime > Time.time;
    }
  }

  public bool InCoyoteTime
  {
    get { return _isGrounded || (_coyoteTimeCounter > 0.03f && _coyoteTimeCounter < _coyoteTime); }
  }

  public bool IsMoving 
  {
    get { return _movement.magnitude > 0.01f; }
  }

  public bool IsFalling 
  {
    get { return _rb.velocity.y < 0; }
  }

  public bool IsGrounded 
  {
    get { return _isGrounded; }
  }

  public Vector3 GetMovement 
  {
    get { return _movement; }
  }

  public float GetSpeed 
  {
    get { return _movementSpeed; }
  }

  public Vector3 Velocity
  {
    set { _rb.velocity = value; }
    get { return _rb.velocity; }
  }
}
