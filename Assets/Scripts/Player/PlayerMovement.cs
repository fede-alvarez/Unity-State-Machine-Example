using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
  [Header("Properties")]
  [SerializeField] private float _movementSpeed;
  [SerializeField] private float _runSpeed;
  [SerializeField] private float _jumpForce;

  [Header("Game Feel")]
  [SerializeField] private float _coyoteTime = 0.1f;
  [SerializeField] private float _jumpBufferTime = 0.1f;
  [SerializeField] private float _rollTime = 1.0f;

  [Header("Gravity")]
  [SerializeField] private float _normalGravityScale = 1;
  [SerializeField] private float _fallGravityScale = 1;

  [Header("Model")]
  [SerializeField] private Transform _playerModel;

  private float _gravity = Mathf.Abs(Physics.gravity.y);
  private float _gravityScale;

  private Vector3 _movement;

  private bool _jumpPressed = false;
  private bool _rollPressed = false;
  private bool _attackPressed = false;

  private bool _isGrounded = false;

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
    float normalizedTime = animStateInfo.normalizedTime;
    
    return (normalizedTime > 0.8f) ? true : false;
  }

  private void Update() 
  {
      _movement = new Vector3(Input.GetAxisRaw("Vertical"), 0, -Input.GetAxisRaw("Horizontal"));

      if (_movement != Vector3.zero)
            _playerModel.forward = Vector3.Slerp(_playerModel.forward, _movement, Time.deltaTime * 10);

      _jumpPressed = Input.GetButtonDown("Fire2"); // PS4 - Cross - Jump
      _rollPressed = Input.GetButtonDown("Fire3"); // PS4 - Circle - Roll
      _attackPressed = Input.GetButtonDown("Fire1"); // PS4 - Square - Attack
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

  public void Roll()
  {
    _rb.AddForce(_playerModel.transform.forward * 0.1f, ForceMode.Impulse);
  }

  private void ApplyGravity()
  {
    _rb.AddForce(-transform.up * 100 * (_gravity * _gravityScale), ForceMode.Force);
  }

  public void SetGravityScale(bool isFalling = false)
  {
    _gravityScale = (isFalling) ? _fallGravityScale : _normalGravityScale;
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

  public bool JumpPressed
  {
    get { return _jumpPressed; }
  }

  public bool AttackPressed
  {
    get { return _attackPressed; }
  }

  public bool RollPressed
  {
    get { return _rollPressed; }
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
