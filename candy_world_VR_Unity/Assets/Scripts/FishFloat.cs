using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishFloat : MonoBehaviour
{
    [SerializeField] private GameObject Fish;
    [SerializeField] private Transform Player;

    [SerializeField] private Transform HMD;
    [SerializeField] private Transform Controller_r;
    [SerializeField] private Transform Controller_l;

    private float HMD_Speed;
    private float controller_r_Speed;
    private float controller_l_Speed;
    private float playerSpeed;

    private Vector3 hmdPreviousPosition;
    private Vector3 controllerRPreviousPosition;
    private Vector3 controllerLPreviousPosition;

    [SerializeField] private float fishSpawnDistance;
    [SerializeField] private float FishSpeed;
    [SerializeField] private float biteDistance;
    [SerializeField] private float catchDistance;

    [SerializeField] private float warningSpeed;
    [SerializeField] private float illigalSpeed;

    private bool isFishActive;
    private bool isFishScared;
    private bool hasFishBit;

    private Vector3 previousPosition;
    private float timeSinceLastMove;

    // Start is called before the first frame update
    void Start()
    {
        Fish.gameObject.SetActive(false);
        isFishActive = false;
        hasFishBit = false;
        isFishScared = false;

        // Initialize the previous position
        previousPosition = transform.position;
        timeSinceLastMove = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        // Track speeds of devices
        HMD_Speed = CalculateSpeed(HMD, ref hmdPreviousPosition);
        controller_r_Speed = CalculateSpeed(Controller_r, ref controllerRPreviousPosition);
        controller_l_Speed = CalculateSpeed(Controller_l, ref controllerLPreviousPosition);

        // Calculate the maximum speed among the three devices
        playerSpeed = CalculateMaxSpeed();

        OnStopDrifting();
        MoveFishToFloat();

        if (Vector3.Distance(Fish.transform.position, Player.transform.position) < catchDistance)
        {
            OnFishCatch();
        }

        if (Vector3.Distance(transform.position, Player.transform.position) < catchDistance)
        {
            FishReset();
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
        }
        isFishActive = true;
    }

    void MoveFishToFloat()
    {
        if (playerSpeed < warningSpeed)
        {
            if (isFishActive)
            {
                if (!hasFishBit)
                {
                    if (!isFishScared)
                    {
                        // Move the Fish towards this object at a set speed (FishSpeed)
                        Fish.transform.position = Vector3.MoveTowards(Fish.transform.position, transform.position, FishSpeed * Time.deltaTime);

                        // The fish should always face the direction in which it is moving
                        Fish.transform.LookAt(transform.position);

                        // If the fish is within a specified distance (biteDistance) from this object, set hasFishBit to true
                        if (Vector3.Distance(Fish.transform.position, transform.position) < biteDistance)
                        {
                            hasFishBit = true;
                        }
                    }
                }
                else
                {
                    OnFishBite();
                }
            }
        }
        else if (playerSpeed >= warningSpeed && playerSpeed < illigalSpeed)
        {
            StartCoroutine(WaitAndReturn());
        }
        else if (playerSpeed >= illigalSpeed)
        {
            if(isFishScared == false)
            {
                StartCoroutine(ScareFish());
            }
            isFishScared = true;
        }
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
            Fish.transform.position = Vector3.MoveTowards(Fish.transform.position, oppositeDirection * fishSpawnDistance, FishSpeed * Time.deltaTime);
            Fish.transform.LookAt(Fish.transform.position + oppositeDirection);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        isFishScared = false;
        FishReset();
    }


    IEnumerator WaitAndReturn()
    {
        yield return new WaitForSeconds(3f);
    }
    void OnFishBite()
    {
        // Make the Fish a child of this object
        Fish.transform.parent = transform;

        print("Fish has bitten for whatever reason");
    }

    void OnFishCatch()
    {
        FishReset();
    }

    void FishReset()
    {
        Fish.transform.parent = null;
        Fish.transform.position = new Vector3(0, 0, 0);
        Fish.SetActive(false);
        isFishActive = false;
        hasFishBit = false;
        isFishScared = false;
    }
}
