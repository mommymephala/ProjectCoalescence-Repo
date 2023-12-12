using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }
    
    [Header("Player")]
    [SerializeField] private EventReference playerfootsteps;
    [SerializeField] private EventReference playerJump;
    [SerializeField] private EventReference playerAttackRanged;
    [SerializeField] private EventReference weaponSwitch;
    [SerializeField] private EventReference rifle;
    [SerializeField] private EventReference deagle;
    [SerializeField] private EventReference shotgun;
    [SerializeField] private EventReference metalDoor;
    [SerializeField] private EventReference playerHurt;
    EventInstance _playerFootstepInstance;
    void Awake()
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
        if (playerfootsteps.IsNull)
        {
            Debug.LogWarning("Fmod event not found: playerFootstep");
            return;
        }
        _playerFootstepInstance = RuntimeManager.CreateInstance(playerfootsteps);
        
        _playerFootstepInstance.start();
        _playerFootstepInstance.release();
     // RuntimeManager.PlayOneShot(playerfootsteps,transform.position);
       
    }
    public void PlayDeagleRanged()
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
