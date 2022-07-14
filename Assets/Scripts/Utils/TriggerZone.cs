using UnityEngine;
using UnityEngine.Events;

public class TriggerZone : MonoBehaviour
{
  public UnityEvent OnEnter;

  private void OnTriggerEnter(Collider other) 
  {
    if (!other.CompareTag("Player")) return;
    OnEnter?.Invoke();
  }
}
