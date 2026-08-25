using System;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace Module5.Player
{
    //[System.Serializable]
    //public class AudioImportSettings
    //{
    //    public AudioType audioType = AudioType.UNKNOWN;
    //}

    public class AudioImporter
    {
        const int FileFormatVersion = 1;

        public async Task<AudioClip> ImportAsync(Uri uri)
        {
            AudioType audioType = Path.GetExtension(uri.AbsolutePath).ToLower() switch
            {
                ".mp3" => AudioType.MPEG,
                ".wav" => AudioType.WAV,
                ".wave" => AudioType.WAV,
                ".ogg" => AudioType.OGGVORBIS,
                _ => AudioType.UNKNOWN
            };


            using (UnityWebRequest request = UnityWebRequestMultimedia.GetAudioClip(uri, audioType))
            {
                await request.SendWebRequest();

                if (request.result == UnityWebRequest.Result.ConnectionError)
                {
                    Debug.Log(request.error);
                }
                else
                {
                    
                    AudioClip myClip = DownloadHandlerAudioClip.GetContent(request);
                    myClip.name = Path.GetFileName(uri.AbsolutePath);
                    return myClip;
                }
            }

            return null;
        }
    }
}
