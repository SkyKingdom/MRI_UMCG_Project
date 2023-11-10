using UnityEngine;

public class FishMovement : MonoBehaviour
{
    public Vector3 swimAreaCenter = Vector3.zero;
    public Vector3 swimAreaSize = new Vector3(10f, 5f, 10f);

    public float swimSpeed = 2.0f;
    public float rotationSpeed = 5.0f;
    public float maxRotationAngle = 30.0f;

    private FishAI fishAI;
    private Vector3 targetPosition;

    [HideInInspector]
    public float trackedHighestSpeed;

    public float scareSpeed = 0.1f;
    [SerializeField] private Transform hook;
    public float hookDetectionRange = 5.0f; // The range at which fish can be affected by the hook

    public bool ifHookActive = false;
    public bool isFishCaught = false;

    private void Start()
    {
        GetRandomTargetPosition();
        FindFishAIController();
    }

    private void FindFishAIController()
    {
        GameObject fishController = GameObject.FindGameObjectWithTag("FishController");
        if (fishController != null)
        {
            fishAI = fishController.GetComponent<FishAI>();
        }
        else
        {
            Debug.LogError("FishAI controller not found. Make sure to tag it as 'Fish'.");
        }
    }

    private void Update()
    {
        if (fishAI == null)
        {
            return;
        }

        if (ifHookActive)
        {
            FindHookObject();
        }

        trackedHighestSpeed = fishAI.highestSpeed;

        if (Vector3.Distance(transform.position, targetPosition) < 1.0f)
        {
            GetRandomTargetPosition();
        }

        Vector3 swimDirection = CalculateSwimDirection();

        if (ifHookActive && IsWithinHookRange())
        {
            HandleHookInteraction(ref swimDirection);
        }

        RotateFish(swimDirection);
        MoveFishForward();
    }

    private void FindHookObject()
    {
        hook = GameObject.Find("Hook(Clone)").transform;
        if (hook == null)
        {
            Debug.LogError("Hook not found. Make sure to name the hook object 'Hook(Clone)'.");
        }
    }

    private Vector3 CalculateSwimDirection()
    {
        Vector3 swimDirection = (targetPosition - transform.position).normalized;
        return swimDirection;
    }

    private void HandleHookInteraction(ref Vector3 swimDirection)
    {
        if (trackedHighestSpeed < scareSpeed && isFishCaught == false)
        {
            swimDirection = (hook.position - transform.position).normalized;
        }
        else
        {
            swimDirection = (transform.position - hook.position).normalized;
        }
    }

    private void RotateFish(Vector3 swimDirection)
    {
        Quaternion targetRotation = Quaternion.LookRotation(swimDirection);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

    private void MoveFishForward()
    {
        transform.Translate(Vector3.forward * swimSpeed * Time.deltaTime);
    }

    private void GetRandomTargetPosition()
    {
        float randomX = Random.Range(swimAreaCenter.x - swimAreaSize.x / 2, swimAreaCenter.x + swimAreaSize.x / 2);
        float randomY = Random.Range(swimAreaCenter.y - swimAreaSize.y / 2, swimAreaCenter.y + swimAreaSize.y / 2);
        float randomZ = Random.Range(swimAreaCenter.z - swimAreaSize.z / 2, swimAreaCenter.z + swimAreaSize.z / 2);

        targetPosition = new Vector3(randomX, randomY, randomZ);
    }

    private bool IsWithinHookRange()
    {
        if (hook == null)
        {
            return false;
        }

        float distanceToHook = Vector3.Distance(transform.position, hook.position);
        return distanceToHook <= hookDetectionRange;
    }
}
