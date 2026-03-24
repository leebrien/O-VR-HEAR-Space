using UnityEngine;

// This script goes on the "Audio Source" object in your Task scenes
[RequireComponent(typeof(AudioSource))]
public class SceneAudioLink : MonoBehaviour
{
    void Start()
    {
        // 1. Get my own AudioSource component
        AudioSource mySource = GetComponent<AudioSource>();
        
        // 2. Tell the SoundManager I exist
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.RegisterSceneAudioSource(mySource);
        }
    }

    void OnDestroy()
    {
        // 3. Tell the SoundManager I'm gone
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.UnregisterSceneAudioSource();
        }
    }
}