using UnityEngine;

public class ProximitySoundPlayer : MonoBehaviour
{
    public string targetTag = "Fish";
    public float detectionRange = 10.0f; // Set this in the Inspector.
    public float farVolume = 0.2f; // Set this in the Inspector as a percentage.
    public float nearVolume = 1.0f; // Set this in the Inspector as a percentage.

    private AudioSource audioSource;
    private Transform hookTransform; // Assuming this is the object with the script attached.

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        hookTransform = transform;
        audioSource.volume = 0.0f; // Start with volume off.
    }

    private void Update()
    {
        // Find all objects with the specified tag.
        GameObject[] fishObjects = GameObject.FindGameObjectsWithTag(targetTag);

        float closestDistance = detectionRange;

        foreach (GameObject fishObject in fishObjects)
        {
            float distance = Vector3.Distance(fishObject.transform.position, hookTransform.position);

            if (distance < closestDistance)
            {
                closestDistance = distance;
            }
        }

        // Calculate the volume based on the closest fish's proximity.
        float volume = Mathf.Lerp(farVolume, nearVolume, 1.0f - (closestDistance / detectionRange));
        audioSource.volume = volume;
    }
}
