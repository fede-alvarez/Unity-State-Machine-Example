using UnityEngine;

public class SimpleMovement : MonoBehaviour
{
  [SerializeField] private float _cloudsSpeed = 0.1f;
  private void LateUpdate() 
  {
    transform.position += new Vector3(_cloudsSpeed * Time.deltaTime, 0, 0);  
  }
}
