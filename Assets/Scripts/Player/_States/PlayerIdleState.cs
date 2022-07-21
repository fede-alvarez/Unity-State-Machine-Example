using UnityEngine;
public class PlayerIdleState : PlayerBaseState
{
  private PlayerMovement _player;

  public override void OnEnterState(PlayerStateManager stateManager)
  {
    if (_player == null)
      _player = stateManager.GetPlayer;
  }

  public override void UpdateState(PlayerStateManager stateManager)
  {
    // Animation
    float magnitude = _player.Velocity.magnitude;
    _player.SetAnimation("Velocity", (magnitude > 0) ? magnitude : 0, false);

    // Parameters + Conditions to transition
    if (_player.IsMoving)
      stateManager.SwitchState(PlayerStateManager.PlayerState.Walk);
    
    if (_player.OnJumpPressed()) {
      stateManager.SwitchState(PlayerStateManager.PlayerState.Jump);
      return;
    }
      

    if (_player.AttackPressed)
      stateManager.SwitchState(PlayerStateManager.PlayerState.Attack);
  }

  public override void FixedUpdateState(PlayerStateManager player)
  {
    return;
  }
}
