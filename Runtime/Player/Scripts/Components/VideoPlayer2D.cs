using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using System.Collections;

namespace Module5.Player
{
    public class VideoPlayer2D : VideoPlayerBase
    {
        [SerializeField] protected RawImage output;
        [SerializeField] protected Button buttonPlay;
        [SerializeField] protected CanvasGroup overlayCanvasGroup;

        public Button ButtonPlay => buttonPlay;
        public CanvasGroup OverlayCanvasGroup => overlayCanvasGroup;

        protected RenderTexture renderTexture;

        public override bool ShowPlayButton
        {
            get => showPlayButton;
            set
            {
                base.ShowPlayButton = value;
                if (buttonPlay != null)
                {
                    buttonPlay.gameObject.SetActive(videoPlayer != null && videoPlayer.isPrepared && value);
                }
            }
        }

        protected override void Awake()
        {
            base.Awake();
            // 確保 VideoPlayer 的 RenderMode 為 Render Texture
            videoPlayer.renderMode = VideoRenderMode.RenderTexture;

            if (buttonPlay != null)
            {
                buttonPlay.onClick.AddListener(OnPlayButtonClicked);
                buttonPlay.gameObject.SetActive(false);

                if (overlayCanvasGroup == null)
                {
                    overlayCanvasGroup = buttonPlay.GetComponentInChildren<CanvasGroup>(true);
                }
            }

            UpdateUIState(isPlaying: false);
        }

        protected virtual void OnPlayButtonClicked()
        {
            TogglePlayPause();
        }

        public void CallOnPlayButtonClicked() => OnPlayButtonClicked();

        public void SetButtonPlay(Button button)
        {
            buttonPlay = button;
            if (buttonPlay != null && overlayCanvasGroup == null)
            {
                overlayCanvasGroup = buttonPlay.GetComponentInChildren<CanvasGroup>(true);
            }
        }

        public void SetOverlayCanvasGroup(CanvasGroup canvasGroup)
        {
            overlayCanvasGroup = canvasGroup;
        }

        protected override void UpdateUIState(bool isPlaying)
        {
            base.UpdateUIState(isPlaying);

            if (overlayCanvasGroup != null)
            {
                overlayCanvasGroup.alpha = isPlaying ? 0f : 1f;
                overlayCanvasGroup.blocksRaycasts = true;
            }
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
            if (buttonPlay != null) buttonPlay.gameObject.SetActive(false);
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
            if (buttonPlay != null) buttonPlay.gameObject.SetActive(false);
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
            if (buttonPlay != null) buttonPlay.gameObject.SetActive(showPlayButton);
        }

        protected override void OnVideoEnd(VideoPlayer source)
        {
            base.OnVideoEnd(source);
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
            if (buttonPlay != null)
            {
                buttonPlay.onClick.RemoveListener(OnPlayButtonClicked);
            }
            ReleaseTexture();
        }
    }
}