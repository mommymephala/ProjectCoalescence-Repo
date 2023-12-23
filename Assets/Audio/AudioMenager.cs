using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;
public class AudioMenager : MonoBehaviour
{
    public static AudioMenager Instance { get; private set; }
    [Header("Player")]
    [SerializeField] private EventReference playerfootsteps;
    [SerializeField] private EventReference playerJump;
    [SerializeField] private EventReference playerAttackRanged;
    [SerializeField] private EventReference weaponSwitch;
    [SerializeField] private EventReference rifle;
    [SerializeField] private EventReference deagle;
    [SerializeField] private EventReference shotgun;
    [SerializeField] private EventReference MetalDoor;
    [SerializeField] private EventReference playerHurt;
    EventInstance playerFootstepInstance;
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
        playerFootstepInstance = RuntimeManager.CreateInstance(playerfootsteps);
        
        playerFootstepInstance.start();
        playerFootstepInstance.release();
     // RuntimeManager.PlayOneShot(playerfootsteps,transform.position);
       
    }
  /*  public void PlayDeagleRanged()
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
        if (rifle.IsNull)
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
        if (MetalDoor.IsNull)
        {
            Debug.LogWarning("Fmod event not found: doorOpen");
            return;
        }
        RuntimeManager.PlayOneShot(MetalDoor, doorObject.transform.position);
    }

}
