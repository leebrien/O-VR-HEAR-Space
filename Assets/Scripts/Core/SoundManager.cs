using System.Collections;
using UnityEngine;
using Oculus.Interaction;
using TMPro;

public class SoundManager : MonoBehaviour
{
    public Transform user; //User reference, eye anchor center
    public GameObject audioObject; // Direct reference to the object in the hierarchy
    public AudioClip genericCue;
    public AudioClip hearsonaCue;
    private readonly Vector3 _roomSize = new Vector3(8.0f, 2.5f, 8.0f);

    private AudioSource _audioSource;
    private Vector3 _soundPosition;

    private Vector3 _initialPosition;

    private void Awake()
    {
        Random.InitState(System.DateTime.Now.Millisecond);

        // Get the AudioSource component from the existing object
        _audioSource = audioObject.GetComponent<AudioSource>();
        if (_audioSource == null)
        {
            Debug.LogError("The assigned audioObject does not have an AudioSource!");
        }

        if (audioObject != null)
        {
            _initialPosition = audioObject.transform.position;
        }
    }

    public void PlaySound()
    {
        if (user == null || genericCue == null || audioObject == null || hearsonaCue == null)
        {
            Debug.LogWarning("User, soundClip, or audioObject not assigned!");
            return;
        }

        if (_audioSource != null)
        {
            _audioSource.Stop();
        }

        _soundPosition = user.position;
        GenerateSoundPosition(ref _soundPosition);

        // Position the existing audio object
        audioObject.transform.position = _soundPosition;
        audioObject.SetActive(true);

        string currentCondition = CoreManager.Instance.currentCondition;
        Debug.Log("Current Condition is: " + currentCondition);

        switch (currentCondition)
        {
            case "PC":
                {
                    InitializeSoundSource(hearsonaCue);
                    break;
                }
            case "GC":
                {
                    InitializeSoundSource(genericCue);
                    break;
                }
        }
    }

    public void StopSound()
    {
        if (_audioSource != null)
        {
            _audioSource.Stop();
            _audioSource.enabled = false;
        }
        //audioObject.SetActive(false);
    }
    public void RevertPosition()
    {
        Rigidbody rb = audioObject.GetComponent<Rigidbody>();

        if (audioObject != null)
        {
            
            rb.MovePosition(_initialPosition);
        }
    }

    private void InitializeSoundSource(AudioClip clip)
    {
        _audioSource.clip = clip;
        _audioSource.spatialBlend = 1.0f;
        _audioSource.playOnAwake = false;
        _audioSource.loop = false;

        StartCoroutine(PlayWithDelay(2f));
    }

    private IEnumerator PlayWithDelay(float delay)
    {
        while (true)
        {
            _audioSource.Play();
            yield return new WaitForSeconds(_audioSource.clip.length + delay);
        }
    }

    private void GenerateSoundPosition(ref Vector3 soundPos)
    {
        int currentTask = CoreManager.Instance.currentTask;
        Debug.Log("Current Task is: " + currentTask);
        DirectionGeneratorPerTask(ref soundPos, currentTask);
    }

    private void DirectionGeneratorPerTask(ref Vector3 soundPos, int taskType)
    {
        if (taskType == 1)
        {
            soundPos.z += 3.5f;
            soundPos.x += Random.Range(-3.75f, 3.75f);
        }
        else if (taskType == 2 || taskType == 3)
        {
            int direction = Random.Range(0, 8);

            // Arms reach
            float offset = Random.Range(0.75f, 1.5f);

            // Far range
            //float offset = Random.Range(1.75f, 3.75f);

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

    public void SetMeshMaterial(Material newMaterial)
    {
        if (audioObject != null)
        {
            Renderer renderer = audioObject.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material = newMaterial;
            }
        }
    }

}