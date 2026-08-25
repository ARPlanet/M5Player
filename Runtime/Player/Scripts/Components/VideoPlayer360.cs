using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;
using Module5.DI;
namespace Module5.Player
{
    public class VideoPlayer360 : VideoPlayerBase, IVR360
    {
        [Inject] ISceneManager SceneManager {  get; set; }

        [SerializeField] MeshRenderer meshRenderer;

        public float Size
        {
            get
            {
                return meshRenderer.transform.localScale.x;
            }
            set
            {
                meshRenderer.transform.localScale = new Vector3(value, value, value);
            }
        }

        [SerializeField] protected bool isOverrideFov = false;
        public bool IsOverrideFov
        {
            get => isOverrideFov;
            set
            {
                isOverrideFov = value;
                if (isActiveAndEnabled)
                {
                    SyncVirtualCamera syncVirtualCamera = SceneManager.VirtualCamera.GetComponent<SyncVirtualCamera>();
                    if (syncVirtualCamera != null)
                    {
                        syncVirtualCamera.enabled = !IsOverrideFov;
                    }
                    if (value)
                    {
                        SceneManager.VirtualCamera.fieldOfView = fov;
                    }
                }
            }
        }

        [SerializeField] protected float fov = 60f;
        public float Fov
        {
            get => fov;
            set
            {
                fov = value;
                if (isActiveAndEnabled && IsOverrideFov)
                {
                    SceneManager.VirtualCamera.fieldOfView = fov;
                }
            }
        }

        protected override void Awake()
        {
            base.Awake();
            // 確保 VideoPlayer 的 RenderMode 為 Render Texture
            videoPlayer.renderMode = VideoRenderMode.MaterialOverride;
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            if (isOverrideFov)
            {
                SyncVirtualCamera syncVirtualCamera = SceneManager.VirtualCamera.GetComponent<SyncVirtualCamera>();
                if (syncVirtualCamera != null)
                {
                    syncVirtualCamera.enabled = false;
                }
                SceneManager.VirtualCamera.fieldOfView = fov;
            }
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            SyncVirtualCamera syncVirtualCamera = SceneManager.VirtualCamera.GetComponent<SyncVirtualCamera>();
            if (syncVirtualCamera != null)
            {
                syncVirtualCamera.enabled = true;
            }
        }

        protected override void OnVideoEnd(VideoPlayer source)
        {
            base.OnVideoEnd(source);
            // 接M5Event
            Debug.Log("OnVideoEnd未實作串接M5Event");
        }

    }
}