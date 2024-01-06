using UnityEngine;

public class CrosshairManager : MonoBehaviour
{
    public static CrosshairManager Instance { get; private set; }

    // public RectTransform topCrosshair;
    // public RectTransform bottomCrosshair;
    // public RectTransform leftCrosshair;
    // public RectTransform rightCrosshair;
    public RectTransform reticle;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}