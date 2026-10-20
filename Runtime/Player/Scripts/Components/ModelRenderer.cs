using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using GLTFast;
using UnityEngine;
using Module5.DI;
namespace Module5.Player
{

    public enum AnimationPlayState
    {
        Stopped,
        Playing,
        Paused
    }

    public class ModelConstructResult
    {
        public bool success;
        public Animation animation;
    }

    public interface IModelData
    {
        string Name { get; set; }
        Task<bool> Construct(Transform root, CancellationToken cancellationToken);
        void Dispose();
    }

    [Alias("Model Renderer")]
    [ExposeToEventHandler]
    public class ModelRenderer : MonoBehaviour
    {
        protected GameObject root;
        protected CancellationTokenSource constructToken;

        protected IModelData modelData;
        [RegisterReference]
        public IModelData ModelData
        {
            get => modelData;
            set
            {
                ReleaseOldModel(value);
                if (modelData == null)
                {
                    return;
                }
                Construct();
            }
        }
        [Inject] public IAssetDataBaseManager AssetDataBaseManager { get; set; }
        [Inject] public IEventManager EventManager { get; set; }

        private Coroutine monitorCoroutine;
        protected string currentClipName;

        [SerializeField] protected Animation m_Animation;
        public Animation Animation => m_Animation;

        [ExposeToEventHandler(canRead: true, canWrite: true)]
        public bool playAutomatically = true;

        [SerializeField] public string currentClip;

        [SerializeField] public WrapMode currentWrapMode = WrapMode.Default;

        [ExposeToEventHandler(true, false)]
        public bool IsAnimationPlaying => playState == AnimationPlayState.Playing;

        [ExposeToEventHandler(true, false)]
        public bool IsAnimationPaused => playState == AnimationPlayState.Paused;

        [ExposeToEventHandler(true, false)]
        public bool IsAnimationStopped => playState == AnimationPlayState.Stopped;

        public AnimationPlayState PlayState => playState;
        protected AnimationPlayState playState = AnimationPlayState.Stopped;

        public string CurrentAnimationName
        {
            get
            {
                if (m_Animation == null) return "";
                foreach (AnimationState state in m_Animation)
                {
                    if (m_Animation.IsPlaying(state.name))
                    {
                       return state.name;
                    }
                }
                return "";
            }
        }

        public string EnsureValidClip()
        {
            if (m_Animation == null)
            {
                return "";
            }

            // 檢查現有的 currentClip 是否有效
            if (!string.IsNullOrEmpty(currentClip) && m_Animation[currentClip] != null)
            {
                return currentClip;
            }

            // 若無效或未設定，則嘗試取得模型的預設 Clip
            string defaultClipName = "";
            if (m_Animation.clip != null)
            {
                defaultClipName = m_Animation.clip.name;
            }
            else
            {
                foreach (AnimationState state in m_Animation)
                {
                    if (state.clip != null)
                    {
                        defaultClipName = state.name;
                        m_Animation.clip = state.clip;
                        break;
                    }
                }
            }

            // 更新 currentClip 為模型預設的 Clip
            currentClip = defaultClipName;
            return currentClip;
        }

        [ExposeToEventHandler(canRead: true, canWrite: false)]
        public virtual string CurrentClip
        {
            get
            {
                if (m_Animation == null) return currentClip ?? "";
                string playing = CurrentAnimationName;
                if (!string.IsNullOrEmpty(playing))
                {
                    return playing;
                }
                return EnsureValidClip();
            }
            set
            {
                currentClip = value;
                if (m_Animation == null) return;
                EnsureValidClip();
                if (Application.isPlaying && m_Animation.isPlaying)
                {
                    PlayAnimation(currentClip, 0.3f, CurrentWrapMode);
                }
            }
        }

        [ExposeToEventHandler(canRead: true, canWrite: false)]
        public virtual WrapMode CurrentWrapMode
        {
            get
            {
                if (m_Animation == null) return currentWrapMode;
                string clip = CurrentClip;
                if (!string.IsNullOrEmpty(clip))
                {
                    var state = m_Animation[clip];
                    if (state != null && state.wrapMode != WrapMode.Default)
                    {
                        return state.wrapMode;
                    }
                }
                return currentWrapMode;
            }
            set
            {
                currentWrapMode = value;
                if (m_Animation == null) return;
                string clip = CurrentClip;
                if (!string.IsNullOrEmpty(clip))
                {
                    var state = m_Animation[clip];
                    if (state != null)
                    {
                        state.wrapMode = value;
                    }
                }
            }
        }

