using System.Collections;
using UnityEngine;

public class RodLogic : MonoBehaviour
{
    public GameObject FishFloat;
    public Transform PlayerTransform;
    public float swingSpeed;
    public float pullSpeed;
    public float castSpeed;
    public float retractSpeed;
    public float upWardsSpeed;

    private bool isFishFloatActive = false;
    private Vector3 previousPosition;

    void Start()
    {
        FishFloat.SetActive(false);
        previousPosition = transform.position;
    }

    void Update()
    {
        CheckSwing();
    }

    private void CheckSwing()
    {
        Vector3 deltaPosition = transform.position - previousPosition;
        float speed = Vector3.Dot(deltaPosition, transform.forward) / Time.deltaTime;

        if (speed > 0)
        {
            OnSwingForwards(speed);
        }
        else if (speed < 0)
        {
            OnSwingBackwards(speed);
        }

        previousPosition = transform.position;
    }

    private void OnSwingForwards(float speed)
    {
        if (Mathf.Abs(speed) >= swingSpeed && !isFishFloatActive)
        {
            FishFloat.SetActive(true);

            // Move the FishFloat to this.position with an offset of <X> on the y axis
            Vector3 fishFloatPosition = transform.position + new Vector3(0, 1f, 0);
            FishFloat.transform.position = fishFloatPosition;

            // Add a forward force to the FishFloat relative to the rod's forward direction at the speed of the float CastSpeed
            Vector3 castForce = transform.forward * castSpeed;
            FishFloat.GetComponent<Rigidbody>().AddForce(castForce, ForceMode.Impulse);

            // Add an upwards force to FishFloat
            Vector3 upwardsForce = transform.up * upWardsSpeed; // Adjust the direction if needed
            FishFloat.GetComponent<Rigidbody>().AddForce(upwardsForce, ForceMode.Impulse);

            // Set isFishFloat bool to true
            isFishFloatActive = true;
        }

        // Limit the max velocity of the FishFloat to the castSpeed
        Rigidbody fishFloatRigidbody = FishFloat.GetComponent<Rigidbody>();
        fishFloatRigidbody.velocity = Vector3.ClampMagnitude(fishFloatRigidbody.velocity, castSpeed);
    }

    private void OnSwingBackwards(float speed)
    {
        if (Mathf.Abs(speed) >= pullSpeed)
        {
            StartCoroutine(FloatReturnCoroutine()); // Start the coroutine to move the FishFloat back
        }
    }

    // Coroutine to gradually move the FishFloat back to the player transform
    private IEnumerator FloatReturnCoroutine()
    {
        // Calculate the direction from FishFloat to the player
        Vector3 returnDirection = (PlayerTransform.position - FishFloat.transform.position).normalized;

        // Continue moving the FishFloat until it reaches the player
        while (Vector3.Distance(FishFloat.transform.position, PlayerTransform.position) > 0.1f)
        {
            // Calculate the movement step
            float step = retractSpeed * Time.deltaTime;

            // Move the FishFloat towards the player
            FishFloat.transform.position = Vector3.MoveTowards(FishFloat.transform.position, PlayerTransform.position, step);

            yield return null; // Wait for the next frame
        }

        // After reaching the player, call the method to handle the FishFloat return
        OnFloatReturn();
    }

    // Method to handle the FishFloat return
    private void OnFloatReturn()
    {
        // Set FishFloat to inactive
        FishFloat.SetActive(false);

        // Set isFishFloat bool to false
        isFishFloatActive = false;
    }
}
