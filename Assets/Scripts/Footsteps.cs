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
    public string jumpParameterName = "Jump"; // Зміни в Інспекторі, якщо параметр називається інакше

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

        if (Input.GetKeyDown(KeyCode.Space) && isCurrentlyGrounded)
        {
            // Передаємо івент стрибка та його власну назву параметра
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

                // Передаємо івент кроків та його власну назву параметра
                PlaySurfaceEvent(footstepsEvent, footstepsParameterName);
            }
        }
    }

    // Тепер функція приймає назву параметра як аргумент (parameterName)
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

                // Використовуємо динамічну назву параметра
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