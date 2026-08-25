using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using Module5.DI;
namespace Module5.Player
{
    public abstract class VideoPlayerBase : MonoBehaviour
    {
        //[Inject] public IAssetDataBaseManager AssetDataBaseManager { get; set; }
        [Inject] public IEventManager EventManager { get; set; }

        [SerializeField] protected VideoPlayer videoPlayer;
        public VideoPlayer VideoPlayer
        {
            get 
            {
                if (videoPlayer == null) videoPlayer = GetComponent<VideoPlayer>();
                return videoPlayer; 
            }
        }

        public bool IsVideoPlaying => VideoPlayer.isPlaying;

        [SerializeField] protected Text textErrorMsg;
        [SerializeField] protected Toggle togglePlay;
        [SerializeField] protected bool showPlayButton = true;
        public virtual bool ShowPlayButton
        {
            get => showPlayButton;
            set
            {
                showPlayButton = value;
                if(togglePlay != null) togglePlay.gameObject.SetActive(VideoPlayer.isPrepared && value);
            }
        }

        public virtual string Url
        {
            get => VideoPlayer.url;
            set => VideoPlayer.url = value;
        }
        public virtual bool IsLoop
        {
            get => VideoPlayer.isLooping;
            set => VideoPlayer.isLooping = value;
        }
        public bool playOnAwake = true;

        protected bool isPlayAfterPrepare = false;

        protected virtual void Awake()
        {
            videoPlayer.prepareCompleted += OnVideoPrepared;
            videoPlayer.loopPointReached += OnVideoEnd;
            videoPlayer.errorReceived += OnVideoError;

            if (togglePlay != null)
            {
                togglePlay.onValueChanged.AddListener(OnTogglePlayChanged);
                togglePlay.gameObject.SetActive(false);
            }
        }

        protected virtual void OnEnable()
        {
            if (videoPlayer.url != null && videoPlayer.isPrepared == false) 
            {
                isPlayAfterPrepare = playOnAwake;
                Prepare();
            }
        }

        protected virtual void OnDisable()
        {
            StopVideo();
        }

        public virtual void PrepareVideoFromUrl(string url, bool isPlayAfterPrepare, bool isLoop)
        {
            this.isPlayAfterPrepare = isPlayAfterPrepare;
            if (!isPlayAfterPrepare)
            {
                StopVideo();
            }
            else
            {
                videoPlayer.Stop();
            }
            videoPlayer.url = url;
            videoPlayer.isLooping = isLoop;
            if (isActiveAndEnabled) Prepare();
        }

        protected virtual void Prepare()
        {
            if (togglePlay != null) togglePlay.gameObject.SetActive(false);
            textErrorMsg.gameObject.SetActive(false);
            videoPlayer.Prepare();
        }

        protected virtual void OnVideoError(VideoPlayer source, string message)
        {
            Debug.LogError("VideoPlayer Error: " + message);
            textErrorMsg.gameObject.SetActive(true);
            textErrorMsg.text = message;
            videoPlayer.Stop();
            if (togglePlay != null)
            {
                togglePlay.isOn = true;
            }
            if (togglePlay != null) togglePlay.gameObject.SetActive(false);
        }

        protected virtual void OnVideoPrepared(VideoPlayer source)
        {
            if (togglePlay != null) togglePlay.gameObject.SetActive(true && showPlayButton);

            if (isPlayAfterPrepare)
            {
                PlayVideo();
            }
            else
            {
                videoPlayer.Pause();
                if (togglePlay != null)
                {
                    togglePlay.isOn = true;
                }
            }
        }

        protected virtual void OnVideoEnd(VideoPlayer source)
        {
            if (!source.isLooping)
            {
                source.Pause();
                M5Event enableEvent = VideoEvents.CreatePlayFinishEvent(this, this, Url);
                EventManager.TriggerEvent(enableEvent);

                if (togglePlay != null)
                {
                    togglePlay.isOn = true;
                }
            }
        }

        protected virtual void OnTogglePlayChanged(bool value)
        {
            if (value)
            {
                if (videoPlayer.isPlaying)
                {
                    PauseVideo();
                }
            }
            else
            {
                if (!videoPlayer.isPlaying)
                {
                    PlayVideo();
                }
            }
        }

        [ContextMenu("Play")]
        public virtual void PlayVideo()
        {
            if (videoPlayer.isPrepared)
            {
                videoPlayer.Play();
                M5Event enableEvent = VideoEvents.CreatePlayStartEvent(this, this, Url);
                EventManager.TriggerEvent(enableEvent);
            }
            else
            {
                isPlayAfterPrepare = true;
                videoPlayer.Prepare();
            }

            if (togglePlay != null)
            {
                togglePlay.isOn = false;
            }
        }

        [ContextMenu("Pause")]
        public virtual void PauseVideo()
        {
            if(videoPlayer.isPlaying)
            {
                videoPlayer.Pause();
                M5Event enableEvent = VideoEvents.CreatePlayPauseEvent(this, this, Url);
                EventManager.TriggerEvent(enableEvent);
            }

            if (togglePlay != null)
            {
                togglePlay.isOn = true;
            }
        }

        public virtual void StopVideo()
        {
            if (videoPlayer.isPlaying)
            {
                videoPlayer.Stop();
                M5Event enableEvent = VideoEvents.CreatePlayStopEvent(this, this, Url);
                EventManager.TriggerEvent(enableEvent);
            }

            if (togglePlay != null)
            {
                togglePlay.isOn = true;
            }
        }

        protected virtual void OnDestroy()
        {
            videoPlayer.Stop();
            videoPlayer.prepareCompleted -= OnVideoPrepared;
            videoPlayer.loopPointReached -= OnVideoEnd;
            videoPlayer.errorReceived -= OnVideoError;
        }
    }
}