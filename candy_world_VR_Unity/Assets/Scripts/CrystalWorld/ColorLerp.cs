using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
public class ColorLerp : MonoBehaviour
{
    public Color[] colors; //array to hold coloras
    public float lerpDuration = 1f; // duration of cycle

    private Renderer renderer; //Reference to Renderer component

    void Start()
    {
        renderer = GetComponent<Renderer>(); // Gets the Renderer component in any GameObject the code is placed in
        StartCoroutine(LerpColors());// start colour change
    }

    IEnumerator LerpColors()//Endless loop of colour
    {
        int currentIndex = 0;//the index of the current color in the array

        // Infinite while loop for neverending color change
        while (true)
        {
            //elapsed time determines the duration between a start and an end time
            float timeElapsed = 0f;//initate time elapse//

            while (timeElapsed < lerpDuration)
            {
                float t = timeElapsed / lerpDuration;
                //to be able switch colours
                renderer.material.color = Color.Lerp(colors[currentIndex], colors[(currentIndex + 1) % colors.Length], t);
                timeElapsed += Time.deltaTime;
                yield return null;
            }

            currentIndex = (currentIndex + 1) % colors.Length;//to move to next colour
        }
    }
}