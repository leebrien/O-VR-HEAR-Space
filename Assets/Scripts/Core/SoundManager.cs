using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;
    private Transform _user; //User reference, eye anchor center
    public GameObject audioObject; // Direct reference to the object in the hierarchy
    public AudioClip genericCue;
    public AudioClip hearsonaCue;
    private readonly Vector3 _roomSize = new Vector3(8.0f, 2.5f, 8.0f);

    private AudioSource _audioSource;
    public Vector3 _soundPosition;
    
    private Coroutine _playCoroutine;

    private void Awake()
    {
        
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (CoreManager.Instance == null)
        {
            Debug.LogError("CoreManager.Instance is null. Cannot initialize SoundManager.");
            return;
        }
        
        _user = CoreManager.Instance.GetCenterEyeAnchor();
        audioObject = GameObject.FindGameObjectWithTag("CueSource");
        
        if (audioObject != null) 
            _audioSource = audioObject.GetComponent<AudioSource>();
        
        if (_audioSource == null)
            Debug.LogWarning("No AudioSource found in scene: " + scene.name);
    }

    public void PlaySound()
    {
        if (_user == null || genericCue == null || audioObject == null || hearsonaCue == null)
        {
            Debug.LogWarning("User, soundClip, or audioObject not assigned!");
            return;
        }

        if (_audioSource != null)
        {
            _audioSource.Stop();
        }

        _soundPosition = _user.position;
        GenerateSoundPosition(ref _soundPosition);

        // Position the existing audio object
        audioObject.transform.position = _soundPosition;
        audioObject.SetActive(true);
        
        
        if (CoreManager.Instance == null)
        {
            Debug.LogWarning("CoreManager.Instance is null. Cannot determine condition.");
            return;
        }

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

        // Add this
        if (_playCoroutine != null)
        {
            StopCoroutine(_playCoroutine);
            _playCoroutine = null;
        }
    }

    private void InitializeSoundSource(AudioClip clip)
    {
        if (_playCoroutine != null)
        {
            StopCoroutine(_playCoroutine);
        }
        
        _audioSource.clip = clip;
        _audioSource.spatialBlend = 1.0f;
        _audioSource.playOnAwake = false;
        _audioSource.loop = false;

        _playCoroutine = StartCoroutine(PlayWithDelay(1.5f));
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
        if (CoreManager.Instance == null)
        {
            Debug.LogWarning("CoreManager.Instance is null. Cannot get current task.");
            return; 
        }
        
        int currentTask = CoreManager.Instance.currentTask;
        Debug.Log("Current Task is: " + currentTask);
        DirectionGeneratorPerTask(ref soundPos, currentTask);
    }

    private void DirectionGeneratorPerTask(ref Vector3 soundPos, int taskType)
    {
        if (taskType == 1)
        {
            soundPos.z += 2.5f;
            soundPos.x += Random.Range(-3.5f, 3.5f);
        }
        else if (taskType == 2 || taskType == 3)
        {
            int direction = Random.Range(0, 8);
           // float offset = Random.Range(1.75f, 3.75f);
            float[] allowedoffsets = { 0.75f, 1f, 1.25f };
            float offset = allowedoffsets[Random.Range(0, allowedoffsets.Length)];

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
        soundPos.y = Random.Range(1, _user.position.y + 0.3f);
        soundPos.x = Mathf.Clamp(soundPos.x, -_roomSize.x / 2, _roomSize.x / 2);
        soundPos.z = Mathf.Clamp(soundPos.z, -_roomSize.z / 2, _roomSize.z / 2);
    }

}