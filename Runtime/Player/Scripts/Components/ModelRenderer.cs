using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using GLTFast;
using UnityEngine;
using Module5.DI;
namespace Module5.Player
{

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

        [SerializeField] protected Animation m_Animation;
        public Animation Animation => m_Animation;

        public bool playAutomatically = true;

        public bool IsAnimationPlaying => m_Animation != null ? m_Animation.isPlaying : false;
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

        protected virtual void OnEnable()
        {
            if (m_Animation == null) return;

            if (m_Animation.playAutomatically && m_Animation.clip != null)
            {
                TriggerAnimationPlayStart(m_Animation.clip.name);
            }
        }

        private void OnDestroy()
        {
            if (constructToken != null)
            {
                constructToken.Cancel();
                constructToken.Dispose();
                constructToken = null;
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
            if (root != null) Destroy(root);
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
            constructToken.Dispose();
            constructToken = null;
        }

        protected virtual void OnConstructSuccess()
        {
            m_Animation = root.GetComponent<Animation>();
            if (m_Animation != null)
            {
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
            if (AssetDataBaseManager == null || EventManager == null) return;

            M5Event animationEvent = AnimationEvents.CreatePlayStartEvent(this, this, clipName);
            EventManager.TriggerEvent(animationEvent);

            if (monitorCoroutine != null) StopCoroutine(monitorCoroutine);
            monitorCoroutine = StartCoroutine(MonitorAnimationFinish(clipName));
        }

        protected void TriggerAnimationPlayFinish(string clipName)
        {
            if (AssetDataBaseManager == null || EventManager == null) return;

            M5Event animationEvent = AnimationEvents.CreatePlayFinishEvent(this, this, clipName);
            EventManager.TriggerEvent(animationEvent);
        }

        private IEnumerator MonitorAnimationFinish(string clipName)
        {
            if (m_Animation == null) yield break;
            var state = m_Animation[clipName];
            if (state == null) yield break;

            // Wait for it to start (if it hasn't)
            while (!m_Animation.IsPlaying(clipName))
                yield return null;

            // Wait for it to finish or be stopped
            while (m_Animation.IsPlaying(clipName))
            {
                // If it's looping, it will never "finish" normally unless switched or stopped.
                // We skip "Finish" for loops to match common event expectations.
                if (state.wrapMode == WrapMode.Loop || state.wrapMode == WrapMode.PingPong)
                    yield break;

                yield return null;
            }

            // Trigger Finish Event
            TriggerAnimationPlayFinish(clipName);
            monitorCoroutine = null;
        }

        public void PlayAnimation(string clipName, float fadeLength = 0.3f)
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
                        state.speed = 1;
                        hasPausedClip = true;
                        TriggerAnimationPlayStart(state.name);
                    }
                }

                // 如果沒有被暫停的動畫，才嘗試播放預設動畫
                if (!hasPausedClip && !m_Animation.isPlaying)
                {
                    // 檢查是否有預設 Clip
                    if (m_Animation.clip != null)
                    {
                        // 對預設 Clip 執行 CrossFade
                        m_Animation.CrossFade(m_Animation.clip.name, fadeLength);
                        TriggerAnimationPlayStart(m_Animation.clip.name);
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
                if (state != null)
                {
                    state.speed = 1; // 確保速度是正常的
                                     // 如果當前已經在播這個動作了，就不重複 CrossFade (避免抖動)
                    if (!m_Animation.IsPlaying(clipName))
                    {
                        m_Animation.CrossFade(clipName, fadeLength);
                        TriggerAnimationPlayStart(clipName);
                    }
                }
            }
        }

        public void PauseAnimation()
        {
            if (m_Animation == null) return;
            foreach (AnimationState state in m_Animation)
            {
                // 只有正在作用 (Weight > 0) 的動畫才需要暫停
                if (state.enabled && state.weight > 0)
                {
                    state.speed = 0;
                }
            }
        }

        public void StopAnimation()
        {
            if (m_Animation == null) return;
            m_Animation.Stop();
        }
    }
}