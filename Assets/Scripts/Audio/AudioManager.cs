using UnityEngine;
using FMODUnity;
using FMOD.Studio;
public class AudioManager : MonoBehaviour
{
    private static AudioManager Instance { get; set; }
    
    [Header("Player")]
    [SerializeField] private EventReference playerFootsteps;
    [SerializeField] private EventReference playerTakeDamage;
    // [SerializeField] private EventReference playerJump;
    // [SerializeField] private EventReference playerAttackRanged;
    // [SerializeField] private EventReference weaponSwitch;
    [SerializeField] private EventReference metalDoor;
    // [SerializeField] private EventReference playerHurt;
    private EventInstance _playerFootstepInstance;
    private EventInstance _playerTakeDamage;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
        
        DontDestroyOnLoad(this);
    }
    
    public void PlayFootstep()
    {
        if (playerFootsteps.IsNull)
        {
            Debug.LogWarning("Fmod event not found: playerFootstep");
            return;
        }
        _playerFootstepInstance = RuntimeManager.CreateInstance(playerFootsteps);
        
        _playerFootstepInstance.start();
        _playerFootstepInstance.release();
        
     // RuntimeManager.PlayOneShot(playerFootsteps,transform.position);
     
    }

    public void PlayPlayerTakeDamage()
    {
        if (playerTakeDamage.IsNull)
        {
            Debug.LogWarning("Fmod event not found: playertakedamage");
            return;
        }

        _playerTakeDamage = RuntimeManager.CreateInstance(playerFootsteps);
        
        _playerTakeDamage.start();
        _playerTakeDamage.release();
        
    }
    
    
    
    public void PlayDoor(GameObject doorObject)
    {
        if (metalDoor.IsNull)
        {
            Debug.LogWarning("Fmod event not found: doorOpen");
            return;
        }
        
        RuntimeManager.PlayOneShot(metalDoor, doorObject.transform.position);
    }
    
}
