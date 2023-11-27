using UnityEngine;
using UnityEngine.SceneManagement;

namespace LevelEvents
{
    public class LevelEndTrigger : MonoBehaviour
    {
        private int _index;

        private void Awake()
        {
            _index = SceneManager.GetActiveScene().buildIndex;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player") && _index <= SceneManager.sceneCount)
                SceneManager.LoadScene(_index + 1);
            else
                SceneManager.LoadScene("LevelSelectionScene");
        }
    }
}
