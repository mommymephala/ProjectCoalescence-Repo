using UnityEngine;

public class DynamicCrosshair : MonoBehaviour
{
    public Rigidbody playerRigidbody;

    private RectTransform _reticle;
    public float restingSize;
    public float maxSize;
    public float speed;
    private float _currentSize;
    
    private void Awake() 
    {
        _reticle = GetComponent<RectTransform>();
    }
    
    private void Update() 
    {
        // Check if player is currently moving and Lerp currentSize to the appropriate value.
        if (isMoving) {
            _currentSize = Mathf.Lerp(_currentSize, maxSize, Time.deltaTime * speed);
        } else {
            _currentSize = Mathf.Lerp(_currentSize, restingSize, Time.deltaTime * speed);
        }

        // Set the reticle's size to the currentSize value.
        _reticle.sizeDelta = new Vector2(_currentSize, _currentSize);
    }

    // Bool to check if player is currently moving.
    bool isMoving 
    {

        get
        {
            // If we have assigned a rigidbody, check if its velocity is not zero. If so, return true.
            if (playerRigidbody != null)
                if (playerRigidbody.velocity.sqrMagnitude != 0)
                    return true;
                else
                    return false;
            return false;
        }
    }
}