        protected virtual void OnEnable()
        {
            if (m_Animation == null) return;

            ApplyAutoPlaySettings();

            if (m_Animation.playAutomatically && m_Animation.clip != null)
            {
                TriggerAnimationPlayStart(m_Animation.clip.name);
            }
        }

        protected virtual void OnDisable()
        {
            StopMonitorCoroutine();
        }

        private void OnDestroy()
        {
            if (constructToken != null)
            {
                constructToken.Cancel();
                constructToken.Dispose();
                constructToken = null;
            }
            StopMonitorCoroutine();
            m_Animation = null;
        }

        private void StopMonitorCoroutine()
        {
            if (monitorCoroutine != null)
            {
                StopCoroutine(monitorCoroutine);
                monitorCoroutine = null;
            }
        }

        public async Task SetModelData(IModelData data)
        {
            ReleaseOldModel(data);
            if (modelData == null)
            {
                return;
            }
            constructToken = new CancellationTokenSource();
            await Construct(constructToken.Token);
        }

        void ReleaseOldModel(IModelData data)
        {
            if (modelData != data)
            {
                modelData = data;
            }
            if (constructToken != null)
            {
                constructToken.Cancel();
                constructToken.Dispose();
                constructToken = null;
            }
            StopMonitorCoroutine();
            if (root != null) Destroy(root);
            m_Animation = null;
        }

        [ContextMenu("Construct")]
        async void Construct()
        {
            constructToken = new CancellationTokenSource();
            await Construct(constructToken.Token);
            
        }

        async Task Construct(CancellationToken cancellationToken)
        {
            if (modelData != null)
            {
                root = new GameObject("Root");
                root.transform.SetParent(transform, false);
                root.transform.SetSiblingIndex(0);
                root.SetActive(false);
                if (!await modelData.Construct(root.transform, cancellationToken))
                {
                    Debug.LogError(name + " : 3D model construct fail.");
                    Destroy(root);
                }
                else
                {
                    OnConstructSuccess();
                    root.SetActive(true);
                }
            }
            constructToken?.Dispose();
            constructToken = null;
        }

        public void ApplyAutoPlaySettings()
        {
            if (m_Animation == null) return;

            EnsureValidClip();

            if (!string.IsNullOrEmpty(currentClip))
            {
                var state = m_Animation[currentClip];
                if (state != null)
                {
                    if (currentWrapMode != WrapMode.Default)
                    {
                        state.wrapMode = currentWrapMode;
                    }
                    if (state.clip != null)
                    {
                        m_Animation.clip = state.clip;
                    }
                }
            }
            else if (m_Animation.clip != null && currentWrapMode != WrapMode.Default)
            {
                var state = m_Animation[m_Animation.clip.name];
                if (state != null)
                {
                    state.wrapMode = currentWrapMode;
                }
            }
        }

        protected virtual void OnConstructSuccess()
        {
            m_Animation = root != null ? root.GetComponent<Animation>() : null;
            if (m_Animation != null)
            {
                EnsureValidClip();
                ApplyAutoPlaySettings();
                m_Animation.playAutomatically = playAutomatically;
                if (playAutomatically && m_Animation.clip != null)
                {
                    if (gameObject.activeInHierarchy)
                    {
                        TriggerAnimationPlayStart(m_Animation.clip.name);
                    }
                }
            }
        }

        protected void TriggerAnimationPlayStart(string clipName)
        {
            currentClipName = clipName;
            currentClip = clipName;
            playState = AnimationPlayState.Playing;
            if (m_Animation != null && !string.IsNullOrEmpty(clipName))
            {
                var state = m_Animation[clipName];
                if (state != null && state.wrapMode != WrapMode.Default)
                {
                    currentWrapMode = state.wrapMode;
                }
            }
            if (AssetDataBaseManager == null || EventManager == null) return;

            M5Event animationEvent = new AnimationEvents.PlayStartEvent(this, this, clipName);
            EventManager.TriggerEvent(animationEvent);

            StopMonitorCoroutine();
            monitorCoroutine = StartCoroutine(MonitorAnimationFinish(clipName));
        }

