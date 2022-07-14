using UnityEngine;
public class PlayerFallState : PlayerBaseState
{
  private PlayerMovement _player;

  public override void OnEnterState(PlayerStateManager stateManager)
  {
    if (_player == null)
      _player = stateManager.GetPlayer;

    _player.SetGravityScale(true);
  }

  public override void UpdateState(PlayerStateManager stateManager)
  {
    if (_player.IsGrounded)
    {
      if (!_player.IsMoving)
        stateManager.SwitchState(PlayerStateManager.PlayerState.Idle);
      else
        stateManager.SwitchState(PlayerStateManager.PlayerState.Walk);
    }
      
  }

  public override void FixedUpdateState(PlayerStateManager player)
  {
    _player.ApplyInputMovement();
  }

  public override void OnExitState(PlayerStateManager player) 
  {
    _player.SetGravityScale(false);
  }
}
