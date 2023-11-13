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
            Debug.Log("Rod is being swung forwards!");
            FishFloat.SetActive(true);

            // Move the FishFloat to this.position with an offset of <X> on the y axis
            Vector3 fishFloatPosition = transform.position + new Vector3(0, 1f, 0);
            FishFloat.transform.position = fishFloatPosition;

            // Add a forward force to the FishFloat relative to the rod's forward direction at the speed of the float CastSpeed
            Vector3 castForce = transform.forward * castSpeed;
            FishFloat.GetComponent<Rigidbody>().AddForce(castForce, ForceMode.Impulse);

            // Add an upwards force to FishFloat at a 45-degree angle
            Vector3 upwardsForce = Quaternion.Euler(45, 0, 0) * transform.up * upWardsSpeed;
            FishFloat.GetComponent<Rigidbody>().AddForce(upwardsForce);

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
            Debug.Log("Rod is being swung backwards!");

            FishFloat.SetActive(false);
            isFishFloatActive = false;
        }
    }

    private void OnFloatReturn()
    {
        // Set FishFloat to inactive
        FishFloat.SetActive(false);

        // Set isFishFloat bool to false
        isFishFloatActive = false;
    }
}
