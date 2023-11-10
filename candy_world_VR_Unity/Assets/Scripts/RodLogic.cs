using UnityEngine;

public class RodLogic : MonoBehaviour
{
    // Public references
    public GameObject FishFloat;
    public Transform PlayerTransform;
    public float swingSpeed;
    public float pullSpeed;

    // Internal variables
    private Vector3 previousRodPosition;
    private bool isFishFloatActive;

    // Offset for FishFloat position (set in the inspector)
    public Vector3 fishFloatOffset;

    void Start()
    {
        // Initialize previousRodPosition with the initial position of the rod
        previousRodPosition = transform.position;
        // Ensure FishFloat is initially disabled
        FishFloat.SetActive(false);
    }

    void Update()
    {
        // Calculate the speed of the rod
        float rodSpeed = (transform.position - previousRodPosition).magnitude / Time.deltaTime;

        // Check for forward motion and enable FishFloat
        if (rodSpeed >= swingSpeed)
        {
            EnableFishFloat();
        }
        // Check for backward motion and move FishFloat to PlayerTransform
        else if (isFishFloatActive && rodSpeed >= pullSpeed)
        {
            MoveFishFloatToPlayer();
        }

        // Update previousRodPosition
        previousRodPosition = transform.position;
    }

    void EnableFishFloat()
    {
        // Enable FishFloat
        FishFloat.SetActive(true);

        // Calculate the velocity
        Vector3 forwardVelocity = transform.forward * swingSpeed;
        Vector3 upwardVelocity = transform.up * swingSpeed * 0.5f; // 45-degree upwards velocity

        // Set FishFloat position and velocity
        FishFloat.transform.position = transform.position + fishFloatOffset;
        FishFloat.GetComponent<Rigidbody>().velocity = forwardVelocity + upwardVelocity;
    }

    void MoveFishFloatToPlayer()
    {
        // Move FishFloat to PlayerTransform
        FishFloat.transform.position = PlayerTransform.position;

        // Check if FishFloat has reached the PlayerTransform
        if (Vector3.Distance(FishFloat.transform.position, PlayerTransform.position) < 0.1f)
        {
            // Disable FishFloat
            FishFloat.SetActive(false);
            isFishFloatActive = false;
        }
    }
}
