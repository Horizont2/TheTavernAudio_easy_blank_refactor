using FMODUnity;
using System.Collections;
using UnityEngine;

public class Doors : MonoBehaviour, IInteractable
{
    public float rotationDuration = 1f;

    [Header("Room Settings")]
    [Tooltip("Move here object of the room (with trigger collider and RoomAmbient script), where this door takes to")]
    public RoomAmbient connectedRoom;

    [SerializeField] private bool doorsOpened = true;
    [SerializeField] private bool isRotating = false;

    private FMOD.Studio.EventInstance doorsSoundInstance;
    public EventReference doorsEvent;

    private FMOD.Studio.EventInstance insideRoomSnapshot;
    public EventReference insideRoomSnap;

    public void Interact()
    {
        if (!isRotating)
        {
            doorsOpened = !doorsOpened;
            StartCoroutine(RotateDoors(doorsOpened ? -65 : 65));
            PlaySound();
            RoomsSnap();
        }
    }

    private IEnumerator RotateDoors(float targetAngle)
    {
        isRotating = true;
        float elapsedTime = 0f;
        Quaternion startRotation = transform.rotation;
        Quaternion targetRotation = startRotation * Quaternion.Euler(0, targetAngle, 0);

        while (elapsedTime < rotationDuration)
        {
            transform.rotation = Quaternion.Lerp(startRotation, targetRotation, elapsedTime / rotationDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.rotation = targetRotation;
        isRotating = false;
    }

    private void PlaySound()
    {
        if (doorsSoundInstance.isValid())
        {
            doorsSoundInstance.release();
        }

        doorsSoundInstance = RuntimeManager.CreateInstance(doorsEvent);
        doorsSoundInstance.set3DAttributes(RuntimeUtils.To3DAttributes(gameObject.transform));

        string parameterLabel = doorsOpened ? "Open" : "Close";
        doorsSoundInstance.setParameterByNameWithLabel("Door", parameterLabel);

        doorsSoundInstance.start();
    }

    private void RoomsSnap()
    {
        if (connectedRoom == null) return;

        if (connectedRoom.ambientActivated && doorsOpened)
        {
            if (insideRoomSnapshot.isValid())
            {
                insideRoomSnapshot.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
                insideRoomSnapshot.release();
            }
        }
        else if (connectedRoom.ambientActivated && !doorsOpened)
        {
            insideRoomSnapshot = RuntimeManager.CreateInstance(insideRoomSnap);
            insideRoomSnapshot.start();
        }
    }
}