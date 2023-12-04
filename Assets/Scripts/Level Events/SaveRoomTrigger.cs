using UnityEngine;

public class SaveRoomTrigger : MonoBehaviour
{
    public GameObject BotanicHallwaytoUpstairs;
    public GameObject LabHallwaytoUpstairs;
    public GameObject Save_Room;
    public GameObject Dark_Hallway;
    
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        BotanicHallwaytoUpstairs.SetActive(false);
        LabHallwaytoUpstairs.SetActive(false);
        Save_Room.SetActive(true);
        Dark_Hallway.SetActive(true);
        //gameObject.SetActive(false);
    }
}
