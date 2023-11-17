using UnityEngine;

public class ProximitySoundPlayer : MonoBehaviour
{
    public Transform FishTransform; // Reference to the fish's transform, assign in the Inspector.
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
        if (FishTransform == null)
        {
            Debug.LogError("Target Transform not assigned in the Inspector!");
            return;
        }

        float distance = Vector3.Distance(FishTransform.position, hookTransform.position);

        // Calculate the volume based on the fish's proximity.
        float volume = Mathf.Lerp(farVolume, nearVolume, 1.0f - (distance / detectionRange));
        audioSource.volume = volume;
    }
}
