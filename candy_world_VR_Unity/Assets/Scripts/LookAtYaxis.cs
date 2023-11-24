using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtYaxis : MonoBehaviour
{
    [SerializeField] private Transform Target;
    [SerializeField] private bool flipYRotation = false;

    void Update()
    {
        if (Target != null)
        {
            Vector3 directionToTarget = Target.position - transform.position;
            directionToTarget.y = 0f;

            // Calculate the rotation
            Quaternion targetRotation = Quaternion.LookRotation(directionToTarget.normalized, Vector3.up);

            // Optionally apply the 180-degree flip
            if (flipYRotation)
            {
                targetRotation *= Quaternion.Euler(0f, 180f, 0f);
            }

            // Apply the rotation to the object
            transform.rotation = targetRotation;
        }
    }
}
