using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnableEnemyAI : BaseEnemyAI
{
   
    
    protected override void SpawnBehavior()
    {
        if (IsSpawn) 
        {
            Debug.Log("spawntrigger babba");
            //_animator.SetBool("IsSpawn", IsSpawn);
            _animator.SetTrigger("Spawn Trigger" );
            //_animator.Play(spawnAnimation.name);
            currentState = State.InitialSpawn;
        }
        else
        {
            currentState = State.Idling;
        }
    }
    
    private void FixedUpdate()
    {
        Debug.Log(IsSpawn);
        _animator.SetBool("IsSpawn", IsSpawn);
        Debug.Log(currentState);
    }

    private void EndSpawnAnimation()
    {
        Debug.Log("spawntrigger bitti babba");
        // This method should be called at the end of the spawn animation.
       
        IsSpawn = false;
        _animator.SetBool("IsSpawn", IsSpawn);
        currentState = State.Idling;
        _agent.isStopped = false;
        Debug.Log(IsSpawn);
    }

}
