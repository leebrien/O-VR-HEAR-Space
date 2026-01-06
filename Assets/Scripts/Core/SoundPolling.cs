using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class SoundPolling : MonoBehaviour
{
    public string backendURL;
    public int maxAttempts = 5;
    public float retryDelay = 2f;
    

    // ReSharper disable Unity.PerformanceAnalysis
    public IEnumerator PollAudio(System.Func<bool> isCancelled,  System.Action<bool> onComplete)
   { 
       yield return new WaitForSeconds(2f);

       for (int attempt = 1; attempt <= maxAttempts; attempt++)
       {
           if (isCancelled()) yield break;
           using var www = UnityWebRequestMultimedia.GetAudioClip(backendURL, AudioType.WAV);
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
           if (www.result == UnityWebRequest.Result.Success)
           {
               
               var dlHandler = (DownloadHandlerAudioClip)www.downloadHandler;
               dlHandler.streamAudio = false;
               AudioClip hearsonaClip = dlHandler.audioClip;

               Debug.Log($"Audio fetched on attempt {attempt}!");

               if (SoundManager.Instance)
               {
                   SoundManager.Instance.hearsonaCue = hearsonaClip;
                   Debug.Log("Exported hearsona cue stored in SoundManager");
               }
               onComplete?.Invoke(true);
               yield break;
           }
           else
           {
               if (attempt == maxAttempts)
               {
                   Debug.LogError("All polling attempts failed.");
                   onComplete?.Invoke(false);
                   yield break;
               }
               yield return new WaitForSeconds(retryDelay);
           }
       }
   }

}
