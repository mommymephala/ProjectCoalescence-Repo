using UnityEngine;
using FMODUnity;
using FMOD.Studio;
using System.Collections.Generic;
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; set; }
    
    public enum AttackType
    {
       NormalAttack,
       HeavyAttack,
    }
    public enum EnemyType
    {
        BaseEnemy,
        ChipEnemy,
        
    }
    [System.Serializable]
    public class EnemySounds
    {
        public EventReference footstep;
        public EventReference takeDamage;
        public EventReference Normalattack;
        public EventReference Heavyattack;
        public EventReference death;
        public EventReference idle;
    }

    [Header("Player")]
    [SerializeField] private EventReference playerFootsteps;
    [SerializeField] private EventReference playerTakeDamage;
    [SerializeField] private EventReference playerDeath;
    [Header("Enemy Sounds")]
    public EnemySounds baseEnemySounds;
    public EnemySounds chipEnemySounds;
    
    // [SerializeField] private EventReference playerJump;
    // [SerializeField] private EventReference playerAttackRanged;
    // [SerializeField] private EventReference weaponSwitch;
    [SerializeField] private EventReference metalDoor;
    // [SerializeField] private EventReference playerHurt;
    private EventInstance _playerFootstepInstance;
    private EventInstance _playerTakeDamage;


    private EventInstance _baseEnemyFootstepInstance;
    
    private Dictionary<EnemyType, EnemySounds> enemySoundsMap;
   
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
        enemySoundsMap = new Dictionary<EnemyType, EnemySounds>
        {
            { EnemyType.BaseEnemy, baseEnemySounds },
            { EnemyType.ChipEnemy, chipEnemySounds }
            // Add other enemies here
        }; 
        
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
        
    }
    public void PlayPlayerTakeDamage()
    {
        if (playerTakeDamage.IsNull)
        {
            Debug.LogWarning("Fmod event not found: playertakedamage");
            return;
        }

        _playerTakeDamage = RuntimeManager.CreateInstance(playerTakeDamage);
        
        _playerTakeDamage.start();
        _playerTakeDamage.release();
    }

    public void PlayPlayerDeath()
    {
        if (playerTakeDamage.IsNull)
        {
            Debug.LogWarning("Fmod event not found: playertakedamage");
            return;
        }
        RuntimeManager.PlayOneShot(playerDeath, transform.position);

    }
    public void PlayEnemyFootStep(GameObject enemyObject, EnemyType enemyType)
    {
        if (enemySoundsMap[enemyType].footstep.IsNull)
        {
            Debug.LogWarning("Fmod event not found: enemyFootstep");
            return;
        }
        RuntimeManager.PlayOneShot(enemySoundsMap[enemyType].footstep, enemyObject.transform.position);
    }
    public void PlayEnemyIdle(GameObject enemyObject, EnemyType enemyType)
    {
        if (enemySoundsMap[enemyType].idle.IsNull)
        {
            Debug.LogWarning("Fmod event not found: enemy idle");
            return;
        }
        RuntimeManager.PlayOneShot(enemySoundsMap[enemyType].idle, enemyObject.transform.position);
    }
    public void PlayEnemyAttack(GameObject enemyobject, EnemyType enemyType, AttackType attackType)
    {
        if (enemySoundsMap[enemyType].Normalattack.IsNull)
        {
            Debug.LogWarning("Fmod event not found: enemyattack");
            return;
        }
        if (enemySoundsMap[enemyType].Heavyattack.IsNull)
        {
            Debug.LogWarning("Fmod event not found: enemyattack");
            return;
        }
        switch (attackType)
        {
            case AttackType.NormalAttack:
                RuntimeManager.PlayOneShot(enemySoundsMap[enemyType].Normalattack, enemyobject.transform.position);
                break;
            case AttackType.HeavyAttack:
                RuntimeManager.PlayOneShot(enemySoundsMap[enemyType].Heavyattack, enemyobject.transform.position);
                break;
            default:
                Debug.LogWarning($"Unsupported attack type: {attackType}");
                return;
        }
        
    //  RuntimeManager.PlayOneShot(enemySoundsMap[enemyType].attack, enemyobject.transform.position);
    }
    

    public void PlayEnemyTakeDamage(GameObject enemyobject, EnemyType enemyType)
    {
        if (enemySoundsMap[enemyType].takeDamage.IsNull)
        {
            Debug.LogWarning("Fmod event not found: enemy take damage");
            return;
        }
        
       RuntimeManager.PlayOneShot(enemySoundsMap[enemyType].takeDamage, enemyobject.transform.position);
    }
    public void PlayEnemyDeath(GameObject enemyobject,EnemyType enemyType)
    {
        if (enemySoundsMap[enemyType].death.IsNull)
        {
            Debug.LogWarning("Fmod event not found: enemy death");
            return;
        }
        
        RuntimeManager.PlayOneShot(enemySoundsMap[enemyType].death, enemyobject.transform.position);
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
