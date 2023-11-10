using UnityEngine;

public class WaterFloating : MonoBehaviour
{
    private Rigidbody rb;
    [SerializeField] private float waterSurface = -4.27f;
    public float floatStrength = 10f; // Adjust this to control the force applied to the object
    public float damping = 0.5f; // Adjust this to control the damping effect

    void Start()
    {
        // Make sure the object has a Rigidbody component
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("The object must have a Rigidbody component to use WaterFloating.");
            enabled = false;
        }
    }

    void Update()
    {
        // Check if the object is below y the surface of the water
        if (transform.position.y < waterSurface)
        {
            // Calculate the depth below the water surface
            float depth = waterSurface - transform.position.y;

            // Calculate the force to push the object up
            float upwardForce = depth * floatStrength;

            // Apply the force to push the object up
            Vector3 floatUpward = new Vector3(0f, upwardForce, 0f);
            rb.AddForce(floatUpward);

            // Apply damping to prevent oscillations
            Vector3 dampingForce = -rb.velocity * damping;
            rb.AddForce(dampingForce);
        }
    }
}
