using System.Collections;
using UnityEngine;
using Module5.DI;

namespace Module5.Player
{
    [RequireComponent(typeof(AudioSource))]
    [ExposeToEventHandler]
    public class AudioPlayer : MonoBehaviour
    {
        [Inject] public IEventManager EventManager { get; set; }

        [SerializeField] protected AudioSource audioSource;
        public AudioSource AudioSource
        {
            get
            {
                if (audioSource == null) audioSource = GetComponent<AudioSource>();
                return audioSource;
            }
        }

        [RegisterReference]
        [ExposeToEventHandler(true, false)]
        public AudioClip Clip
        {
            get => AudioSource != null ? AudioSource.clip : null;
            set { if (AudioSource != null) AudioSource.clip = value; }
        }

        public bool PlayOnAwake
        {
            get => AudioSource != null && AudioSource.playOnAwake;
            set { if (AudioSource != null) AudioSource.playOnAwake = value; }
        }
        
        [ExposeToEventHandler(true, false)]
        public bool IsLoop
        {
            get => AudioSource != null && AudioSource.loop;
            set { if (AudioSource != null) AudioSource.loop = value; }
        }
        [ExposeToEventHandler(true, false)]
        public float PanStereo
        {
            get => AudioSource != null ? AudioSource.panStereo : 0f;
            set { if (AudioSource != null) AudioSource.panStereo = value; }
        }
        [ExposeToEventHandler(true, false)]
        public float SpatialBlend
        {
            get => AudioSource != null ? AudioSource.spatialBlend : 0f;
            set { if (AudioSource != null) AudioSource.spatialBlend = value; }
        }
        [ExposeToEventHandler(true, false)]
        public float Volume
        {
            get => AudioSource != null ? AudioSource.volume : 1f;
            set { if (AudioSource != null) AudioSource.volume = value; }
        }

        public float DopplerLevel
        {
            get => AudioSource != null ? AudioSource.dopplerLevel : 1f;
            set { if (AudioSource != null) AudioSource.dopplerLevel = value; }
        }

        public float Spread
        {
            get => AudioSource != null ? AudioSource.spread : 0f;
            set { if (AudioSource != null) AudioSource.spread = value; }
        }

        public AudioRolloffMode RolloffMode
        {
            get => AudioSource != null ? AudioSource.rolloffMode : AudioRolloffMode.Logarithmic;
            set { if (AudioSource != null) AudioSource.rolloffMode = value; }
        }
        [ExposeToEventHandler(true, false)]
        public float MinDistance
        {
            get => AudioSource != null ? AudioSource.minDistance : 1f;
            set { if (AudioSource != null) AudioSource.minDistance = value; }
        }
        [ExposeToEventHandler(true, false)]
        public float MaxDistance
        {
            get => AudioSource != null ? AudioSource.maxDistance : 500f;
            set { if (AudioSource != null) AudioSource.maxDistance = value; }
        }
        [ExposeToEventHandler(true, false)]
        public bool IsPlaying => AudioSource != null && AudioSource.isPlaying;

        [ExposeToEventHandler(true, false)]
        public bool IsPaused => isPaused;

        [ExposeToEventHandler(true, true)]
        public float CurrentTime
        {
            get => AudioSource != null ? AudioSource.time : 0f;
            set => Seek(value);
        }

        public bool IsSeeking { get; set; }

        protected bool isPaused;
        protected float previousTime;
        protected Coroutine monitorCoroutine;

        protected virtual void OnEnable()
        {
            isPaused = false;
            if (AudioSource != null && AudioSource.playOnAwake && AudioSource.isPlaying)
            {
                TriggerAudioPlayStart();
                StopMonitorCoroutine();
                monitorCoroutine = StartCoroutine(MonitorAudio());
            }
        }

        protected virtual void OnDisable()
        {
            isPaused = false;
            StopMonitorCoroutine();
        }

        public virtual void PlayAudio()
        {
            if (AudioSource == null) return;

            if (isPaused)
            {
                ResumeAudio();
                return;
            }

            if (AudioSource.isPlaying) return;

            AudioSource.Play();
            TriggerAudioPlayStart();

            StopMonitorCoroutine();
            monitorCoroutine = StartCoroutine(MonitorAudio());
        }

