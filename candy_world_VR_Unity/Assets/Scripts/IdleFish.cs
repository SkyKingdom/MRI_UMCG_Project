using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleFish : MonoBehaviour
{
    [SerializeField] private GameObject Fish;
    [SerializeField] private float maxFish;
    [SerializeField] private float maxDistance;
    [SerializeField] private float speed;
    [SerializeField] private float waypointAmount;

    private List<Transform> waypoints = new List<Transform>();
    private List<GameObject> fishObjects = new List<GameObject>();

    void Start()
    {
        GenerateWaypoints();
        InstantiateFish();
    }

    void Update()
    {
        MoveFish();
    }

    void GenerateWaypoints()
    {
        // Add waypoints in random locations within the range of 'maxDistance' compared to this object.
        // The waypoints should only be on the horizontal axis, so no height.
    }

    void InstantiateFish()
    {
        // Instantiate 'Fish' objects within the range of 'maxDistance' compared to this object,
        // the amount of fish instantiated should be equal to maxFish.
    }

    void MoveFish()
    {
        // Move the fish objects between the waypoints previously created
        // The fish should be moved constandtly
        // The fish should always face the directoin in which they are going
    }
}
