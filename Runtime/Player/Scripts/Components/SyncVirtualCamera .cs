using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Module5.Player
{
    /// <summary>
    /// 虛擬相機（Virtual Camera）基礎控制器：支援短邊 FOV 自適應與外部 FOV 覆蓋（如 VR360 / VideoPlayer360）
    /// </summary>
    public class SyncVirtualCamera : MonoBehaviour
    {
        public Camera virtualCamera;

        [Header("FOV Settings (Non-AR Mode)")]
        [SerializeField] protected float defaultFov = 60f;
        public float DefaultFov
        {
            get => defaultFov;
            set
            {
                defaultFov = value;
                UpdateFov();
            }
        }

        [SerializeField] protected bool enableAdaptiveFov = true;
        public bool EnableAdaptiveFov
        {
            get => enableAdaptiveFov;
            set
            {
                enableAdaptiveFov = value;
                UpdateFov();
            }
        }

        [Header("FOV Override (360 Video / VR)")]
        [SerializeField] protected bool isOverrideFov = false;
        public bool IsOverrideFov => isOverrideFov;

        [SerializeField] protected float overrideFov = 60f;
        public float OverrideFov => overrideFov;

        public float CurrentTargetFov => isOverrideFov ? overrideFov : defaultFov;

        /// <summary>
        /// 設定外部覆蓋 FOV（如 VideoPlayer360 / VR360）。
        /// 當 isOverride 為 true 時，VirtualCamera 使用自訂義覆蓋 FOV（且支援短邊自適應）；
        /// 為 false 時恢復預設 FOV（或由衍生類別恢復 AR 同步）。
        /// </summary>
        public virtual void SetFovOverride(bool isOverride, float targetFov = 60f)
        {
            isOverrideFov = isOverride;
            if (isOverride)
            {
                overrideFov = targetFov;
            }
            if (virtualCamera != null)
            {
                virtualCamera.ResetProjectionMatrix();
            }
            OnCameraSync();
        }

        protected virtual void Awake()
        {
            if (virtualCamera == null)
            {
                virtualCamera = GetComponent<Camera>();
            }
        }

        protected virtual void OnEnable()
        {
            if (virtualCamera != null)
            {
                virtualCamera.ResetProjectionMatrix();
            }
            OnCameraSync();
            UnityEngine.Rendering.RenderPipelineManager.beginCameraRendering += OnBeginCameraRendering;
        }

        protected virtual void OnDisable()
        {
            if (virtualCamera != null)
            {
                virtualCamera.ResetProjectionMatrix();
            }
            UnityEngine.Rendering.RenderPipelineManager.beginCameraRendering -= OnBeginCameraRendering;
        }

        protected virtual void LateUpdate()
        {
            if (ShouldUpdateFovInLateUpdate())
            {
                UpdateFov();
            }
        }

        /// <summary>
        /// 控制 LateUpdate 中是否隨螢幕/視口長寬比動態調整 FOV。
        /// 預設情況下（非 AR）每幀動態自適應；AR 模式可覆寫為在 AR 投影控制時暫停。
        /// </summary>
        protected virtual bool ShouldUpdateFovInLateUpdate()
        {
            return true;
        }

        private void OnBeginCameraRendering(UnityEngine.Rendering.ScriptableRenderContext context, Camera camera)
        {
            if (camera == virtualCamera && virtualCamera != null)
            {
                OnCameraSync();
            }
        }

        protected virtual void OnPreRender()
        {
            // Fallback for Built-in Pipeline
            if (virtualCamera != null)
            {
                OnCameraSync();
            }
        }

        /// <summary>
        /// 相機同步核心生命週期。基類預設執行自適應 FOV；衍生類別（如 AR）可覆寫以執行實體相機投影同步。
        /// </summary>
        protected virtual void OnCameraSync()
        {
            UpdateFov();
        }

        /// <summary>
        /// 短邊 FOV 自適應：橫擺時垂直鎖定目標 FOV，直立時水平鎖定目標 FOV
        /// </summary>
        public virtual void UpdateFov()
        {
            if (virtualCamera == null) return;

            float targetFov = CurrentTargetFov;

            if (!enableAdaptiveFov)
            {
                virtualCamera.fieldOfView = targetFov;
                return;
            }

            float aspect = GetAspect();
            if (aspect <= 0f) return;

            if (aspect >= 1.0f)
            {
                // 橫擺 (寬 >= 高)：短邊為高度，垂直 FOV 鎖定為基準 FOV
                virtualCamera.fieldOfView = targetFov;
            }
            else
            {
                // 直立 (高 > 寬)：短邊為寬度，利用 Unity 原生 API 將基準水平 FOV 換算為垂直 FOV
                virtualCamera.fieldOfView = Camera.HorizontalToVerticalFieldOfView(targetFov, aspect);
            }
        }

        protected virtual float GetAspect()
        {
            if (virtualCamera == null) return 1.0f;
            if (virtualCamera.targetTexture != null && virtualCamera.targetTexture.height > 0)
            {
                return (float)virtualCamera.targetTexture.width / virtualCamera.targetTexture.height;
            }
            if (virtualCamera.pixelWidth > 0 && virtualCamera.pixelHeight > 0)
            {
                return (float)virtualCamera.pixelWidth / virtualCamera.pixelHeight;
            }
            return virtualCamera.aspect;
        }
    }
}

