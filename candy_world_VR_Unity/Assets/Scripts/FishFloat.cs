using System.Collections;
using UnityEngine;

public class FishFloat : MonoBehaviour
{
    [SerializeField] private GameObject Fish;
    [SerializeField] private Transform Player;

    // Reference to BoatController script
    [SerializeField] private BoatController boatController;

    // Get the transform of the HMD and controllers so we can track their movement
    [SerializeField] private Transform HMD;
    [SerializeField] private Transform Controller_r;
    [SerializeField] private Transform Controller_l;

    // Reference to the movement scale so we can use the before mentioned players movement to display this visually
    [SerializeField] private GameObject MovementScaleFrame;
    [SerializeField] private Transform MovementScale;

    // No longer used
    [SerializeField] private GameObject FishingScaleFrame;
    [SerializeField] private Transform FishingScale;

    [SerializeField] private float MovementScaleSmoothing;
    [SerializeField] private float minMovementScale;
    [SerializeField] private float maxMovementScale;

    [SerializeField] private float warningSpeed;
    [SerializeField] private float illigalSpeed;

    [SerializeField] private float fishingScaleSpeed;

    private float HMD_Speed;
    private float controller_r_Speed;
    private float controller_l_Speed;
    private float playerSpeed;

    private Vector3 hmdPreviousPosition;
    private Vector3 controllerRPreviousPosition;
    private Vector3 controllerLPreviousPosition;

    [SerializeField] private float fishSpawnDistance;
    [SerializeField] private float fishSpeed;
    [SerializeField] private float biteDistance;
    [SerializeField] private float catchDistance;

    private bool isFishActive;
    private bool isFishScared;
    private bool hasFishBit;
    private bool isFishCarryingPicture;

    // Yet to be used (now obsolete)
    private bool isMovementScaleActive;
    private bool isFishingScaleActive;

    private Vector3 previousPosition;
    private float timeSinceLastMove;

    void Start()
    {
        Fish.gameObject.SetActive(false);
        isFishActive = false;
        hasFishBit = false;
        isFishScared = false;

        // Find the boat controller script so we can access it within this script
        BoatController boatController = GetComponent<BoatController>();

        // Initialize the previous position
        previousPosition = transform.position;
        timeSinceLastMove = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        //debug
        if (Input.GetKey(KeyCode.Space))
        {
            print("pressing space");
            boatController.MoveBoat();
        }

        // Track speeds of devices
        HMD_Speed = CalculateSpeed(HMD, ref hmdPreviousPosition);
        controller_r_Speed = CalculateSpeed(Controller_r, ref controllerRPreviousPosition);
        controller_l_Speed = CalculateSpeed(Controller_l, ref controllerLPreviousPosition);

        // Calculate the maximum speed among the three devices
        playerSpeed = CalculateMaxSpeed();

        // Calculate the percentage based on maxSpeed
        float speedPercentage = Mathf.InverseLerp(minMovementScale, maxMovementScale, playerSpeed);

        // Rotate the MovementScale based on the percentage with smoothing
        Quaternion localTargetRotation = Quaternion.Euler(0f, 0f, Mathf.Lerp(0f, -180f, speedPercentage));
        Quaternion worldTargetRotation = MovementScale.parent.rotation * localTargetRotation;
        MovementScale.rotation = Quaternion.Slerp(MovementScale.rotation, worldTargetRotation, Time.deltaTime * MovementScaleSmoothing);


        OnStopDrifting();
        MoveFishToFloat();

        // When the fish is within catchdistance + 0.5 call OnFishCatch(), we add 0.2 so the catch is registered before FishReset() is called
        if (Vector3.Distance(Fish.transform.position, Player.transform.position) < catchDistance + 0.5)
        {
            OnFishCatch();
        }

        if (Vector3.Distance(transform.position, Player.transform.position) < catchDistance)
        {
            FishReset();

            // Disable Movment scale objects
            MovementScaleFrame.SetActive(false);
        }
    }

    private float CalculateSpeed(Transform targetTransform, ref Vector3 previousPosition)
    {
        // Calculate speed based on the change in position over time
        float speed = Vector3.Distance(targetTransform.position, previousPosition) / Time.deltaTime;

        // Update the previous position
        previousPosition = targetTransform.position;

        return speed;
    }

    private float CalculateMaxSpeed()
    {
        // Calculate the maximum speed among the three devices
        float maxSpeed = Mathf.Max(HMD_Speed, controller_r_Speed, controller_l_Speed);

        return maxSpeed;
    }

