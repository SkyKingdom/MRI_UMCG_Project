using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishAI : MonoBehaviour
{
    public Transform vrHeadset;
    public Transform leftController;
    public Transform rightController;
    public Transform speedPointer;
    public Transform speedFrame;
    public GameObject fishingRod;
    public GameObject fishPrefab;

    public float headsetSpeed { get; private set; }
    public float leftControllerSpeed { get; private set; }
    public float rightControllerSpeed { get; private set; }

    public float highestSpeed;

    public float smoothingFactor = 5.0f;

    private Vector3 previousHeadsetPosition;
    private Vector3 previousLeftControllerPosition;
    private Vector3 previousRightControllerPosition;
    private float smoothRotationAngle;

    // List of spawn locations for fish
    public List<Vector3> fishSpawnLocations = new List<Vector3>();

    // Spawn fish every 10 seconds
    public float spawnInterval = 10f;

    // Maximum number of active fish
    public int maxActiveFish = 10;

    // Current count of active fish
    public int activeFishCount = 0;

    private void Start()
    {
        previousHeadsetPosition = vrHeadset.position;
        previousLeftControllerPosition = leftController.position;
        previousRightControllerPosition = rightController.position;

        speedFrame.gameObject.SetActive(false);

        // Start spawning fish
        StartCoroutine(SpawnFishPeriodically());
    }

    private void Update()
    {
        headsetSpeed = CalculateSpeed(vrHeadset);
        leftControllerSpeed = CalculateSpeed(leftController);
        rightControllerSpeed = CalculateSpeed(rightController);

        UpdatePreviousPositions();

        highestSpeed = Mathf.Max(headsetSpeed, leftControllerSpeed, rightControllerSpeed);

        float rotationPercentage = Mathf.Clamp01(highestSpeed / 0.6f);
        float targetRotationAngle = Mathf.Lerp(0f, -180f, rotationPercentage);

        smoothRotationAngle = Mathf.Lerp(smoothRotationAngle, targetRotationAngle, Time.deltaTime * smoothingFactor);

        speedPointer.localRotation = Quaternion.Euler(0f, 0f, smoothRotationAngle);
    }

    private float CalculateSpeed(Transform deviceTransform)
    {
        return Vector3.Distance(deviceTransform.position, GetPreviousPosition(deviceTransform)) / Time.deltaTime;
    }

    private Vector3 GetPreviousPosition(Transform deviceTransform)
    {
        string key = deviceTransform.name + "_PrevPosition";
        return PlayerPrefs.HasKey(key) ? JsonUtility.FromJson<Vector3>(PlayerPrefs.GetString(key)) : deviceTransform.position;
    }

    private void UpdatePreviousPositions()
    {
        SetPreviousPosition(vrHeadset);
        SetPreviousPosition(leftController);
        SetPreviousPosition(rightController);
    }

    private void SetPreviousPosition(Transform deviceTransform)
    {
        string key = deviceTransform.name + "_PrevPosition";
        PlayerPrefs.SetString(key, JsonUtility.ToJson(deviceTransform.position));
    }

    public void EnablespeedPointer()
    {
        speedFrame.gameObject.SetActive(true);
    }

    public void DisablespeedPointer()
    {
        speedFrame.gameObject.SetActive(false);
    }

    // Method to spawn fish at specified locations
    public void SpawnFish(Vector3 spawnLocation)
    {
        if (activeFishCount < maxActiveFish)
        {
            // Instantiate fish prefab at the specified location
            activeFishCount++;
            Instantiate(fishPrefab, spawnLocation, Quaternion.identity);
        }
    }

    // Coroutine to spawn fish periodically
    private IEnumerator SpawnFishPeriodically()
    {
        while (true)
        {
            // Calculate how many fish can be spawned without exceeding the limit
            int fishToSpawn = Mathf.Min(maxActiveFish - activeFishCount, fishSpawnLocations.Count);

            for (int i = 0; i < fishToSpawn; i++)
            {
                // Spawn fish at the next spawn location
                SpawnFish(fishSpawnLocations[i]);
            }

            yield return new WaitForSeconds(spawnInterval);
        }
    }
}
