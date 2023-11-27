using PlayerActions;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI
{
    public class CursorVisibility : MonoBehaviour
    {
        private void Awake()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
            CheckForCursor();
        }

        private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            CheckForCursor();
        }

        private static void CheckForCursor()
        {
            if (FindObjectOfType<PlayerLook>() != null)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Confined;
                Cursor.visible = true;
            }
        }

        private void OnDestroy()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }
}