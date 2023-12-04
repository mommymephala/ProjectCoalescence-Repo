using PlayerActions;
using UnityEngine;

public class Bobbing : MonoBehaviour
{
    public PlayerMovement _playerMovement;
    [Header("Headbob")]
    [SerializeField] private bool toggleHeadbob = true;
    [SerializeField] private Transform joint;
    [SerializeField] private float bobSpeed = 10f;
    [SerializeField] private Vector3 bobAmount = new Vector3(.15f, .05f, 0f);
    // [SerializeField] private float headbobFrequency = 2f;
    // [SerializeField] private float walkHeadbobAmount = 0.1f;
    // [SerializeField] private float sprintHeadbobAmount = 0.3f;
    // [SerializeField] private float headbobSpeedMultiplier = 1f;
    //private Vector3 _originalLocalPosition;
    private float _headbobTimer;
    private Vector3 _jointOriginalPos;

    private void Awake()
    {
        //_playerMovement = GetComponent<PlayerMovement>();
    }

    private void Update()
    {
        ApplyBobbing();
    }

    private void ApplyBobbing()
    {
        if (!toggleHeadbob) return;

        if (_playerMovement.moveSpeed > 0)
        {
            var currentBobSpeed = _playerMovement.IsSprinting ? (bobSpeed + (_playerMovement.sprintSpeed * 0.5f)) : bobSpeed;
        
            _headbobTimer += Time.deltaTime * currentBobSpeed;

            joint.localPosition = new Vector3(
                _jointOriginalPos.x + Mathf.Sin(_headbobTimer) * bobAmount.x,
                _jointOriginalPos.y + Mathf.Sin(_headbobTimer) * bobAmount.y,
                _jointOriginalPos.z + Mathf.Sin(_headbobTimer) * bobAmount.z
            );
        }
        else
        {
            _headbobTimer = 0;

            joint.localPosition = Vector3.Lerp
            (
                joint.localPosition,
                _jointOriginalPos,
                Time.deltaTime * bobSpeed
            );
        }
    }
}
