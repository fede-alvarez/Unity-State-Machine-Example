using UnityEngine;
public class PlayerWalkState : PlayerBaseState
{
    private PlayerMovement _player;

    public override void OnEnterState(PlayerStateManager stateManager)
    {
      if (_player == null) _player = stateManager.GetPlayer;
    }

    public override void UpdateState(PlayerStateManager stateManager)
    {
      float magnitude = _player.Velocity.magnitude;
      _player.SetAnimation("Velocity", (magnitude > 0) ? magnitude : 0, false);

      if(!_player.IsMoving)
        stateManager.SwitchState(PlayerStateManager.PlayerState.Idle);

      if (_player.OnJumpPressed())
        stateManager.SwitchState(PlayerStateManager.PlayerState.Jump);

      if (_player.AttackPressed)
        stateManager.SwitchState(PlayerStateManager.PlayerState.Attack);

      if (_player.RollPressed)
        stateManager.SwitchState(PlayerStateManager.PlayerState.Roll);
    }

    public override void FixedUpdateState(PlayerStateManager player)
    {
      _player.ApplyInputMovement();
    }
}
