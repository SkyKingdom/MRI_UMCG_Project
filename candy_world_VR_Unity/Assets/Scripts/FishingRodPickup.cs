using UnityEngine;
using UnityEngine.InputSystem.Android.LowLevel;

public class FishingRodPickup : MonoBehaviour
{
    private Transform controllerTransform;
    private bool isPickedUp = false;

    private void OnTriggerEnter(Collider other)
    {
        if (isPickedUp) return;

        if (other.CompareTag("GameController"))
        {
            controllerTransform = other.transform;
            isPickedUp = true;
            print("Is picking up");
        }
    }

    private void FixedUpdate()
    {
        if (isPickedUp && controllerTransform != null)
        {
            // Apply the transforms (location and rotation) to the fishing rod
            transform.position = controllerTransform.position;
            transform.rotation = controllerTransform.rotation;

            // Add an additional 90 degrees of rotation to the X-axis
            transform.Rotate(Vector3.right, 90f);
        }
    }
}
