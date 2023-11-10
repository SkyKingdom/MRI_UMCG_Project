using UnityEngine;

public class FishCaught : MonoBehaviour
{
    private GameObject fishAIController; // Reference to the FishAI controller GameObject

    private void Start()
    {
        // Find the "FishAI Controller" object in the scene
        fishAIController = GameObject.Find("FishAI Controller");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Fish"))
        {
            // Remove rigid body and all scripts from the fish
            Rigidbody fishRigidbody = other.GetComponent<Rigidbody>();

            if (fishRigidbody != null)
            {
                Destroy(fishRigidbody);
            }

            MonoBehaviour[] scripts = other.GetComponents<MonoBehaviour>();
            foreach (MonoBehaviour script in scripts)
            {
                if (script != this)
                {
                    Destroy(script);
                }
            }

            // Make the fish a child of this object
            other.transform.parent = transform;

            // Set isFishCaught bool in the FishMovement script to true
            FishMovement fishMovement = other.GetComponent<FishMovement>();
            if (fishMovement != null)
            {
                fishMovement.isFishCaught = true;
            }

            // Find the "Fishing rod" object and retrieve the Fishing script from it
            GameObject fishingRod = GameObject.Find("Fishing rod");

            if (fishingRod != null)
            {
                Fishing fishingScript = fishingRod.GetComponent<Fishing>();
                if (fishingScript != null)
                {
                    // Activate the ScoreScale GameObject in the Fishing script
                    fishingScript.ScoreScale.SetActive(true);
                    fishingScript.isFishCaught = true;
                    fishingScript.ScoreScale.transform.position = transform.position + Vector3.up * 1f;
                }
            }

            // Send a message to the FishAI script on the fishAIController GameObject to disable the speed tracker
            if (fishAIController != null)
            {
                FishAI fishAIScript = fishAIController.GetComponent<FishAI>();
                if (fishAIScript != null)
                {
                    fishAIScript.DisablespeedPointer();
                }
            }
        }
    }
}
