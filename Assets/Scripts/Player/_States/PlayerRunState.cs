using UnityEngine;
public class PlayerRunState : PlayerBaseState
{
  private PlayerMovement _player;

  public override void OnEnterState(PlayerStateManager stateManager)
  {
    if (_player == null)
      _player = stateManager.GetPlayer;
  }

  public override void UpdateState(PlayerStateManager stateManager)
  {
    return;
  }

  public override void FixedUpdateState(PlayerStateManager player)
  {
    return;
  }
}
