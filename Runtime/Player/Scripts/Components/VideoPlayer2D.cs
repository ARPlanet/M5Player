using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using System.Collections;

namespace Module5.Player
{
    public class VideoPlayer2D : VideoPlayerBase
    {
        [SerializeField] protected RawImage output;

        protected RenderTexture renderTexture;


        protected override void Awake()
        {
            base.Awake();
            // 確保 VideoPlayer 的 RenderMode 為 Render Texture
            videoPlayer.renderMode = VideoRenderMode.RenderTexture;
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            FitVideoAspect();
        }

        protected virtual void OnRectTransformDimensionsChange()
        {
            FitVideoAspect();
        }

        protected override void Prepare()
        {
            // 清理之前的資源
            ReleaseTexture();
            base.Prepare();
        }

        protected virtual void FitVideoAspect()
        {
            RectTransform parent = output.transform.parent as RectTransform;

            if (!videoPlayer.isPrepared)
            {
                output.rectTransform.sizeDelta = new Vector2(parent.rect.width, parent.rect.height);
                return;
            }
            // **長寬比調整邏輯**
            // 取得影片和父物件的寬高
            float videoWidth = videoPlayer.width;
            float videoHeight = videoPlayer.height;
            float parentWidth = parent.rect.width;
            float parentHeight = parent.rect.height;

            // 計算影片和父物件的長寬比
            float videoAspectRatio = videoWidth / videoHeight;
            float parentAspectRatio = parentWidth / parentHeight;

            // 根據長寬比決定是填滿水平還是垂直空間
            if (videoAspectRatio > parentAspectRatio)
            {
                // 影片比父物件寬，以水平為基準
                float newHeight = parentWidth / videoAspectRatio;
                output.rectTransform.sizeDelta = new Vector2(parentWidth, newHeight);
            }
            else
            {
                // 影片比父物件高，以垂直為基準
                float newWidth = parentHeight * videoAspectRatio;
                output.rectTransform.sizeDelta = new Vector2(newWidth, parentHeight);
            }
        }

        protected override void OnVideoError(VideoPlayer source, string message)
        {
            base.OnVideoError(source, message);
            ReleaseTexture();
        }

        protected override void OnVideoPrepared(VideoPlayer source)
        {
            FitVideoAspect();
            // 動態創建 RenderTexture
            renderTexture = new RenderTexture((int)source.width, (int)source.height, 24);

            source.targetTexture = renderTexture;
            if (output != null)
            {
                output.texture = renderTexture;
                output.color = Color.white;
            }
            base.OnVideoPrepared(source);
        }

        protected override void OnVideoEnd(VideoPlayer source)
        {
            base.OnVideoEnd(source);
        }

        protected override void OnTogglePlayChanged(bool value)
        {
            base.OnTogglePlayChanged(value);
        }

        protected virtual void ReleaseTexture()
        {
            if (renderTexture != null)
            {
                // 釋放 RenderTexture 資源
                renderTexture.Release();
                Destroy(renderTexture);
                renderTexture = null;
            }
            if (output != null)
            {
                output.texture = null;
                output.color = Color.black;
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            ReleaseTexture();
        }
    }
}