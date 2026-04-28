using UnityEngine;
using FMODUnity;

/// <summary>
/// Zarządza odtwarzaniem dźwięków kroków, skoków i lądowania. 
/// Zoptymalizowano dla CharacterController oraz ignorowania Triggerów.
/// </summary>
public class Footsteps : MonoBehaviour
{
    [Header("Steps Settings (Footsteps)")]
    public EventReference footstepsEvent;
    [Tooltip("Parameter name in FMOD for steps (ex: Steps)")]
    public string footstepsParameterName = "Steps";

    [Header("Jump Settings (Jump)")]
    public EventReference jumpEvent;
    [Tooltip("Parameter name in FMOD for jumps")]
    public string jumpParameterName = "Jump";

    private float lastFootstepTime = 0f;
    private float distToGround;

    private CharacterController controller;
    private bool wasGrounded = true;

    void Start()
    {
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

    void Update()
    {
        HandleJumpAndLand();
    }

    void FixedUpdate()
    {
        HandleFootsteps();
    }

    private void HandleJumpAndLand()
    {
        bool isCurrentlyGrounded = IsGrounded();

        // FIX: We use 'wasGrounded' instead of 'isCurrentlyGrounded'.
        // This ensures the sound plays even if the FPS Controller has already 
        // started lifting the player off the ground in this exact frame.
        if (Input.GetKeyDown(KeyCode.Space) && wasGrounded)
        {
            PlaySurfaceEvent(jumpEvent, jumpParameterName);
        }

        wasGrounded = isCurrentlyGrounded;
    }

    private void HandleFootsteps()
    {
        bool isMoving = (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0);
        bool isRunning = Input.GetKey(KeyCode.LeftShift);

        if (isMoving && IsGrounded())
        {
            float footstepInterval = isRunning ? 0.25f : 0.5f;

            if (Time.time - lastFootstepTime > footstepInterval)
            {
                lastFootstepTime = Time.time;
                PlaySurfaceEvent(footstepsEvent, footstepsParameterName);
            }
        }
    }
    private void PlaySurfaceEvent(EventReference eventRef, string parameterName)
    {
        if (eventRef.IsNull) return;

        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, distToGround + 0.5f, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore))
        {
            string surfaceTag = hit.collider.tag;
            string surfaceParameterValue = null;

            switch (surfaceTag)
            {
                case "Stone":
                case "Inside_stone":
                case "Outside":
                    surfaceParameterValue = "Stone";
                    break;

                case "Wood":
                case "Inside_wood":
                    surfaceParameterValue = "Wood";
                    break;

                case "Bed":
                    surfaceParameterValue = "Bed";
                    break;
                case "Stairs":
                    surfaceParameterValue = "Stairs";
                    break;
            }

            if (surfaceParameterValue != null)
            {
                var soundInstance = RuntimeManager.CreateInstance(eventRef);
                soundInstance.set3DAttributes(RuntimeUtils.To3DAttributes(gameObject.transform));

                soundInstance.setParameterByNameWithLabel(parameterName, surfaceParameterValue);
                soundInstance.start();
                soundInstance.release();
            }
        }
    }

    bool IsGrounded()
    {
        if (controller != null)
        {
            return controller.isGrounded;
        }
        return Physics.Raycast(transform.position, Vector3.down, distToGround + 0.5f, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore);
    }
}