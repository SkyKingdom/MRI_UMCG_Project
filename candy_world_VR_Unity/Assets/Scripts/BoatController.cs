using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoatController : MonoBehaviour
{
    [SerializeField] private List<Transform> keyframes; // List of empty objects to serve as keyframes
    [SerializeField] private float moveSpeed = 5.0f; // Speed at which the boat moves
    [SerializeField] private GameObject FishingFloat;
    [SerializeField] private RodLogic rodLogic;
    private int currentKeyframeIndex = 0; // Index of the current keyframe

    private void Start()
    {
        RodLogic rodlogic = GetComponent<RodLogic>();
    }
    public void MoveBoat()
    {
        // Check if there are keyframes in the list
        if (keyframes.Count > 0)
        {
            StartCoroutine(MoveToNextKeyframe());
        }
    }

    private IEnumerator MoveToNextKeyframe()
    {
        if (currentKeyframeIndex < keyframes.Count)
        {
            Transform targetKeyframe = keyframes[currentKeyframeIndex];

            while (Vector3.Distance(transform.position, targetKeyframe.position) > 0.1f)
            {
                print("This debug line makes the code work for some reason, I love programming");

                // Smoothly move the boat to the target keyframe position and rotation
                transform.position = Vector3.MoveTowards(transform.position, targetKeyframe.position, moveSpeed * Time.deltaTime);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetKeyframe.rotation, moveSpeed * 10.0f * Time.deltaTime);
                FishingFloat.SetActive(false);
                rodLogic.enabled = false;

                yield return null;
            }

            rodLogic.enabled = true;

            // Update the current keyframe index
            currentKeyframeIndex++;

            // Check if we have reached the final keyframe
            if (currentKeyframeIndex >= keyframes.Count)
            {
                currentKeyframeIndex = 0; // Start over when all keyframes have been reached
            }
        }
    }
}
