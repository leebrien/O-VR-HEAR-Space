using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class SoundPolling : MonoBehaviour
{
    public string backendURL;

   public IEnumerator PollAudio(System.Func<bool> isCancelled)
{
    using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(backendURL, AudioType.WAV))

    {
        var request = www.SendWebRequest();
        while (!request.isDone)
        {
            if (isCancelled())
            {
                www.Abort();
                yield break;
            }
            yield return null;
        }
        
        if (isCancelled()) yield break;

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
