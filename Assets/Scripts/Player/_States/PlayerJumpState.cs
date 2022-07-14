using UnityEngine;
public class PlayerJumpState : PlayerBaseState
{
  private PlayerMovement _player;
  
  public override void OnEnterState(PlayerStateManager stateManager)
  {
    if (_player == null) _player = stateManager.GetPlayer;
    _player.Jump();
  }

  public override void UpdateState(PlayerStateManager stateManager)
  {
    if (_player.IsFalling)
        stateManager.SwitchState(PlayerStateManager.PlayerState.Fall);
  }

  public override void FixedUpdateState(PlayerStateManager player)
  {
    _player.ApplyInputMovement();
  }
}
