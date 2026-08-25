using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using GLTFast;
using UnityEngine;

namespace Module5.Player
{
    /// <summary>
    /// 同步AR Camera投影矩陣
    /// </summary>
    public class SyncVirtualCamera : MonoBehaviour
    {
        public Camera arCamera;
        public Camera virtualCamera;

        protected virtual void OnEnable()
        {
            if (virtualCamera != null && arCamera != null)
            {
                virtualCamera.ResetProjectionMatrix();
                virtualCamera.projectionMatrix = arCamera.projectionMatrix;
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

        private void OnBeginCameraRendering(UnityEngine.Rendering.ScriptableRenderContext context, Camera camera)
        {
            if (camera == virtualCamera && virtualCamera != null && arCamera != null)
            {
                // 同步投影矩陣 (包含FOV)
                virtualCamera.projectionMatrix = arCamera.projectionMatrix;
            }
        }

        protected virtual void OnPreRender()
        {
            // Fallback for Built-in Pipeline
            if (virtualCamera != null && arCamera != null)
            {
                virtualCamera.projectionMatrix = arCamera.projectionMatrix;
            }
        }
    }
}