using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using Module5.DI;
namespace Module5.Player
{
    [Alias("Video Player")]
    [ExposeToEventHandler]
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

        [ExposeToEventHandler(true, false)]
        public bool IsVideoPlaying => VideoPlayer.isPlaying;

        [ExposeToEventHandler(true, false)]
        public bool IsVideoPrepared => VideoPlayer.isPrepared;

        [ExposeToEventHandler(true, false)]
        public bool IsPreparing => isPreparing;

        [SerializeField] protected AudioSource audioSource;
        public AudioSource AudioSource
        {
            get
            {
                if (audioSource == null) audioSource = GetComponentInChildren<AudioSource>(true);
                return audioSource;
            }
            set => audioSource = value;
        }

        [SerializeField] protected Text textErrorMsg;
        [SerializeField] protected bool showPlayButton = true;
        public virtual bool ShowPlayButton
        {
            get => showPlayButton;
            set
            {
                showPlayButton = value;
                UpdateUIState(videoPlayer != null && videoPlayer.isPlaying);
            }
        }

        [ExposeToEventHandler(true, false)]
        public virtual string Url
        {
            get => VideoPlayer.url;
            set
            {
                VideoPlayer.url = value;
                SetupAudioTrack();
            }
        }

        [ExposeToEventHandler(true, false)]
        public virtual bool IsLoop
        {
            get => VideoPlayer.isLooping;
            set => VideoPlayer.isLooping = value;
        }

        [ExposeToEventHandler(true, false)]
        public bool playOnAwake = true;

        protected bool isPlayAfterPrepare = false;
        protected bool isPreparing = false;

        protected virtual void Awake()
        {
            if (VideoPlayer != null)
            {
                VideoPlayer.playOnAwake = false;
                VideoPlayer.prepareCompleted += OnVideoPrepared;
                VideoPlayer.loopPointReached += OnVideoEnd;
                VideoPlayer.errorReceived += OnVideoError;
                SetupAudioTrack();
            }

            UpdateUIState(isPlaying: false);
        }

        public virtual void SetupAudioTrack()
        {
            if (VideoPlayer == null) return;

            videoPlayer.controlledAudioTrackCount = 1;
            videoPlayer.EnableAudioTrack(0, true);

            if (videoPlayer.audioOutputMode == VideoAudioOutputMode.AudioSource)
            {
                if (AudioSource != null)
                {
                    AudioSource.playOnAwake = false;
                    videoPlayer.SetTargetAudioSource(0, AudioSource);
                }
            }
            else if (videoPlayer.audioOutputMode == VideoAudioOutputMode.Direct)
            {
                videoPlayer.SetDirectAudioVolume(0, 1.0f);
                videoPlayer.SetDirectAudioMute(0, false);
            }
        }

        protected virtual void OnEnable()
        {
            if (VideoPlayer == null) return;

            if (videoPlayer.isPlaying)
            {
                if (playOnAwake)
                {
                    TriggerPlayStart();
                }
            }
            else if (videoPlayer.isPrepared)
            {
                if (playOnAwake)
                {
                    PlayVideo();
                }
            }
            else if (!string.IsNullOrEmpty(videoPlayer.url) && !isPreparing) 
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
            bool isSameUrl = (videoPlayer.url == url);

            if (isSameUrl)
            {
                this.isPlayAfterPrepare = isPlayAfterPrepare;
                videoPlayer.isLooping = isLoop;

                if (videoPlayer.isPrepared && isPlayAfterPrepare)
                {
                    PlayVideo();
                    return;
                }

                if (isPreparing)
                {
                    return;
                }
            }

            // 中途換片（或新片載入）：中斷舊影片的準備與播放
            videoPlayer.Stop();
            isPreparing = false;

            this.isPlayAfterPrepare = isPlayAfterPrepare;
            videoPlayer.url = url;
            videoPlayer.isLooping = isLoop;
            SetupAudioTrack();

            if (isActiveAndEnabled) Prepare();
        }

        public virtual void TogglePlayPause()
        {
            if (videoPlayer.isPlaying)
            {
                PauseVideo();
            }
            else
            {
                PlayVideo();
            }
        }

        protected virtual void Prepare()
        {
            if (videoPlayer.isPrepared || isPreparing) return;

            SetupAudioTrack();

            isPreparing = true;
            if (textErrorMsg != null) textErrorMsg.gameObject.SetActive(false);
            UpdateUIState(isPlaying: false);
            videoPlayer.Prepare();
        }

        protected virtual void OnVideoError(VideoPlayer source, string message)
        {
            Debug.LogError("VideoPlayer Error: " + message);
            isPreparing = false;
            isPlayAfterPrepare = false;
            if (textErrorMsg != null)
            {
                textErrorMsg.gameObject.SetActive(true);
                textErrorMsg.text = message;
            }
            videoPlayer.Stop();
            UpdateUIState(isPlaying: false);
        }