    private void OnStopDrifting()
    {
        // Check if either the x or z position has not changed for longer than 1 second
        if (Mathf.Approximately(transform.position.x, previousPosition.x) && Mathf.Approximately(transform.position.z, previousPosition.z))
        {
            timeSinceLastMove += Time.deltaTime;

            if (timeSinceLastMove > 1f)
            {
                StartFish();
            }
        }
        else
        {
            // Reset the timer if there is any movement
            timeSinceLastMove = 0f;
        }

        // Update the previous position
        previousPosition = transform.position;
    }

    void StartFish()
    {
        if (isFishActive == false)
        {
            Fish.gameObject.SetActive(true);

            // Move the Fish to a random spot around this object on the horizontal axis at a specified distance
            float randomAngle = Random.Range(0f, 360f);
            Vector3 randomDirection = Quaternion.Euler(0, randomAngle, 0) * Vector3.forward;
            Vector3 spawnPosition = transform.position + randomDirection * fishSpawnDistance;
            Fish.transform.position = new Vector3(spawnPosition.x, transform.position.y, spawnPosition.z);

            // Enable the movement scale objects
            MovementScaleFrame.SetActive(true);
        }
        isFishActive = true;
    }

    void MoveFishToFloat()
    {
        // If the fish is within a specified distance (biteDistance) from this object, set hasFishBit to true
        if (Vector3.Distance(Fish.transform.position, transform.position) < biteDistance)
        {
            hasFishBit = true;
        }
        if (hasFishBit && isFishActive)
        {
            OnFishBite();
        }
        if (playerSpeed < warningSpeed)
        {
            if (isFishActive)
            {
                if (!hasFishBit)
                {
                    if (!isFishScared)
                    {
                        // Move the Fish towards this object at a set speed (fishSpeed)
                        Fish.transform.position = Vector3.MoveTowards(Fish.transform.position, transform.position, fishSpeed * Time.deltaTime);

                        // The fish should always face the direction in which it is moving
                        Fish.transform.LookAt(transform.position);
                    }
                }
            }
        }
        else if (playerSpeed >= warningSpeed && playerSpeed < illigalSpeed)
        {
            return;
        }
        else if (playerSpeed >= illigalSpeed && !hasFishBit)
        {
            if (!isFishScared)
            {
                // Disable Movment scale objects
                MovementScaleFrame.SetActive(false);

                StartCoroutine(ScareFish());
            }
            isFishScared = true;
        }
    }

    void OnFishBite()
    {
        // Make the Fish a child of this object
        Fish.transform.parent = transform;

        // Disable Movement scale objects
        MovementScaleFrame.SetActive(false);

        // Enable the FishingScale object (No longer used)
        //FishingScaleFrame.SetActive(true);

        // Calculate the new x position based on the sine of Time.time multiplied by fishingScaleSpeed (No longer used)
        //float newX = Mathf.Sin(Time.time * fishingScaleSpeed);

        // Map the sine value from [-1, 1] to your desired range [-1, 1] (No longer used)
        //newX = Mathf.Lerp(-1f, 1f, (newX + 1f) / 2f);

        // Set the new local position of the FishingScale relative to its parent (No longer used)
        //FishingScale.localPosition = new Vector3(newX, FishingScale.localPosition.y, FishingScale.localPosition.z);
    }

    public void OnSwingBackwards()
    {

    }

    void OnFishCatch()
    {
        // Execute the MoveBoat function in Boat > BoatController.cs
        boatController.MoveBoat();

        FishReset();
    }

    void FishReset()
    {
        Fish.transform.parent = null;
        Fish.transform.position = new Vector3(0, 0, 0);
        Fish.SetActive(false);
        // No longer used
        //FishingScaleFrame.SetActive(false);
        isFishActive = false;
        hasFishBit = false;
        isFishScared = false;
    }

    IEnumerator ScareFish()
    {
        Vector3 originalPosition = Fish.transform.position;
        Vector3 oppositeDirection = originalPosition - transform.position;
        oppositeDirection.Normalize();

        float elapsedTime = 0f;
        float duration = 3f;

        while (elapsedTime < duration)
        {
            Fish.transform.position = Vector3.MoveTowards(Fish.transform.position, oppositeDirection * fishSpawnDistance, fishSpeed * Time.deltaTime);
            Fish.transform.LookAt(Fish.transform.position + oppositeDirection);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        isFishScared = false;
        FishReset();
    }
}
