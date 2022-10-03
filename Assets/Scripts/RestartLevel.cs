using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace DefaultNamespace {
  public class RestartLevel : MonoBehaviour {
    public Button BRestart;

    void Start() {
      BRestart.onClick.AddListener(Restart);
    }

    void Restart() {
      SceneManager.LoadScene ("Game");
    }
  }
}