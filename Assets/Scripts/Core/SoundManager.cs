using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;
    private Transform _user; 
    public AudioClip genericCue;
    public AudioClip hearsonaCue;
    private readonly Vector3 _roomSize = new Vector3(8.0f, 2.5f, 8.0f);

    private AudioSource _currentSceneAudioSource;
    
    //private AudioSource _audioSource;
    public Vector3 _soundPosition;
    
    private Coroutine _playCoroutine;

    private void Awake()
    {
        
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            //SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        //SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (CoreManager.Instance == null)
        {
            Debug.LogError("CoreManager.Instance is null. Cannot initialize SoundManager.");
            return;
        }
        
        _user = CoreManager.Instance.GetCenterEyeAnchor();
    }
    
    public void RegisterSceneAudioSource(AudioSource source)
    {
        _currentSceneAudioSource = source;
    }

    public void UnregisterSceneAudioSource()
    {
        _currentSceneAudioSource = null;
    }

    public void PlaySound()
    {
        
        if (_user == null && CoreManager.Instance != null)
        {
            _user = CoreManager.Instance.GetCenterEyeAnchor();
        }

        // Use the new _currentSceneAudioSource reference
        if (_user == null || genericCue == null || _currentSceneAudioSource == null || hearsonaCue == null)
        {
            Debug.LogWarning("SoundManager is missing references! User, Clip, or SceneAudioSource is null.");
            return;
        }

        if (_currentSceneAudioSource != null)
        {
            _currentSceneAudioSource.Stop();
        }

        _soundPosition = _user.position;
        GenerateSoundPosition(ref _soundPosition);

       
        _currentSceneAudioSource.transform.position = _soundPosition;
        _currentSceneAudioSource.gameObject.SetActive(true);
        
        
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
        if (_currentSceneAudioSource != null)
        {
            _currentSceneAudioSource.Stop();
            _currentSceneAudioSource.enabled = false;
        }
        
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
        
        _currentSceneAudioSource.clip = clip;
        _currentSceneAudioSource.spatialBlend = 1.0f;
        _currentSceneAudioSource.playOnAwake = false;
        _currentSceneAudioSource.loop = false;

        _playCoroutine = StartCoroutine(PlayWithDelay(1.5f));
    }

    private IEnumerator PlayWithDelay(float delay)
    {
        while (true)
        {
            _currentSceneAudioSource.Play();
            yield return new WaitForSeconds(_currentSceneAudioSource.clip.length + delay);
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
            float offset = Random.Range(1.75f, 3.25f);
            //For testing offsets to allow searching in small spaces
            //float[] allowedoffsets = { 0.75f, 1f, 1.25f };
            //float offset = allowedoffsets[Random.Range(0, allowedoffsets.Length)];

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