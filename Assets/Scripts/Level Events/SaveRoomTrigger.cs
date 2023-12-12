using UnityEngine;

public class SaveRoomTrigger : MonoBehaviour
{
    public GameObject botanicHallwaytoUpstairs;
    public GameObject labHallwaytoUpstairs;
    public GameObject saveRoom;
    public GameObject darkHallway;
    
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        botanicHallwaytoUpstairs.SetActive(false);
        labHallwaytoUpstairs.SetActive(false);
        saveRoom.SetActive(true);
        darkHallway.SetActive(true);
        //gameObject.SetActive(false);
    }
}
