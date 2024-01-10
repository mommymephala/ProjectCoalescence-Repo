using UnityEngine;

public class AudioManagerSpawn : MonoBehaviour
{
    public GameObject audioManagerObject;

    private void Awake()
    {
        if (FindObjectOfType<AudioManager>() == null)
        {
            Instantiate(audioManagerObject);
        }
    }
}