using UnityEngine;
using FMODUnity;
using FMOD.Studio;
public class AudioManager : MonoBehaviour
{
    private static AudioManager Instance { get; set; }
    
    [Header("Player")]
    [SerializeField] private EventReference playerFootsteps;
    // [SerializeField] private EventReference playerJump;
    // [SerializeField] private EventReference playerAttackRanged;
    // [SerializeField] private EventReference weaponSwitch;
    [SerializeField] private EventReference metalDoor;
    // [SerializeField] private EventReference playerHurt;
    private EventInstance _playerFootstepInstance;

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
     // RuntimeManager.PlayOneShot(playerfootsteps,transform.position);
       
    }
    /*public void PlayDeagleRanged()
    {
        if (deagle.IsNull)
        {
            Debug.LogWarning("Fmod event not found: playerAttackRanged");
            return;
        }
        RuntimeManager.PlayOneShot(deagle, transform.position);
       
        Debug.Log("deagle ses");
    }
    public void PlayRifleRanged()
    {
        if (deagle.IsNull)
        {
            Debug.LogWarning("Fmod event not found: playerAttackRanged");
            return;
        }
        RuntimeManager.PlayOneShot(rifle, transform.position);
       
        Debug.Log("deagle ses");
    }
    public void PlayShotgunRanged()
    {
        if (deagle.IsNull)
        {
            Debug.LogWarning("Fmod event not found: playerAttackRanged");
            return;
        }
        RuntimeManager.PlayOneShot(shotgun, transform.position);
       
        Debug.Log("shotgun ses");
    }*/
    
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
