using System.Collections;
using UnityEngine;

public class RodLogic : MonoBehaviour
{
    [SerializeField] private GameObject fishFloat;
    [SerializeField] private Transform playerTransform;

    [SerializeField] private float swingSpeed;
    [SerializeField] private float pullSpeed;
    [SerializeField] private float castSpeed;
    [SerializeField] private float retractSpeed;
    [SerializeField] private float upwardsSpeed;

    // Reference to FishFloat script
    [SerializeField] private FishFloat fishFloatScript;

    private bool isFishFloatActive;
    private bool isSwingingBackwards;
    private Vector3 previousPosition;

    void Start()
    {
        SetFishFloatActive(false);
        previousPosition = transform.position;

        FishFloat fishFloat = GetComponent<FishFloat>();
    }

    void Update()
    {
        CheckSwing();
    }

    private void CheckSwing()
    {
        Vector3 deltaPosition = transform.position - previousPosition;
        float speed = Vector3.Dot(deltaPosition, transform.forward) / Time.deltaTime;

        float rodXRotation = transform.rotation.eulerAngles.x;

        // Check if the object is rotated more than 30 and lower than 180 degrees on the x-axis
        if (speed > 0 && (rodXRotation > 30f && rodXRotation < 180f))
        {
            OnSwingForwards(speed);
        }
        else if (speed < 0 && !isSwingingBackwards)
        {
            OnSwingBackwards(speed);
        }

        previousPosition = transform.position;
    }


    private void OnSwingForwards(float speed)
    {
        if (Mathf.Abs(speed) >= swingSpeed && !isFishFloatActive)
        {
            SetFishFloatActive(true);

            Vector3 fishFloatPosition = transform.position + new Vector3(0, 1f, 0);
            fishFloat.transform.position = fishFloatPosition;

            ApplyForceToFishFloat(castSpeed * transform.forward, ForceMode.Impulse);
            ApplyForceToFishFloat(upwardsSpeed * transform.up, ForceMode.Impulse);
        }
    }

    private void OnSwingBackwards(float speed)
    {
        if (Mathf.Abs(speed) >= pullSpeed)
        {
            isSwingingBackwards = true;
            StartCoroutine(FloatReturnCoroutine());
            ApplyForceToFishFloat(upwardsSpeed * transform.up, ForceMode.Impulse);
            fishFloatScript.OnSwingBackwards();
        }
    }

    private void ApplyForceToFishFloat(Vector3 force, ForceMode mode)
    {
        fishFloat.GetComponent<Rigidbody>().AddForce(force, mode);
    }

    private void SetFishFloatActive(bool isActive)
    {
        isFishFloatActive = isActive;
        fishFloat.SetActive(isActive);
    }

    private IEnumerator FloatReturnCoroutine()
    {
        Vector3 returnDirection = (playerTransform.position - fishFloat.transform.position).normalized;

        while (Vector3.Distance(fishFloat.transform.position, playerTransform.position) > 0.5f)
        {
            float step = retractSpeed * Time.deltaTime;
            fishFloat.transform.position = Vector3.MoveTowards(fishFloat.transform.position, playerTransform.position, step);
            yield return null;
        }

        OnFloatReturn();
    }

    private void OnFloatReturn()
    {
        Rigidbody fishFloatRigidbody = fishFloat.GetComponent<Rigidbody>();
        fishFloatRigidbody.velocity = Vector3.zero;
        fishFloatRigidbody.angularVelocity = Vector3.zero;

        SetFishFloatActive(false);
        isFishFloatActive = false;
        isSwingingBackwards = false;
    }
}
