using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
  public void ResetLevel()
  {
    SceneManager.LoadScene("GameScene");
  }
}
