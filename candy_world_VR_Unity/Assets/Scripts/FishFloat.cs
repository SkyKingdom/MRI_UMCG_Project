using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishFloat : MonoBehaviour
{
    [SerializeField] private GameObject Fish;
    [SerializeField] private Transform Player;

    [SerializeField] private float fishSpawnDistance;
    [SerializeField] private float FishSpeed;
    [SerializeField] private float biteDistance;
    [SerializeField] private float catchDistance;
    [SerializeField] private float rotationDuration = 3f;

    private bool isFishActive;
    private bool hasFishBit;

    private Vector3 previousPosition;
    private float timeSinceLastMove;

    // Start is called before the first frame update
    void Start()
    {
        Fish.gameObject.SetActive(false);
        isFishActive = false;
        hasFishBit = false;

        // Initialize the previous position
        previousPosition = transform.position;
        timeSinceLastMove = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        OnStopDrifting();
        MoveFishToFloat();
        if (Vector3.Distance(Fish.transform.position, Player.transform.position) < catchDistance)
        {
            onFishCatch();
        }

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
        if (isFishActive == true)
        {
            if (!hasFishBit)
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
            else
            {
                OnFishBite();
            }
        }
    }

    void OnFishBite()
    {
        // Make the Fish a child of this object
        Fish.transform.parent = transform;
    }

    void onFishCatch()
    {
        // Remove the fish as a child of this object
        Fish.transform.parent = null;

        // Rotate the fish for rotationDuration seconds on the y-axis
        StartCoroutine(RotateFish());
    }

    IEnumerator RotateFish()
    {
        float elapsedTime = 0f;

        while (elapsedTime < rotationDuration)
        {
            Fish.transform.Rotate(Vector3.up, Time.deltaTime * 360f / rotationDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Stop rotating and set the fish to inactive
        Fish.SetActive(false);
        print("Fish set to inactive");
    }
}
