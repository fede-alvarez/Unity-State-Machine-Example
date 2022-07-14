using UnityEngine;
public class PlayerRollState : PlayerBaseState
{
  private PlayerMovement _player;

  public override void OnEnterState(PlayerStateManager stateManager)
  {
    if (_player == null)
      _player = stateManager.GetPlayer;

    _player.SetAnimation("Roll");
  }

  public override void UpdateState(PlayerStateManager stateManager)
  {
    if (_player.AnimationEnded(0)) // Roll animation is in Layer 0
    {
      if (!_player.IsMoving)
        stateManager.SwitchState(PlayerStateManager.PlayerState.Idle);
      else
        stateManager.SwitchState(PlayerStateManager.PlayerState.Walk);
    }
  }

  public override void FixedUpdateState(PlayerStateManager player)
  {
    _player.Roll();
  }
}
