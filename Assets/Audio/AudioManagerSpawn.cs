using UnityEngine;

public class AudioManagerSpawn : MonoBehaviour
{
    public Object audioManagerObject;

    private void Awake()
    {
        if (FindObjectOfType<AudioManager>() == null)
        {
            Instantiate(audioManagerObject);
        }
    }
}