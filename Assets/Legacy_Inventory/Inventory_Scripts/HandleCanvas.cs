using UnityEngine;
using UnityEngine.UI;

public class HandleCanvas : MonoBehaviour
{
    private CanvasScaler _scaler;
    private void Awake()
    {
        _scaler = GetComponent<CanvasScaler>();
        _scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
    }
}
