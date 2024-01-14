using System;
using System.Collections;
using HorrorEngine;
using UnityEngine;
using FMODUnity;
public class BatonWeapon : MonoBehaviour
{
    public EventReference batonAttack;
    public EventReference batonSwoosh;
    public EventReference batonElectric;
    
    private Animator _animator;
    private bool _isAttacking = false;
    [SerializeField] private Collider hitbox;
    public float attackDelay;
    private DepleteEquipment _depleteEquipment;
    
    [SerializeField] private GameObject chargedAttackEffect;
    private bool IsCharged => _depleteEquipment.HasCharge();


    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _depleteEquipment = GetComponent<DepleteEquipment>();
    }

    private void Start()
    {
        hitbox.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (PauseController.Instance.IsPaused)
        {
            return;
        }
        
        if (Input.GetMouseButtonDown(0) && !_isAttacking)
        {
            StartCoroutine(AttackRoutine());
        }
    }

    private IEnumerator AttackRoutine()
    {
        StartAttack();
        yield return new WaitForSeconds(GetCurrentAnimationLength() + attackDelay);
        FinishAttack();
    }

    private float GetCurrentAnimationLength()
    {
        AnimatorStateInfo stateInfo = _animator.GetCurrentAnimatorStateInfo(0);
        return stateInfo.length;
    }

    private void StartAttack()
    {
        _isAttacking = true;
        _animator.SetTrigger("Attack");
        
        if (IsCharged)
        {
            PlayBatonElectric();
            _depleteEquipment.Deplete();
            EnableChargedAttackEffects();
            hitbox.GetComponent<BatonHit>().SetChargedAttack(true);
        }
        
        else
        {
            PlayBatonAttack();
            hitbox.GetComponent<BatonHit>().SetChargedAttack(false); // Unflag the hit as a charged attack
        }
    }
    
    public void EnableHitbox()
    {
        hitbox.gameObject.SetActive(true);
    }

    public void DisableHitbox()
    {
        hitbox.gameObject.SetActive(false);
    }

    private void FinishAttack()
    {
        _isAttacking = false;
    }
    
    private void EnableChargedAttackEffects()
    {
        if (chargedAttackEffect == null) return;
        GameObject effectInstance = Instantiate(chargedAttackEffect, hitbox.transform.position, Quaternion.identity);
        Destroy(effectInstance, 1f);
    }
    
    public void PlayBatonAttack()
    {
        if (batonAttack.IsNull)
        {
            Debug.LogWarning("Fmod event not found: BatonAttack");
            return;
        }

        Debug.Log("batonAttack");
        RuntimeManager.PlayOneShot(batonAttack, transform.position);
    }
    
    public void PlayBatonElectric()
    {
        if (batonElectric.IsNull)
        {
            Debug.LogWarning("Fmod event not found: BatonAttack");
            return;
        }

        Debug.Log("bruh");
        RuntimeManager.PlayOneShot(batonElectric, transform.position);
    }

    public void PlayBatonSwoosh()
    {
        if (batonSwoosh.IsNull)
        {
            Debug.LogWarning("Fmod event not found: BatonAttack");
            return;
        }

        Debug.Log("batonAttack");
        RuntimeManager.PlayOneShot(batonSwoosh, transform.position);
    }
}