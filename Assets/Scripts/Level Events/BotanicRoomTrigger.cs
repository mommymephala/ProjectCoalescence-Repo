using UnityEngine;

public class BotanicRoomTrigger : MonoBehaviour
{
    public GameObject BotanicHallwaytoUpstairs;
    public GameObject LabHallwaytoUpstairs;
    public GameObject Save_Room;
    public GameObject Dark_Hallway;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        Save_Room.SetActive(false);
        Dark_Hallway.SetActive(false);
        BotanicHallwaytoUpstairs.SetActive(true);
        LabHallwaytoUpstairs.SetActive(true);
        //gameObject.SetActive(false);
    }
}
