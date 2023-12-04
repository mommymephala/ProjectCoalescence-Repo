using System.Collections;
using UnityEngine;

public class Door : MonoBehaviour
{
    public bool isOpen;
    
    [SerializeField] private bool isRotatingDoor = true;
    [SerializeField] private float speed = 1f;
    
    [Header("Rotation Configs")]
    [SerializeField] private float rotationAmount = 90f;
    [SerializeField] private float forwardDirection;
    
    [Header("Sliding Configs")] 
    [SerializeField] private Vector3 slideDirection = Vector3.back; 
    [SerializeField] private float slideAmount = 1.9f;

    private Vector3 _startRotation;
    private Vector3 _startPosition;
    private Vector3 _forward;

    private Coroutine _animationCoroutine;

    private void Awake()
    {
        _startRotation = transform.rotation.eulerAngles;
        // Since "Forward" actually is pointing into the door frame, choose a direction to think about as "forward" 
        _forward = transform.right;
        _startPosition = transform.position;
    }

    public void Open(Vector3 userPosition)
    {
        if (isOpen) return;
        if (_animationCoroutine != null)
        {
            StopCoroutine(_animationCoroutine);
        }

        if (isRotatingDoor)
        {
            var dot = Vector3.Dot(_forward, (userPosition - transform.position).normalized);
            Debug.Log($"Dot: {dot:N3}");
            _animationCoroutine = StartCoroutine(DoRotationOpen(dot));
        }
        else
        {
            _animationCoroutine = StartCoroutine(DoSlidingOpen()); 
        }
    }

    private IEnumerator DoRotationOpen(float forwardAmount)
    {
        Quaternion startRotation = transform.rotation;
        Quaternion endRotation = Quaternion.Euler
            (forwardAmount >= forwardDirection ? new Vector3(0, _startRotation.y + rotationAmount, 0) : new Vector3(0, _startRotation.y - rotationAmount, 0));

        isOpen = true;

        float time = 0;
        
        while (time < 1)
        {
            transform.rotation = Quaternion.Slerp(startRotation, endRotation, time);
            yield return null;
            time += Time.deltaTime * speed;
        }
    }

    private IEnumerator DoSlidingOpen()
    {
        Vector3 endPosition = _startPosition + slideAmount * slideDirection;
        Vector3 startPosition = transform.position;

        float time = 0;
        isOpen = true;
        while (time < 1)
        {
            transform.position = Vector3.Lerp(startPosition, endPosition, time);
            yield return null;
            time += Time.deltaTime * speed;
        }
    }

    public void Close()
    {
        if (!isOpen) return;
        if (_animationCoroutine != null)
        {
            StopCoroutine(_animationCoroutine);
        }

        _animationCoroutine = StartCoroutine(isRotatingDoor ? DoRotationClose() : DoSlidingClose());
    }

    private IEnumerator DoRotationClose()
    {
        Quaternion startRotation = transform.rotation;
        Quaternion endRotation = Quaternion.Euler(_startRotation);

        isOpen = false;

        float time = 0;
        while (time < 1)
        {
            transform.rotation = Quaternion.Slerp(startRotation, endRotation, time);
            yield return null;
            time += Time.deltaTime * speed;
        }
    }

    private IEnumerator DoSlidingClose()
    {
        Vector3 endPosition = _startPosition;
        Vector3 startPosition = transform.position;
        float time = 0;

        isOpen = false;

        while (time < 1)
        {
            transform.position = Vector3.Lerp(startPosition, endPosition, time);
            yield return null;
            time += Time.deltaTime * speed;
        }
    }
}