        protected virtual void OnVideoPrepared(VideoPlayer source)
        {
            isPreparing = false;

            if (source.audioTrackCount > 0)
            {
                source.controlledAudioTrackCount = (ushort)Mathf.Max(1, source.audioTrackCount);
                source.EnableAudioTrack(0, true);
                if (source.audioOutputMode == VideoAudioOutputMode.AudioSource && AudioSource != null)
                {
                    source.SetTargetAudioSource(0, AudioSource);
                }
            }

            if (isPlayAfterPrepare)
            {
                if (videoPlayer.isPlaying)
                {
                    TriggerPlayStart();
                    UpdateUIState(isPlaying: true);
                }
                else
                {
                    PlayVideo();
                }
            }
            else
            {
                videoPlayer.Pause();
                videoPlayer.frame = 0;
                UpdateUIState(isPlaying: false);
            }
        }

        protected virtual void OnVideoEnd(VideoPlayer source)
        {
            if (source.isLooping)
            {
                TriggerLoopPointReached();
            }
            else
            {
                source.Pause();
                TriggerPlayFinish();
                UpdateUIState(isPlaying: false);
            }
        }

        protected virtual void UpdateUIState(bool isPlaying)
        {
        }

        [ContextMenu("Play")]
        public virtual void PlayVideo()
        {
            if (!videoPlayer.isPlaying)
            {
                if (videoPlayer.isPrepared)
                {
                    videoPlayer.Play();
                    if (videoPlayer.audioOutputMode == VideoAudioOutputMode.AudioSource && AudioSource != null && !AudioSource.isPlaying)
                    {
                        AudioSource.Play();
                    }
                    TriggerPlayStart();
                }
                else
                {
                    isPlayAfterPrepare = true;
                    if (!isPreparing)
                    {
                        Prepare();
                    }
                }
            }

            UpdateUIState(isPlaying: true);
        }

        [ContextMenu("Pause")]
        public virtual void PauseVideo()
        {
            if (isPreparing)
            {
                isPlayAfterPrepare = false;
            }

            if (videoPlayer.isPlaying)
            {
                videoPlayer.Pause();
                if (videoPlayer.audioOutputMode == VideoAudioOutputMode.AudioSource && AudioSource != null && AudioSource.isPlaying)
                {
                    AudioSource.Pause();
                }
                TriggerPlayPause();
            }

            UpdateUIState(isPlaying: false);
        }

        [ContextMenu("Stop")]
        public virtual void StopVideo()
        {
            if (isPreparing)
            {
                videoPlayer.Stop();
                isPreparing = false;
                isPlayAfterPrepare = false;
            }

            if (videoPlayer.isPrepared)
            {
                videoPlayer.Pause();
                videoPlayer.frame = 0;
                if (videoPlayer.audioOutputMode == VideoAudioOutputMode.AudioSource && AudioSource != null)
                {
                    AudioSource.Stop();
                }
                TriggerPlayStop();
            }

            UpdateUIState(isPlaying: false);
        }

        protected void TriggerPlayStart()
        {
            if (EventManager == null) return;
            M5Event enableEvent = new VideoEvents.PlayStartEvent(this, this, Url);
            EventManager.TriggerEvent(enableEvent);
        }

        protected void TriggerPlayPause()
        {
            if (EventManager == null) return;
            M5Event enableEvent = new VideoEvents.PlayPauseEvent(this, this, Url);
            EventManager.TriggerEvent(enableEvent);
        }

        protected void TriggerPlayStop()
        {
            if (EventManager == null) return;
            M5Event enableEvent = new VideoEvents.PlayStopEvent(this, this, Url);
            EventManager.TriggerEvent(enableEvent);
        }

        protected void TriggerPlayFinish()
        {
            if (EventManager == null) return;
            M5Event enableEvent = new VideoEvents.PlayFinishEvent(this, this, Url);
            EventManager.TriggerEvent(enableEvent);
        }

        protected void TriggerLoopPointReached()
        {
            if (EventManager == null) return;
            M5Event loopEvent = new VideoEvents.LoopPointReachedEvent(this, this, Url);
            EventManager.TriggerEvent(loopEvent);
        }

        protected virtual void OnDestroy()
        {
            isPreparing = false;
            if (videoPlayer != null)
            {
                videoPlayer.Stop();
                videoPlayer.prepareCompleted -= OnVideoPrepared;
                videoPlayer.loopPointReached -= OnVideoEnd;
                videoPlayer.errorReceived -= OnVideoError;
            }
        }
    }
}