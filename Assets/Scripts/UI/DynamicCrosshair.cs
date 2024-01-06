
using System;
using ECM.Examples;
using UnityEngine;
using UnityEngine.UI;
using WeaponRelated;

public class DynamicCrosshair : MonoBehaviour 
{
    public Rigidbody playerRigidbody;
    public Weapon weapon1;
    public Weapon weapon2;
    public Weapon weapon3;
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
        if(!weapon1.aimingDownSight)
        {
            if (IsMoving) 
            {
                _currentSize = Mathf.Lerp(_currentSize, maxSize, Time.deltaTime * speed);
            } 
            else 
            {
                _currentSize = Mathf.Lerp(_currentSize, restingSize, Time.deltaTime * speed);
            }
        }
        else
        {
            gameObject.SetActive(false);
        }
        
        if(!weapon2.aimingDownSight)
        {
            if (IsMoving) 
            {
                _currentSize = Mathf.Lerp(_currentSize, maxSize, Time.deltaTime * speed);
            } 
            else 
            {
                _currentSize = Mathf.Lerp(_currentSize, restingSize, Time.deltaTime * speed);
            }
        }
        else
        {
            gameObject.SetActive(false);
        }
        
        if(!weapon3.aimingDownSight)
        {
            if (IsMoving) 
            {
                _currentSize = Mathf.Lerp(_currentSize, maxSize, Time.deltaTime * speed);
            } 
            else 
            {
                _currentSize = Mathf.Lerp(_currentSize, restingSize, Time.deltaTime * speed);
            }
        }
        else
        {
            gameObject.SetActive(false);
        }
        
        _reticle.sizeDelta = new Vector2(_currentSize, _currentSize);
    }

    // Bool to check if player is currently moving.
    private bool IsMoving
    {
        get
        {
            if (playerRigidbody != null)
                if (playerRigidbody.velocity.sqrMagnitude != 0)
                    return true;
                else
                    return false;
            return false;
        }
    }
}

