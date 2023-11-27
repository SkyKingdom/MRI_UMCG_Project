using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeScene : MonoBehaviour
{
    [SerializeField] private string scene;
    [SerializeField] private GameObject FadeToBlack;
    [SerializeField] private float fadeSpeed;

    private Material fadeMaterial;
    private bool isFading = false;

    void Start()
    {
        // Assuming FadeToBlack has a material with a color property that supports transparency
        fadeMaterial = FadeToBlack.GetComponent<Renderer>().material;
        fadeMaterial.color = new Color(0, 0, 0, 0); // Set initial alpha to 0
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isFading)
        {
            StartCoroutine(FadeOutAndLoadScene());
        }
    }

    private IEnumerator FadeOutAndLoadScene()
    {
        isFading = true;

        float alpha = 0f;

        while (alpha < 1f)
        {
            alpha += Time.deltaTime * fadeSpeed;
            fadeMaterial.color = new Color(0, 0, 0, alpha);
            yield return null;
        }

        // Ensure the alpha is exactly 1
        fadeMaterial.color = new Color(0, 0, 0, 1);

        // Call the scene transition method after fading out
        TransitionScene();
    }

    public void TransitionScene()
    {
        SceneManager.LoadScene(scene);
    }
}
