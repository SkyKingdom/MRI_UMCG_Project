using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProtoTravelCamera : MonoBehaviour
{
    public GameObject rightController; // Reference to the right VR controller
    public float countdownDuration = 5f; // Public variable for countdown duration in seconds

    private GameObject lastHitObject; // Track the last object that was hit
    private float countdownTimer; // Timer for the countdown

    public Image LoadUI;

    private void Update()
    {
        if (rightController == null)
        {
            Debug.LogWarning("Right controller reference is not set!");
            return;
        }

        Vector3 controllerPosition = rightController.transform.position;
        Vector3 direction = controllerPosition - transform.position;

        RaycastHit hit;
        if (Physics.Raycast(transform.position, direction, out hit))
        {
            HandleHitObject(hit.collider.gameObject);
            Debug.DrawRay(transform.position, direction * hit.distance, Color.red);

            LoadUI.transform.parent.position = Vector3.Lerp(transform.position, hit.point, 0.5f); //Position of loadbar
            LoadUI.transform.parent.LookAt(transform.position);
        }
        else
        {
            ResetHitObject();
            Debug.DrawRay(transform.position, direction.normalized * 100f, Color.red);
        }

        // Countdown timer logic
        UpdateCountdownTimer();
    }

    private void HandleHitObject(GameObject hitObject)
    {
        if (hitObject.CompareTag("TravelLoc"))
        {
            if (lastHitObject != hitObject)
            {
                ResetCountdownTimer();
            }
            lastHitObject = hitObject;

            StartCountdownTimer();
        }
        else
        {
            ResetHitObject();
        }
    }

    private void UpdateCountdownTimer()
    {
        if (countdownTimer > 0)
        {
            countdownTimer -= Time.deltaTime;

            LoadUI.fillAmount = Mathf.InverseLerp(countdownDuration, 0, countdownTimer);


            if (countdownTimer <= 0)
            {
                countdownTimer = 0;
                SendChangeSceneMessage();
            }
        }
    }

    private void StartCountdownTimer()
    {
        if (countdownTimer <= 0)
        {
            countdownTimer = countdownDuration;
        }

        LoadUI.transform.parent.gameObject.SetActive(true); //Activates load UI

    }

    private void ResetCountdownTimer()
    {
        countdownTimer = 0;
        LoadUI.transform.parent.gameObject.SetActive(false); //Deactivates load UI
    }

    private void ResetHitObject()
    {
        lastHitObject = null;
        ResetCountdownTimer();
    }

    private void SendChangeSceneMessage()
    {
        if (lastHitObject != null)
        {
            ChangeScene changeSceneScript = lastHitObject.GetComponent<ChangeScene>();
            if (changeSceneScript != null)
            {
                changeSceneScript.TransitionScene();
            }
        }
    }
}
