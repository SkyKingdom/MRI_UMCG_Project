using UnityEngine;
using System.Collections;

public class Fishing : MonoBehaviour
{
    public FishAI fishAI;
    public GameObject ScoreScale; // Reference to the ScoreScale GameObject

    private Transform objTransform; // Reference to the object's transform
    private Vector3 lastPosition; // Stores the last position of the object
    private float speed; // Stores the speed of the object

    public GameObject hookPrefab; // Reference to the hook prefab to be instantiated
    public float hookSpeed = 10f; // Speed at which the hook is shot
    public float zOffset = 0f; // Offset on the Z-axis
    public float yOffset = 0f; // Offset on the Y-axis
    public float hookReturnSpeed = 10f; // Constant speed at which the hook returns
    public Transform playerTransform; // Reference to the player's transform

    public float scoreScale = 50f; // Public scoreScale variable
    public float scoreScaleSpeed = 5f; // Speed multiplier for fluctuating scoreScale
    public float minScoreScale = 0f; // Minimum value for scoreScale
    public float maxScoreScale = 100f; // Maximum value for scoreScale
    private float scoreScaleDirection = 1f; // Direction of scoreScale change
    public int rodSwingSpeed = 7;

    private GameObject currentHook; // Reference to the currently active hook
    public GameObject fishAIController; // Reference to the FishAI controller GameObject

    private bool isReturning = false; // Flag to track if the hook is returning
    private bool updateScoreScale = true; // Flag to control updating of scoreScale
    public bool isFishCaught = false; //flag to check if a fish has been caught (referenced in FishCaught.cs)

    public Transform FishLineStart;

    void Start()
    {
        // Get the object's transform
        objTransform = transform;

        // Initialize the last position to the current position
        lastPosition = objTransform.position;

        ScoreScale.SetActive(false);

        // Start fluctuating the scoreScale
        StartCoroutine(FluctuateScoreScale());
    }

    void Update()
    {
        if (isFishCaught)
        {

        }
        // Calculate the speed based on the change in Z-position over time
        float deltaZ = objTransform.position.z - lastPosition.z;
        float deltaTime = Time.deltaTime;

        // Calculate speed (magnitude of change in position divided by time)
        speed = Mathf.Abs(deltaZ / deltaTime);

        // Update the last position
        lastPosition = objTransform.position;

        // Check if there is no current hook, the rod is being swung forward (positive Z-axis movement), and the speed is above a threshold
        if (currentHook == null && deltaZ > 0 && speed >= rodSwingSpeed)
        {
            CastHook();
        }

        // Check if there is a current hook, it's not already returning, and the rod is pulled backward (negative Z-axis movement) and the speed is above a threshold
        if (currentHook != null && !isReturning && deltaZ < 0 && speed >= rodSwingSpeed)
        {
            isReturning = true; // Set the returning flag
            updateScoreScale = false; // Stop updating scoreScale when returning hook
        }

        // Check if the hook is currently returning
        if (isReturning)
        {
            ReturnHook();
        }

        // Find the FishMovement script on all objects with the tag "Fish"
        GameObject[] fishObjects = GameObject.FindGameObjectsWithTag("Fish");
        foreach (var fishObject in fishObjects)
        {
            FishMovement fishMovement = fishObject.GetComponent<FishMovement>();
            if (fishMovement != null)
            {
                // When CastHook is called, set ifHookActive to true
                if (currentHook != null)
                {
                    fishMovement.ifHookActive = true;
                }
                // When ReturnHook is called, set ifHookActive to false
                else
                {
                    fishMovement.ifHookActive = false;
                }
            }
        }

        // Rotate the ScoreScale GameObject based on the scoreScale value
        if (updateScoreScale)
        {
            float targetRotation = Mathf.Lerp(0f, -180f, scoreScale / 100f);
            ScoreScale.transform.rotation = Quaternion.Euler(0f, 0f, targetRotation);
        }
    }

    void CastHook()
    {
        if (hookPrefab != null)
        {
            // Calculate the direction for the hook (45-degree angle upwards in world space)
            Vector3 hookDirection = objTransform.forward + Vector3.up * Mathf.Tan(Mathf.Deg2Rad * 45);

            // Normalize the direction to ensure it has a length of 1
            hookDirection.Normalize();

            // Calculate the initial position for the hook with offsets
            Vector3 hookPosition = objTransform.position + objTransform.right * zOffset + objTransform.up * yOffset;

            // Instantiate the hook prefab at the calculated position
            currentHook = Instantiate(hookPrefab, hookPosition, Quaternion.identity);
            currentHook.GetComponent<FishingLine>().startPoint = FishLineStart;
            Rigidbody hookRigidbody = currentHook.GetComponent<Rigidbody>();

            if (hookRigidbody != null)
            {
                // Apply an initial velocity to the hook
                hookRigidbody.velocity = hookDirection * hookSpeed;
            }

            // Send a message to the FishAI script on the fishAIController GameObject to disable the speed tracker
            if (fishAIController != null)
            {
                FishAI fishAIScript = fishAIController.GetComponent<FishAI>();
                if (fishAIScript != null)
                {
                    fishAIScript.EnablespeedPointer();
                }
            }
        }
    }

    void ReturnHook()
    {
        if (currentHook != null && playerTransform != null)
        {
            // Calculate the direction for the hook to return to the player
            Vector3 returnDirection = (playerTransform.position - currentHook.transform.position).normalized;

            // Apply a constant velocity to the hook to make it return
            Rigidbody hookRigidbody = currentHook.GetComponent<Rigidbody>();

            if (hookRigidbody != null)
            {
                hookRigidbody.velocity = returnDirection * hookReturnSpeed;
            }

            // Check if the hook is within 1 unit of the player
            if (Vector3.Distance(currentHook.transform.position, playerTransform.position) < 1f)
            {
                // Destroy the hook
                foreach (Transform child in currentHook.transform)
                    if (child.tag == "Fish")
                    {
                        fishAI.activeFishCount--;
                    }
                Destroy(currentHook);
                if (fishAIController != null)
                {
                    FishAI fishAIScript = fishAIController.GetComponent<FishAI>();
                    if (fishAIScript != null)
                    {
                        fishAIScript.DisablespeedPointer();
                    }
                }
                currentHook = null;
                isReturning = false; // Reset the returning flag
                GameObject fishingRod = GameObject.Find("Fishing rod");
                if (fishingRod != null)
                {
                    Fishing fishingScript = fishingRod.GetComponent<Fishing>();
                    if (fishingScript != null)
                    {
                        // Deactivate the ScoreScale GameObject in the Fishing script
                        fishingScript.ScoreScale.SetActive(false);
                    }
                }
                updateScoreScale = true; // Resume updating scoreScale
            }
        }

    }

    // Coroutine to fluctuate the scoreScale
    private IEnumerator FluctuateScoreScale()
    {
        while (true)
        {
            // Change the scoreScale direction when reaching the minimum or maximum value
            if (scoreScale <= minScoreScale || scoreScale >= maxScoreScale)
            {
                scoreScaleDirection *= -1f;
            }

            // Update the scoreScale based on the direction and speed
            scoreScale += scoreScaleDirection * scoreScaleSpeed * Time.deltaTime;

            // Clamp scoreScale to ensure it stays within the 0-100 range
            scoreScale = Mathf.Clamp(scoreScale, minScoreScale, maxScoreScale);

            yield return null;
        }
    }
}