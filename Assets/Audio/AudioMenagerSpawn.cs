using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioMenagerSpawn : MonoBehaviour
{
    public Object audioManagerObject;

    private void Awake()
    {
        if (FindObjectOfType<AudioMenager>() == null)
        {
            Instantiate(audioManagerObject);
        }
    }
}