        protected void TriggerAnimationPlayPause(string clipName)
        {
            if (AssetDataBaseManager == null || EventManager == null) return;

            M5Event animationEvent = new AnimationEvents.PlayPauseEvent(this, this, clipName);
            EventManager.TriggerEvent(animationEvent);
        }

        protected void TriggerAnimationPlayStop(string clipName)
        {
            if (AssetDataBaseManager == null || EventManager == null) return;

            M5Event animationEvent = new AnimationEvents.PlayStopEvent(this, this, clipName);
            EventManager.TriggerEvent(animationEvent);
        }

        protected void TriggerAnimationPlayFinish(string clipName)
        {
            if (AssetDataBaseManager == null || EventManager == null) return;

            M5Event animationEvent = new AnimationEvents.PlayFinishEvent(this, this, clipName);
            EventManager.TriggerEvent(animationEvent);
        }

        protected void TriggerAnimationPlayLoopPointReached(string clipName)
        {
            if (AssetDataBaseManager == null || EventManager == null) return;

            M5Event animationEvent = new AnimationEvents.LoopPointReachedEvent(this, this, clipName);
            EventManager.TriggerEvent(animationEvent);
        }

        private IEnumerator MonitorAnimationFinish(string clipName)
        {
            if (m_Animation == null)
            {
                monitorCoroutine = null;
                yield break;
            }
            var state = m_Animation[clipName];
            if (state == null)
            {
                monitorCoroutine = null;
                yield break;
            }

            // Wait for it to start (if it hasn't)
            while (m_Animation != null && !m_Animation.IsPlaying(clipName))
                yield return null;

            if (m_Animation == null)
            {
                monitorCoroutine = null;
                yield break;
            }

            float previousNormalizedTime = state.normalizedTime;

            // Wait for it to finish or reach loop point
            while (m_Animation != null && (m_Animation.IsPlaying(clipName) || playState == AnimationPlayState.Paused))
            {
                if (playState == AnimationPlayState.Paused)
                {
                    yield return null;
                    continue;
                }

                if (state == null)
                {
                    monitorCoroutine = null;
                    yield break;
                }

                WrapMode effectiveWrapMode = state.wrapMode;
                if (effectiveWrapMode == WrapMode.Default)
                {
                    if (state.clip != null && state.clip.wrapMode != WrapMode.Default)
                        effectiveWrapMode = state.clip.wrapMode;
                    else if (m_Animation != null)
                        effectiveWrapMode = m_Animation.wrapMode;
                }

                if (effectiveWrapMode == WrapMode.Loop)
                {
                    if (Mathf.FloorToInt(state.normalizedTime) > Mathf.FloorToInt(previousNormalizedTime))
                    {
                        TriggerAnimationPlayLoopPointReached(clipName);
                    }
                    previousNormalizedTime = state.normalizedTime;
                }

                yield return null;
            }

            if (m_Animation == null)
            {
                monitorCoroutine = null;
                yield break;
            }

            WrapMode finalWrapMode = state.wrapMode;
            if (finalWrapMode == WrapMode.Default)
            {
                if (state.clip != null && state.clip.wrapMode != WrapMode.Default)
                    finalWrapMode = state.clip.wrapMode;
                else if (m_Animation != null)
                    finalWrapMode = m_Animation.wrapMode;
            }

            // If it was looping, it will not trigger Finish
            if (finalWrapMode != WrapMode.Loop && finalWrapMode != WrapMode.PingPong && playState != AnimationPlayState.Paused)
            {
                playState = AnimationPlayState.Stopped;
                TriggerAnimationPlayFinish(clipName);
            }

            monitorCoroutine = null;
        }

