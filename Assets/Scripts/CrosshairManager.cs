using UnityEngine;
using WeaponRelated;

public class CrosshairManager : MonoBehaviour
{
    public static CrosshairManager Instance { get; private set; }

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