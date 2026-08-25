using UnityEngine;
using UnityEngine.UI;

namespace Module5.Player 
{
    public interface IProgressBarManager
    {
        void Show();
        void UpdatePosition(Vector2 screenPosition);
        void SetProgress(float amount);
        void Hide();
    }

    public class ProgressBarManager : MonoBehaviour, IProgressBarManager
    {
        [Header("UI 參考")]
        public GameObject progressRoot;
        public Image fillImage;
        public RectTransform rectTransform; // 進度條本身的 RectTransform

        private Canvas parentCanvas;

        void Awake()
        {
            parentCanvas = GetComponentInParent<Canvas>();
            Hide();
        }

        public void Show()
        {
            progressRoot.SetActive(true);
        }

        public void UpdatePosition(Vector2 screenPosition)
        {
            // 將螢幕點轉換為 Canvas 內的區域座標
            Vector2 localPos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                parentCanvas.transform as RectTransform,
                screenPosition,
                parentCanvas.worldCamera,
                out localPos
            );
            rectTransform.anchoredPosition = localPos;
        }

        public void SetProgress(float amount)
        {
            if (fillImage != null)
            {
                fillImage.fillAmount = Mathf.Clamp01(amount);
                // 滿了變綠色，沒滿白色
                fillImage.color = amount >= 1f ? Color.green : Color.white;
            }
        }

        public void Hide()
        {
            progressRoot.SetActive(false);
        }
    }
}