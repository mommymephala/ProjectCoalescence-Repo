using UnityEngine;

namespace Level_Events
{
    public class ZoneTrigger : MonoBehaviour
    {
        public ActivationManager.ObjectIdentifier[] objectsToActivate;
        public ActivationManager.ObjectIdentifier[] objectsToDeactivate;

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                foreach (ActivationManager.ObjectIdentifier id in objectsToActivate)
                {
                    ActivationManager.Instance.ActivateObject(id);
                }

                foreach (ActivationManager.ObjectIdentifier id in objectsToDeactivate)
                {
                    ActivationManager.Instance.DeactivateObject(id);
                }
            }
        }
    }
}