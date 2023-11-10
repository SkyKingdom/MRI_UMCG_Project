using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoatController : MonoBehaviour
{
    public List<Transform> keyframes; // List of empty objects to serve as keyframes
    public float moveSpeed = 5.0f; // Speed at which the boat moves
    private int currentKeyframeIndex = 0; // Index of the current keyframe

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // Check if there are keyframes in the list
            if (keyframes.Count > 0)
            {
                StartCoroutine(MoveToNextKeyframe());
            }
        }
    }

    private IEnumerator MoveToNextKeyframe()
    {
        if (currentKeyframeIndex < keyframes.Count)
        {
            Transform targetKeyframe = keyframes[currentKeyframeIndex];

            while (Vector3.Distance(transform.position, targetKeyframe.position) > 0.1f)
            {
                // Smoothly move the boat to the target keyframe position and rotation
                transform.position = Vector3.MoveTowards(transform.position, targetKeyframe.position, moveSpeed * Time.deltaTime);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetKeyframe.rotation, moveSpeed * 10.0f * Time.deltaTime);

                yield return null;
            }

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
