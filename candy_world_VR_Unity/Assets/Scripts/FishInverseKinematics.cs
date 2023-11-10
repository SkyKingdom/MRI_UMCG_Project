using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishInverseKinematics : MonoBehaviour
{
    public float tailLength = 3.0f;         // Total length of the tail
    public int numSegments = 10;            // Number of tail segments
    public float amplitude = 30.0f;        // Amplitude of the tail animation
    public float frequency = 2.0f;         // Frequency of the tail animation

    private Transform[] tailSegments;
    private float[] segmentOffsets;

    private void Start()
    {
        InitializeTailSegments();
    }

    private void Update()
    {
        AnimateTail();
    }

    void InitializeTailSegments()
    {
        tailSegments = new Transform[numSegments];
        segmentOffsets = new float[numSegments];

        tailSegments[0] = transform;

        // Assuming the tail segments are child objects of this GameObject
        for (int i = 1; i < numSegments; i++)
        {
            tailSegments[i] = tailSegments[i - 1].transform.GetChild(0);
            segmentOffsets[i] = Random.Range(0, 180);
        }
    }

    void AnimateTail()
    {
        float time = Time.time;
        for (int i = 0; i < numSegments; i++)
        {
            float angleOffset = segmentOffsets[i];
            float angle = Mathf.Sin(time * frequency + angleOffset) * amplitude;
            Quaternion rotation = Quaternion.Euler(0.0f, 0.0f, angle);
            tailSegments[i].localRotation = rotation;
        }
    }
}
