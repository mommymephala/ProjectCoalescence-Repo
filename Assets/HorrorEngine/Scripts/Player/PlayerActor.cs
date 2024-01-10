using UnityEngine;

namespace HorrorEngine
{
    public class PlayerActor : Actor
    {
        void Start()
        {
            if (GameManager.Exists)
                GameManager.Instance.RegisterPlayer(this);
        }

        public void SetVisible(bool visible)
        {
            gameObject.SetActive(visible);
        }
    }

}