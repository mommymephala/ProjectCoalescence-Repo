using System;
using System.Collections;
using HorrorEngine;
using UnityEngine;

public class BatonWeapon : MonoBehaviour
{
    private Animator animator;
    private bool isAttacking = false;
    [SerializeField] private Collider hitbox;
    public float attackDelay;
    private DepleteEquipment depleteEquipment;
    
    [SerializeField] private GameObject chargedAttackEffect; // Particle effect for charged attack
    private bool isCharged => depleteEquipment.HasCharge(); // Check if the baton is charged


    private void Awake()
    {
        animator = GetComponent<Animator>();
        depleteEquipment = GetComponent<DepleteEquipment>();
    }

    private void Start()
    {
        hitbox.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && !isAttacking)
        {
            StartCoroutine(AttackRoutine());
        }
    }

    private IEnumerator AttackRoutine()
    {
        StartAttack();
        // Wait for the duration of the current animation state plus the additional delay
        yield return new WaitForSeconds(GetCurrentAnimationLength() + attackDelay);
        FinishAttack();
    }

    private float GetCurrentAnimationLength()
    {
        // Make sure the correct animation layer is referenced, usually layer 0
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        return stateInfo.length;
    }

    private void StartAttack()
    {
        isAttacking = true;
        animator.SetTrigger("Attack");

        if (isCharged)
        {
            depleteEquipment.Deplete();
            // Enable charged attack effects and flags
            EnableChargedAttackEffects(); // Pass the hitbox's position
            hitbox.GetComponent<BatonHit>().SetChargedAttack(true); // Flag the hit as a charged attack
        }
        else
        {
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
        // DisableHitbox();
        isAttacking = false;
    }
    
    private void EnableChargedAttackEffects()
    {
        if (chargedAttackEffect == null) return;
        GameObject effectInstance = Instantiate(chargedAttackEffect, hitbox.transform.position, Quaternion.identity);
        Destroy(effectInstance, 1f);
    }
}