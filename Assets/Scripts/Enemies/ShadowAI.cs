using System.Collections;
using HorrorEngine;
using Interfaces;
using UnityEngine;

public class ShadowAI : MonoBehaviour, IDamageable
{
    private Health _health;
    public Transform[] teleportLocations;
    public float fadeDuration = 2.0f;

    private Renderer _renderer;
    private Color _originalColor;

    private void Awake()
    {
        _health = GetComponent<Health>();
        _renderer = GetComponent<Renderer>();
        if (_renderer != null)
        {
            _originalColor = _renderer.material.color;
        }
        StartCoroutine(TeleportRoutine());
    }

    private IEnumerator TeleportRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(4f, 7f));
            TeleportToRandomLocation();
        }
    }

    private void TeleportToRandomLocation()
    {
        if (teleportLocations.Length > 0)
        {
            int randomIndex = Random.Range(0, teleportLocations.Length);
            transform.position = teleportLocations[randomIndex].position;
        }
    }

    public void TakeDamage(float damage, bool isChargedAttack, bool isWeakpoint)
    {
        if (isChargedAttack)
        {
            _health.DamageReceived(damage);
        }

        if (_health.IsDead)
        {
            StartCoroutine(FadeOutAndDeactivate());
        }
    }

    private IEnumerator FadeOutAndDeactivate()
    {
        float elapsed = 0.0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            var fraction = elapsed / fadeDuration;
            Color newColor = _originalColor;
            newColor.a = Mathf.Lerp(_originalColor.a, 0, fraction);
            _renderer.material.color = newColor;
            yield return null;
        }

        gameObject.SetActive(false);
    }
}