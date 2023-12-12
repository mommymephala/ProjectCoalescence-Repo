using UnityEngine;

public class BotanicRoomTrigger : MonoBehaviour
{
    public GameObject botanicHallwaytoUpstairs;
    public GameObject labHallwaytoUpstairs;
    public GameObject saveRoom;
    public GameObject darkHallway;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        saveRoom.SetActive(false);
        darkHallway.SetActive(false);
        botanicHallwaytoUpstairs.SetActive(true);
        labHallwaytoUpstairs.SetActive(true);
        //gameObject.SetActive(false);
    }
}