        public void PlayAnimation(string clipName, float fadeLength = 0.3f, WrapMode wrapMode = WrapMode.Default)
        {
            if (m_Animation == null) return;

            // 情況 A：未指定名稱 -> 執行「恢復播放 (Resume)」
            if (string.IsNullOrEmpty(clipName))
            {
                bool hasPausedClip = false;
                foreach (AnimationState state in m_Animation)
                {
                    // 檢查是否處於「被手動暫停」的狀態
                    if (state.enabled && state.speed == 0 && state.weight > 0)
                    {
                        if (wrapMode != WrapMode.Default)
                        {
                            state.wrapMode = wrapMode;
                            currentWrapMode = wrapMode;
                        }
                        state.speed = 1;
                        hasPausedClip = true;
                        TriggerAnimationPlayStart(state.name);
                    }
                }

                // 如果沒有被暫停的動畫，才嘗試播放預設動畫
                if (!hasPausedClip && !m_Animation.isPlaying)
                {
                    string targetClip = EnsureValidClip();
                    if (!string.IsNullOrEmpty(targetClip))
                    {
                        var state = m_Animation[targetClip];
                        if (state != null && wrapMode != WrapMode.Default)
                        {
                            state.wrapMode = wrapMode;
                            currentWrapMode = wrapMode;
                        }
                        // 對預設 Clip 執行 CrossFade
                        m_Animation.CrossFade(targetClip, fadeLength);
                        TriggerAnimationPlayStart(targetClip);
                    }
                    else
                    {
                        m_Animation.Play(); // 沒預設 Clip 就用一般的 Play
                        TriggerAnimationPlayStart(m_Animation.clip?.name ?? "");
                    }
                }
            }
            // 情況 B：指定名稱 -> 切換動畫
            else
            {
                var state = m_Animation[clipName];
                if (state == null)
                {
                    Debug.LogWarning($"[ModelRenderer] Clip '{clipName}' not found in Animation. Fallback to default clip.");
                    clipName = EnsureValidClip();
                    state = !string.IsNullOrEmpty(clipName) ? m_Animation[clipName] : null;
                }

                if (state != null)
                {
                    if (wrapMode != WrapMode.Default)
                    {
                        state.wrapMode = wrapMode;
                        currentWrapMode = wrapMode;
                    }
                    state.speed = 1; // 確保速度是正常的
                                     // 如果當前已經在播這個動作了，就不重複 CrossFade (避免抖動)
                    if (!m_Animation.IsPlaying(clipName))
                    {
                        m_Animation.CrossFade(clipName, fadeLength);
                        TriggerAnimationPlayStart(clipName);
                    }
                    else if (playState == AnimationPlayState.Paused)
                    {
                        TriggerAnimationPlayStart(clipName);
                    }
                }
            }
        }

        public void PauseAnimation()
        {
            if (m_Animation == null) return;
            // 只有在播放中 (Playing) 才能暫停；Stop 或 Paused 狀態下不能觸發 Pause
            if (playState != AnimationPlayState.Playing) return;

            string pausedClipName = null;
            foreach (AnimationState state in m_Animation)
            {
                // 只有正在作用 (Weight > 0) 的動畫才需要暫停
                if (state.enabled && state.weight > 0)
                {
                    state.speed = 0;
                    if (pausedClipName == null)
                    {
                        pausedClipName = state.name;
                    }
                }
            }

            playState = AnimationPlayState.Paused;

            string clipToReport = pausedClipName ?? (!string.IsNullOrEmpty(currentClipName) ? currentClipName : CurrentAnimationName);
            if (!string.IsNullOrEmpty(clipToReport))
            {
                TriggerAnimationPlayPause(clipToReport);
            }
        }

        public void StopAnimation()
        {
            if (m_Animation == null) return;
            // 若已經是 Stopped 狀態，則不重複執行 Stop
            if (playState == AnimationPlayState.Stopped) return;

            string stoppedClipName = !string.IsNullOrEmpty(currentClipName) ? currentClipName : CurrentAnimationName;
            if (string.IsNullOrEmpty(stoppedClipName) && m_Animation.clip != null)
            {
                stoppedClipName = m_Animation.clip.name;
            }

            // 若有因暫停而被設為 0 的 state speed，重置為正常速度 1
            foreach (AnimationState state in m_Animation)
            {
                if (state.speed == 0)
                {
                    state.speed = 1;
                }
            }

            StopMonitorCoroutine();
            m_Animation.Stop();
            playState = AnimationPlayState.Stopped;

            if (!string.IsNullOrEmpty(stoppedClipName))
            {
                TriggerAnimationPlayStop(stoppedClipName);
            }
        }
    }
}