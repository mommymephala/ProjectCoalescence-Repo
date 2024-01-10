using UnityEngine;

public class LightControl : MonoBehaviour
{
    [SerializeField] private Light controlledLight;
    [SerializeField] private float newIntensity = 5.0f;
    // [SerializeField] private Color newColor = Color.blue;

    private void OnTriggerEnter(Collider other)
    {
        if (controlledLight != null && other.CompareTag("Player"))
        {
            controlledLight.gameObject.SetActive(true);
            controlledLight.intensity = newIntensity;
            // controlledLight.color = newColor;
        }
    }
}