using UnityEngine;
using FMODUnity;

/// <summary>
/// Zarządza aktywacją snapshotu 'Outside' na podstawie tagu powierzchni, na której znajduje się gracz.
/// </summary>
public class Outside_foot_switch : MonoBehaviour
{
    [SerializeField]
    private bool snapshotActivated = false;

    private float distToGround;
    private CharacterController controller;

    private FMOD.Studio.EventInstance outsideSnapshotInstance;
    public EventReference outsideSnapshot;

    void Start()
    {
        // Safely get the distance to ground using CharacterController
        controller = GetComponent<CharacterController>();
        if (controller != null)
        {
            distToGround = controller.bounds.extents.y;
        }
        else
        {
            distToGround = GetComponent<Collider>().bounds.extents.y;
        }
    }

    void FixedUpdate()
    {
        ToggleSnapshotLogic();
    }

    private void ToggleSnapshotLogic()
    {
        // FIX: Added QueryTriggerInteraction.Ignore so the raycast doesn't get blocked by Room triggers
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, distToGround + 0.5f, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore))
        {
            string currentTag = hit.collider.tag;

            if (currentTag == "Outside" && !snapshotActivated)
            {
                ToggleSnapshot(true);
            }
            // I also added standard "Wood" and "Stone" tags here just in case your tavern floors 
            // use those tags instead of the "Inside_" specific ones.
            else if ((currentTag == "Inside_stone" || currentTag == "Inside_wood" || currentTag == "Wood" || currentTag == "Stone") && snapshotActivated)
            {
                ToggleSnapshot(false);
            }
        }
    }

    private void ToggleSnapshot(bool activate)
    {
        // Safety check: Prevent errors if you haven't assigned the snapshot in the Inspector yet
        if (outsideSnapshot.IsNull) return;

        if (activate)
        {
            outsideSnapshotInstance = RuntimeManager.CreateInstance(outsideSnapshot);
            outsideSnapshotInstance.start();
        }
        else
        {
            if (outsideSnapshotInstance.isValid())
            {
                outsideSnapshotInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
                outsideSnapshotInstance.release();
            }
        }
        snapshotActivated = activate;
    }
}