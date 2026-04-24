using UnityEngine;

/// <summary>
/// Zarządza stanem ambientu pokoju w zależności od pozycji gracza.
/// </summary>
[RequireComponent(typeof(RoomAmbient))]
public class Rooms : MonoBehaviour
{
    private RoomAmbient myRoomAmbient;

    private void Start()
    {
        myRoomAmbient = GetComponent<RoomAmbient>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && myRoomAmbient != null)
        {
            myRoomAmbient.ambientActivated = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && myRoomAmbient != null)
        {
            myRoomAmbient.ambientActivated = false;
        }
    }
}