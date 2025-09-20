using System.Collections;
using UnityEngine;

public class PracticeSoundManager : MonoBehaviour
{
    public Transform user; // Assign to center eye anchor

    // Have two different audio source object to avoid one that is both grabbable and pointable
    public GameObject audioObjectTask1; // Audio object for Task 1
    public GameObject audioObjectTask2_3; // Audio object for Tasks 2 & 3

    public AudioClip practiceCue;

    private AudioSource _audioSource;
    private readonly Vector3 _roomSize = new Vector3(8.0f, 2.5f, 8.0f);

    private void Awake()
    {
        // Get the AudioSource component for the active audio object.
        // It will be assigned in the PlaySound method based on the task type.
    }

    public void PlaySound(int taskType)
    {
        // Hide all audio objects to ensure only one is active at a time
        audioObjectTask1?.SetActive(false);
        audioObjectTask2_3?.SetActive(false);

        // Determine which audio object to use and get its AudioSource
        GameObject activeAudioObject;
        if (taskType == 1)
        {
            activeAudioObject = audioObjectTask1;
        }
        else if (taskType == 2 || taskType == 3)
        {
            activeAudioObject = audioObjectTask2_3;
        }
        else
        {
            Debug.LogWarning("Invalid task type provided.");
            return;
        }

        if (user == null || activeAudioObject == null || practiceCue == null)
        {
            Debug.LogWarning("Required components or audio clip not assigned in the Inspector.");
            return;
        }

        // Get the audio source from the active object
        _audioSource = activeAudioObject.GetComponent<AudioSource>();
        if (_audioSource == null)
        {
            Debug.LogWarning("AudioSource component not found on the active audio object.");
            return;
        }

        _audioSource.Stop();

        // Generate a new position based on the selected task
        Vector3 soundPosition = user.position;
        GenerateSoundPosition(ref soundPosition, taskType);

        activeAudioObject.transform.position = soundPosition;
        activeAudioObject.SetActive(true);

        InitializeSoundSource(practiceCue);
    }

    public void StopSound()
    {
        if (_audioSource != null)
        {
            _audioSource.Stop();
        }
    }

    private void InitializeSoundSource(AudioClip clip)
    {
        _audioSource.clip = clip;
        _audioSource.spatialBlend = 1.0f; // Enable 3D sound
        _audioSource.playOnAwake = false;
        _audioSource.loop = false;

        StartCoroutine(PlayWithDelay(2f));
    }

    private IEnumerator PlayWithDelay(float delay)
    {
        while (true)
        {
            if (_audioSource == null) yield break;
            _audioSource.Play();
            yield return new WaitForSeconds(_audioSource.clip.length + delay);
        }
    }

    private void GenerateSoundPosition(ref Vector3 soundPos, int taskType)
    {
        if (taskType == 1)
        {
            // Position for Task 1: Pointing
            soundPos.z += 3.5f;
            soundPos.x += Random.Range(-3.75f, 3.75f);
        }
        else if (taskType == 2 || taskType == 3)
        {
            // Positions for Task 2 & 3: Grabbing
            int direction = Random.Range(0, 8);
            float offset = Random.Range(1.75f, 3.75f);

            switch (direction)
            {
                case 0: soundPos.z += offset; break;
                case 1: soundPos.z -= offset; break;
                case 2: soundPos.x += offset; break;
                case 3: soundPos.x -= offset; break;
                case 4: soundPos.z += offset; soundPos.x += offset; break;
                case 5: soundPos.z += offset; soundPos.x -= offset; break;
                case 6: soundPos.z -= offset; soundPos.x += offset; break;
                case 7: soundPos.z -= offset; soundPos.x -= offset; break;
            }
        }

        soundPos.y = Random.Range(1, user.position.y + 0.3f);
        soundPos.x = Mathf.Clamp(soundPos.x, -_roomSize.x / 2, _roomSize.x / 2);
        soundPos.z = Mathf.Clamp(soundPos.z, -_roomSize.z / 2, _roomSize.z / 2);
    }
}