using UnityEngine;

public class ControlLightIntensity : MonoBehaviour
{
    private void Update()
    {
        RenderSettings.ambientIntensity = 0.35f;
    }
}
