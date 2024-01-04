using UnityEngine;

namespace HorrorEngine
{
    public interface IInteractor { }

    public class PlayerInteractor : MonoBehaviour, IInteractor, IDeactivateWithActor
    {
        [SerializeField] InteractionColliderDetector m_Detector;
        private Interactive m_Interactive; // Holds the currently focused interactive object

        private IPlayerInput m_Input;

        public bool IsInteracting { get; private set; }

        private void Awake()
        {
            m_Input = GetComponentInParent<IPlayerInput>();
        }

        private void OnEnable()
        {
            IsInteracting = CheckIsInteracting();
        }

        private void Update()
        {
            IsInteracting = CheckIsInteracting();
        }

        private bool CheckIsInteracting()
        {
            if (!PauseController.Instance.IsPaused && m_Detector.FocusedInteractive && m_Input.IsInteractingDown())
            {
                m_Interactive = m_Detector.FocusedInteractive; // Set the focused interactive object
                Interact();
                return true;
            }
            else
            {
                return false;
            }
        }

        private void Interact()
        {
            if (m_Interactive != null)
            {
                m_Interactive.Interact(GetComponentInParent<PlayerInteractor>());
            }
            else
            {
                Debug.LogError("No interactive object focused.");
            }
        }
    }
}