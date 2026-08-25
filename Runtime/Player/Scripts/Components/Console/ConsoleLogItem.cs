using System;
using UnityEngine;
using UnityEngine.UI;

namespace Module5.Player
{
    /// <summary>
    /// Console 中單筆 Log 條目的 UGUI 元件。
    /// 點擊後觸發 OnSelected 事件，由 ConsolePanel 在底部詳細區塊顯示完整訊息與 StackTrace。
    ///
    /// UGUI 結構（手動建立 Prefab）：
    /// ConsoleLogItem (此腳本掛載位置，含 Button 元件)
    /// ├── Background  (Image)  — 用於選取高亮
    /// ├── TypeIcon    (Image)  — 依 Log 類型顯示不同顏色
    /// ├── Timestamp   (Text)
    /// └── Message     (Text)  — 單行顯示，完整內容見底部詳細面板
    /// </summary>
    public class ConsoleLogItem : MonoBehaviour
    {
        [Header("參照")]
        [SerializeField] Image backgroundImage;
        [SerializeField] Image typeIcon;
        [SerializeField] Text timestampText;
        [SerializeField] Text messageText;
        [SerializeField] Button selectButton;   // 掛在整列的 Button

        [Header("類型顏色")]
        [SerializeField] Color logColor     = new Color(0.85f, 0.85f, 0.85f);
        [SerializeField] Color warningColor = new Color(1f, 0.85f, 0.3f);
        [SerializeField] Color errorColor   = new Color(1f, 0.35f, 0.35f);

        [Header("類型圖示")]
        [SerializeField] Sprite logSprite;
        [SerializeField] Sprite warningSprite;
        [SerializeField] Sprite errorSprite;

        [Header("選取顏色")]
        [SerializeField] Color normalBgColor   = new Color(0f, 0f, 0f, 0f);
        [SerializeField] Color selectedBgColor = new Color(0.17f, 0.36f, 0.53f, 1f);

        /// <summary>此 Item 被點擊選取時觸發，攜帶對應的 ConsoleLogEntry</summary>
        public event Action<ConsoleLogEntry> OnSelected;

        ConsoleLogEntry _entry;

        // ── 公開方法 ──────────────────────────────────────────────────────────

        /// <summary>以 ConsoleLogEntry 資料初始化此 Item</summary>
        public void Setup(ConsoleLogEntry entry)
        {
            _entry = entry;

            if (timestampText != null)
                timestampText.text = entry.Timestamp;

            if (messageText != null)
            {
                // 只顯示第一行，避免撐開列表
                string firstLine = entry.Message;
                int newline = firstLine.IndexOf('\n');
                if (newline >= 0) firstLine = firstLine[..newline];
                
                // 限制最大字數，避免 UGUI Text 頂點數超過 65000 (約 16250 字)
                if (firstLine.Length > 15000)
                {
                    firstLine = firstLine[..15000] + "...<Truncated>";
                }
                
                messageText.text = firstLine;
            }

            Color color = entry.Type switch
            {
                ConsoleLogType.Warning   => warningColor,
                ConsoleLogType.Error     => errorColor,
                ConsoleLogType.Exception => errorColor,
                _                        => logColor,
            };

            Sprite sprite = entry.Type switch
            {
                ConsoleLogType.Warning   => warningSprite,
                ConsoleLogType.Error     => errorSprite,
                ConsoleLogType.Exception => errorSprite,
                _                        => logSprite,
            };

            if (typeIcon != null)
            {
                typeIcon.color = Color.white;
                typeIcon.sprite = sprite;
            }

            if (messageText != null)
                messageText.color = color;

            // 初始化為未選取狀態
            SetSelected(false);
        }

        /// <summary>設定此 Item 的選取高亮狀態</summary>
        public void SetSelected(bool selected)
        {
            if (backgroundImage != null)
                backgroundImage.color = selected ? selectedBgColor : normalBgColor;
        }

        // ── Unity 生命週期 ────────────────────────────────────────────────────

        void Awake()
        {
            if (selectButton != null)
                selectButton.onClick.AddListener(OnClicked);
        }

        void OnClicked()
        {
            OnSelected?.Invoke(_entry);
        }
    }
}
