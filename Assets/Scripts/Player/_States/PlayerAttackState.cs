using UnityEngine;
public class PlayerAttackState : PlayerBaseState
{
  private PlayerMovement _player;

  public override void OnEnterState(PlayerStateManager stateManager)
  {
    if (_player == null)
      _player = stateManager.GetPlayer;

      _player.SetLayerWeight(1);
      _player.SetAnimation("Attack");
  }

  public override void UpdateState(PlayerStateManager stateManager)
  {
    if (_player.AnimationEnded(1)) // Attack animation is in Layer 1
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
    _player.SetLayerWeight(0);
  }
}
