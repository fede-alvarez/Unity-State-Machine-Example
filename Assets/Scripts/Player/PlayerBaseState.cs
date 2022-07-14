using UnityEngine;

public abstract class PlayerBaseState : MonoBehaviour
{
    public abstract void OnEnterState(PlayerStateManager player);
    public abstract void UpdateState(PlayerStateManager player);
    public abstract void FixedUpdateState(PlayerStateManager player);
    public virtual void OnExitState(PlayerStateManager player) {}
}
