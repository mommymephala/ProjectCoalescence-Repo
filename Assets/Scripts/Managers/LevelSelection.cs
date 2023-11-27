using UnityEngine;
using UnityEngine.SceneManagement;

namespace Managers
{
    public class LevelSelection : MonoBehaviour
    {
        public void LoadLevel(string levelName)
        {
            SceneManager.LoadScene(levelName);
        }
    }
}