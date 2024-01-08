using System;
using ECM.Components;
using ECM.Examples;
using UnityEngine;

namespace HorrorEngine
{
    public class GamePausedMessage : BaseMessage
    {
        public static GamePausedMessage Default = new GamePausedMessage();
    }

    public class GameUnpausedMessage : BaseMessage
    {
        public static GameUnpausedMessage Default = new GameUnpausedMessage();
    }

    public class PauseController : SingletonBehaviourDontDestroy<PauseController>
    {
        private int _pauseCount;
        public bool IsPaused => _pauseCount > 0;

        public void Pause()
        {
            ++_pauseCount;
            if (_pauseCount == 1)
                MessageBuffer<GamePausedMessage>.Dispatch(GamePausedMessage.Default);
            
            Time.timeScale = 0f;
        }

        // --------------------------------------------------------------------

        public void Resume()
        {
            --_pauseCount;

            if (_pauseCount <= 0)
            {
                MessageBuffer<GameUnpausedMessage>.Dispatch(GameUnpausedMessage.Default);
                Time.timeScale = 1f;
            }

            Debug.Assert(_pauseCount >= 0, "PauseController: PauseCount went below 0");
        }

#if UNITY_EDITOR
        private void Update()
        {
            if (Input.GetKey(KeyCode.Numlock))
            {
                Debug.Break();
            }
        }
#endif
    }    
}