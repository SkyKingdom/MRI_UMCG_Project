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
        fadeMaterial = FadeToBlack.GetComponent<Renderer>().material;
        fadeMaterial.color = new Color(0, 0, 0, 1); // Set initial alpha to 1 for fade-in effect
        StartCoroutine(FadeIn());
    }

    private IEnumerator FadeIn()
    {
        float alpha = 1f;

        while (alpha > 0f)
        {
            alpha -= Time.deltaTime * fadeSpeed;
            fadeMaterial.color = new Color(0, 0, 0, alpha);
            yield return null;
        }

        // Ensure the alpha is exactly 0
        fadeMaterial.color = new Color(0, 0, 0, 0);
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

        fadeMaterial.color = new Color(0, 0, 0, 1);

        TransitionScene();
    }

    public void TransitionScene()
    {
        SceneManager.LoadScene(scene);
    }
}
