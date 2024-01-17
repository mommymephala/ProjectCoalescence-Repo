using System.Collections;
using System.Collections.Generic;
using HorrorEngine;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void PauseMenuActive()
    {
        PauseController.Instance.Pause();
        
    }
    public void PauseMenuClosed()
    {
        PauseController.Instance.Resume();
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