        public virtual void ResumeAudio()
        {
            if (AudioSource == null) return;

            if (isPaused)
            {
                AudioSource.UnPause();
                isPaused = false;
                TriggerAudioPlayStart();

                StopMonitorCoroutine();
                monitorCoroutine = StartCoroutine(MonitorAudio());
            }
        }

        public virtual void PauseAudio()
        {
            if (AudioSource == null) return;

            if (AudioSource.isPlaying)
            {
                isPaused = true;
                AudioSource.Pause();
                TriggerAudioPlayPause();
            }
        }

        public virtual void StopAudio()
        {
            if (AudioSource == null) return;

            isPaused = false;
            StopMonitorCoroutine();
            AudioSource.Stop();
            TriggerAudioPlayStop();
        }

        public virtual void Seek(float targetTime)
        {
            if (AudioSource == null || AudioSource.clip == null) return;

            targetTime = Mathf.Clamp(targetTime, 0f, AudioSource.clip.length);
            AudioSource.time = targetTime;
            previousTime = targetTime;
        }

        protected void StopMonitorCoroutine()
        {
            if (monitorCoroutine != null)
            {
                StopCoroutine(monitorCoroutine);
                monitorCoroutine = null;
            }
        }

        protected virtual IEnumerator MonitorAudio()
        {
            if (AudioSource == null || AudioSource.clip == null)
            {
                monitorCoroutine = null;
                yield break;
            }

            previousTime = AudioSource.time;

            while (AudioSource != null && (AudioSource.isPlaying || isPaused))
            {
                if (isPaused || IsSeeking)
                {
                    previousTime = AudioSource.time;
                    yield return null;
                    continue;
                }

                if (AudioSource.clip == null)
                {
                    break;
                }

                if (AudioSource.loop)
                {
                    // Check loop wrap: was near the end of the clip and wrapped back to start
                    float threshold = Mathf.Min(Mathf.Max(0.15f, Time.deltaTime * 2f), AudioSource.clip.length * 0.5f);
                    bool wrapped = AudioSource.time < previousTime;
                    bool wasNearEnd = (AudioSource.clip.length - previousTime) <= threshold;
                    bool isAtStart = AudioSource.time <= threshold;

                    if (wrapped && wasNearEnd && isAtStart)
                    {
                        TriggerAudioPlayLoopPointReached();
                    }
                    previousTime = AudioSource.time;
                }
                yield return null;
            }

            if (AudioSource == null)
            {
                monitorCoroutine = null;
                yield break;
            }

            // If not looping, not paused, and not seeking (natural playback finish)
            if (!AudioSource.loop && !isPaused && !IsSeeking)
            {
                TriggerAudioPlayFinish();
            }

            monitorCoroutine = null;
        }

        protected void TriggerAudioPlayStart()
        {
            if (EventManager == null) return;
            string clipName = Clip != null ? Clip.name : string.Empty;
            M5Event audioEvent = new AudioEvents.PlayStartEvent(this, this, clipName);
            EventManager.TriggerEvent(audioEvent);
        }

        protected void TriggerAudioPlayPause()
        {
            if (EventManager == null) return;
            string clipName = Clip != null ? Clip.name : string.Empty;
            M5Event audioEvent = new AudioEvents.PlayPauseEvent(this, this, clipName);
            EventManager.TriggerEvent(audioEvent);
        }

        protected void TriggerAudioPlayStop()
        {
            if (EventManager == null) return;
            string clipName = Clip != null ? Clip.name : string.Empty;
            M5Event audioEvent = new AudioEvents.PlayStopEvent(this, this, clipName);
            EventManager.TriggerEvent(audioEvent);
        }

        protected void TriggerAudioPlayFinish()
        {
            if (EventManager == null) return;
            string clipName = Clip != null ? Clip.name : string.Empty;
            M5Event audioEvent = new AudioEvents.PlayFinishEvent(this, this, clipName);
            EventManager.TriggerEvent(audioEvent);
        }

        protected void TriggerAudioPlayLoopPointReached()
        {
            if (EventManager == null) return;
            string clipName = Clip != null ? Clip.name : string.Empty;
            M5Event audioEvent = new AudioEvents.LoopPointReachedEvent(this, this, clipName);
            EventManager.TriggerEvent(audioEvent);
        }
    }
}
