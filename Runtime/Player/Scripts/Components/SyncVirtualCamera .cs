using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Module5.Player
{
    /// <summary>
    /// 同步 AR Camera 投影矩陣；在非 AR（純 3D）模式下支援短邊 FOV 自適應
    /// </summary>
    public class SyncVirtualCamera : MonoBehaviour
    {
        public Camera arCamera;
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
        /// 當 isOverride 為 true 時，VirtualCamera 暫停同步 ARCamera 投影矩陣，改用自訂義覆蓋 FOV（且支援短邊自適應）；
        /// 為 false 時恢復 AR 投影矩陣同步（非 AR 模式則恢復 defaultFov）。
        /// </summary>
        public void SetFovOverride(bool isOverride, float targetFov = 60f)
        {
            isOverrideFov = isOverride;
            if (isOverride)
            {
                overrideFov = targetFov;
                if (virtualCamera != null)
                {
                    // 解除實體 AR 硬體投影矩陣，允許自訂 fieldOfView 生效
                    virtualCamera.ResetProjectionMatrix();
                }
            }
            else
            {
                if (virtualCamera != null && arCamera != null)
                {
                    // 恢復 AR 模式投影矩陣同步
                    virtualCamera.ResetProjectionMatrix();
                    SyncFromARCamera();
                }
            }
            UpdateFov();
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
            if (virtualCamera != null && arCamera != null && !isOverrideFov)
            {
                virtualCamera.ResetProjectionMatrix();
                SyncFromARCamera();
            }
            else
            {
                if (virtualCamera != null)
                {
                    virtualCamera.ResetProjectionMatrix();
                }
                UpdateFov();
            }
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
            // 在非 AR 模式 或 360 覆蓋 FOV 模式下，隨螢幕/視口長寬比動態調整 FOV
            if (arCamera == null || isOverrideFov)
            {
                UpdateFov();
            }
        }

        private void OnBeginCameraRendering(UnityEngine.Rendering.ScriptableRenderContext context, Camera camera)
        {
            if (camera == virtualCamera && virtualCamera != null)
            {
                if (arCamera != null && !isOverrideFov)
                {
                    // AR 模式且未覆蓋 FOV：同步 Near/Far 與真實硬體投影矩陣 (包含實體鏡頭 FOV)
                    SyncFromARCamera();
                }
                else
                {
                    // 非 AR 模式 或 360 覆蓋 FOV 模式：使用自訂自適應 FOV
                    UpdateFov();
                }
            }
        }

        protected virtual void OnPreRender()
        {
            // Fallback for Built-in Pipeline
            if (virtualCamera != null)
            {
                if (arCamera != null && !isOverrideFov)
                {
                    SyncFromARCamera();
                }
                else
                {
                    UpdateFov();
                }
            }
        }

        /// <summary>
        /// 同步 ARCamera 的裁剪面 (nearClipPlane, farClipPlane) 與投影矩陣 (projectionMatrix)。
        /// 注意：必須先同步 near/far，最後再設定 projectionMatrix，避免被 Unity 內部重算矩陣覆蓋。
        /// </summary>
        public virtual void SyncFromARCamera()
        {
            if (virtualCamera == null || arCamera == null) return;

            if (!Mathf.Approximately(virtualCamera.nearClipPlane, arCamera.nearClipPlane))
            {
                virtualCamera.nearClipPlane = arCamera.nearClipPlane;
            }

            if (!Mathf.Approximately(virtualCamera.farClipPlane, arCamera.farClipPlane))
            {
                virtualCamera.farClipPlane = arCamera.farClipPlane;
            }

            virtualCamera.projectionMatrix = arCamera.projectionMatrix;
        }

        /// <summary>
        /// 短邊 FOV 自適應：支援外部覆蓋 FOV，橫擺時垂直鎖定目標 FOV，直立時水平鎖定目標 FOV
        /// </summary>
        public virtual void UpdateFov()
        {
            if (virtualCamera == null) return;
            // 只有在 AR 模式 且 未覆蓋 FOV 時，才不干涉相機（完全交由 AR 實體投影矩陣控制）
            if (arCamera != null && !isOverrideFov) return;

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

