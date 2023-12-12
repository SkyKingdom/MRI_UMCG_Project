using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrogLogic : MonoBehaviour
{
    [SerializeField] private List<AnimationClip> animationClips;
    [SerializeField] private List<AudioClip> audioClips;

    [SerializeField] private GameObject FrogModel;

    private AudioSource audioSource;
    private Animation animation;

    private float interactionDuration;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        animation = FrogModel.GetComponent<Animation>();
        ReturnInteractionTime();
    }

    void Update()
    {
        print(interactionDuration);
    }

    void ReturnInteractionTime()
    {
        if (animationClips.Count == 0 || audioClips.Count == 0 || animation == null || audioSource == null)
        {
            // No animation or audio clips available, or missing components, return
            return;
        }

        float maxAnimationDuration = 0f;
        foreach (AnimationClip clip in animationClips)
        {
            if (clip.length > maxAnimationDuration)
            {
                maxAnimationDuration = clip.length;
            }
        }

        float maxAudioDuration = 0f;
        foreach (AudioClip clip in audioClips)
        {
            if (clip.length > maxAudioDuration)
            {
                maxAudioDuration = clip.length;
            }
        }

        interactionDuration = Mathf.Max(maxAnimationDuration, maxAudioDuration);
    }
}
