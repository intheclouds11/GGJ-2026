using FMODUnity;
using UnityEngine;
using static Unity.Cinemachine.CinemachineCore;



public class FootstepPlayer : MonoBehaviour
{
    [Header("FMOD Events")]
    [SerializeField] private EventReference footstepEvent;
    [SerializeField] private EventReference landEvent;

    [Header("Ground Check")]
    [SerializeField] private float groundCheckDistance = 0.3f;
    [SerializeField] private LayerMask groundLayer;

    [Header("Movement")]
    [SerializeField] private float stepInterval = 0.5f;

    Rigidbody rb;
    Collider col;

    float stepTimer;
    bool wasGrounded;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
    }

    void Update()
    {
        bool groundedNow = IsGrounded();

        // Landing sound
        if (!wasGrounded && groundedNow)
        {
            RuntimeManager.PlayOneShot(landEvent, transform.position);
        }

        // Footsteps
        if (!groundedNow || rb.linearVelocity.magnitude < 0.1f)
        {
            stepTimer = 0f;
        }
        else
        {
            stepTimer -= Time.deltaTime;
            if (stepTimer <= 0f)
            {
                RuntimeManager.PlayOneShot(footstepEvent, transform.position);
                stepTimer = stepInterval;
            }
        }

        wasGrounded = groundedNow;
    }

    bool IsGrounded()
    {
        Vector3 origin = col.bounds.center;
        float distance = col.bounds.extents.y + groundCheckDistance;

        return Physics.Raycast(
            origin,
            Vector3.down,
            distance,
            groundLayer
        );
    }
}
