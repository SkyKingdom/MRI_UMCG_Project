using System.Collections;
using UnityEngine;

public class GrandpaController : MonoBehaviour
{
    private Animator animator;
    private int currentParameterIndex = 0;
    private string[] animationParameters;

    [SerializeField]
    private string[] cheerAnimationParameters; // Add this serialized string array field in the Inspector

    [SerializeField]
    private Vector3 cheerTriggerPosition = Vector3.zero; // Specify the trigger position in the Inspector

    [SerializeField]
    private float cheerTriggerRadius = 5f; // Specify the trigger radius in the Inspector

    private bool canCheer = true; // Flag to track if Grandpa can cheer
    private float cheerCooldown = 5f; // Cooldown time in seconds

    private void Start()
    {
        // Get the Animator component attached to the GameObject
        animator = GetComponent<Animator>();

        if (animator == null)
        {
            Debug.LogError("Animator component not found on this GameObject.");
        }

        // Get all animation parameters from the Animator Controller
        animationParameters = GetAnimatorParameters();

        // Trigger "StartPointing" animation parameter 7 seconds after the game starts
        Invoke("TriggerStartPointing", 7f);
    }

    private void TriggerStartPointing()
    {
        if (animator != null)
        {
            animator.SetTrigger("StartPointing");
        }
    }

    private void Update()
    {
        // Check if the spacebar is pressed
        /*if (Input.GetKeyDown(KeyCode.Space))
        {
            if (animator != null)
            {
                animator.SetTrigger("StopPointing");
            }
        }*/

        // Check if any object(s) with the "Fish" tag are within the trigger radius of the specified position
        Collider[] fishColliders = Physics.OverlapSphere(cheerTriggerPosition, cheerTriggerRadius);
        foreach (Collider fishCollider in fishColliders)
        {
            if (fishCollider.CompareTag("Fish") && canCheer)
            {
                GrandpaCheer();
                StartCoroutine(StartCooldown());
                break; // Exit the loop after triggering the cheer once
            }
        }
    }

    private void GrandpaCheer()
    {
        if (cheerAnimationParameters.Length > 0)
        {
            // Pick a random parameter from the list
            int randomIndex = Random.Range(0, cheerAnimationParameters.Length);
            string randomParameter = cheerAnimationParameters[randomIndex];

            // Trigger the selected animation parameter
            animator.SetTrigger(randomParameter);
        }
        else
        {
            Debug.LogWarning("No cheer animation parameters are defined in the Inspector.");
        }
    }

    private string[] GetAnimatorParameters()
    {
        if (animator == null)
        {
            return new string[0];
        }

        AnimatorControllerParameter[] parameters = animator.parameters;
        string[] parameterNames = new string[parameters.Length];

        for (int i = 0; i < parameters.Length; i++)
        {
            parameterNames[i] = parameters[i].name;
        }

        return parameterNames;
    }

    private IEnumerator StartCooldown()
    {
        canCheer = false;
        yield return new WaitForSeconds(cheerCooldown);
        canCheer = true;
    }
}
