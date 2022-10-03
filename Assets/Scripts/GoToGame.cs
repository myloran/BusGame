using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace DefaultNamespace {
  public class GoToGame : MonoBehaviour {
    public Button Button;

    void Start() {
      Button.onClick.AddListener(SwitchScene);
    }

    void SwitchScene() {
      SceneManager.LoadScene ("Game");
    }
  }
}