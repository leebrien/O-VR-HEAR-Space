using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class SoundPolling : MonoBehaviour
{
    public string backendURL = "http://Rams-MacBook-Pro.local:8000/api/v1/export/audio.mp3";

    public void FetchAudioOnce()
    {
        StartCoroutine(PollAudio());
    }

    private IEnumerator PollAudio()
{
    using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(backendURL, AudioType.MPEG))

    {
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log("No audio available: " + www.error);
        }
        else
        {
            var dlHandler = (DownloadHandlerAudioClip)www.downloadHandler;
            dlHandler.streamAudio = false;
            AudioClip hearsonaClip = dlHandler.audioClip;

            Debug.Log("Audio fetched!");

            if (SoundManager.Instance)
            {
                SoundManager.Instance.hearsonaCue = hearsonaClip;
                Debug.Log("Exported hearsona cue stored in SoundManager");
            }
        }
    }
}

}
