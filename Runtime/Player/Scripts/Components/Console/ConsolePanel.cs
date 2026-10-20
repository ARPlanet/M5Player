using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Module5.DI;

namespace Module5.Player
{
    /// <summary>
    /// Console UGUI 顯示面板。
    /// 訂閱 IConsoleManager 的事件，在 ScrollView 中即時顯示 Log 條目。
    /// 點擊 Log 條目後，在底部詳細面板顯示完整訊息與 StackTrace（仿 Unity Console 風格）。
    ///
    /// 【虛擬列表最佳化（方案 B：純座標定位）】：
    /// 僅根據 Viewport 高度動態生成少數 ConsoleLogItem 實體（約 15~25 個），
    /// 滾動時根據 Content Y 軸座標推算資料 Index 並透過 anchoredPosition 定位，
    /// 徹底解除大量 Log 時頻繁 Instantiate/Destroy 與 LayoutGroup 重繪之效能瓶頸。
    ///
    /// UGUI 結構（手動建立 Prefab）：
    /// ConsolePanel (此腳本掛載位置)
    /// ├── Toolbar (HorizontalLayoutGroup)
    /// │   ├── ClearButton    (Button)
    /// │   ├── CopyButton     (Button，複製當前選取 Log 內容)
    /// │   ├── LogToggle      (Toggle) + LogCountText (Text)
    /// │   ├── WarnToggle     (Toggle) + WarnCountText (Text)
    /// │   └── ErrorToggle    (Toggle) + ErrorCountText (Text)
    /// ├── ListScrollView (ScrollRect) — 上半部 Log 列表
    /// │   └── Content (純 RectTransform，自動禁用 VerticalLayoutGroup/ContentSizeFitter)
    /// ├── Separator (可選的分隔線 Image)
    /// └── DetailScrollView (ScrollRect) — 下半部詳細面板
    ///     └── DetailContent
    ///         └── DetailText (Text，支援多行換行)
    /// </summary>
    public class ConsolePanel : MonoBehaviour
    {
        [Inject] IConsoleManager _consoleManager;

        [Header("Log 列表參照")]
        [SerializeField] RectTransform content;
        [SerializeField] ScrollRect listScrollRect;
        [SerializeField] ConsoleLogItem logItemPrefab;

        [Header("虛擬列表設定")]
        [Tooltip("單筆 LogItem 高度。設為 0 時會自動從 logItemPrefab (LayoutElement 或 RectTransform) 動態取得")]
        [SerializeField] float itemHeight = 0f;
        [Tooltip("上下邊界額外緩衝的 Item 數量")]
        [SerializeField] int bufferItems = 2;

        float _resolvedItemHeight = 0f;

        [Header("底部詳細面板")]
        [SerializeField] ScrollRect detailScrollRect;
        [SerializeField] Text detailText;

        // 用來顯示長文字的多個 Text 實體
        readonly List<Text> _detailTextInstances = new List<Text>();
        const int MAX_TEXT_LENGTH = 15000;

        [Header("工具列")]
        [SerializeField] Button clearButton;
        [SerializeField] Button copyButton;
        [SerializeField] Toggle logToggle;
        [SerializeField] Toggle warningToggle;
        [SerializeField] Toggle errorToggle;
        [SerializeField] Text logCountText;
        [SerializeField] Text warningCountText;
        [SerializeField] Text errorCountText;

        // 虛擬化資料集合與實體池
        readonly List<ConsoleLogEntry> _filteredLogs = new List<ConsoleLogEntry>();
        readonly List<ConsoleLogItem> _activeItems = new List<ConsoleLogItem>();

        bool _showLog     = true;
        bool _showWarning = true;
        bool _showError   = true;

        bool _dirtyScroll;
        float _lastViewportHeight = -1f;
        int _lastStartIndex = -1;
        int _lastTotalCount = -1;

        ConsoleLogEntry _selectedEntry;
        string _selectedLogText;

        // ── Unity 生命週期 ────────────────────────────────────────────────────

        void Awake()
        {
            _resolvedItemHeight = ResolveItemHeight();

            if (clearButton != null)
                clearButton.onClick.AddListener(OnClearClicked);

            if (copyButton != null)
                copyButton.onClick.AddListener(OnCopyClicked);

            if (logToggle != null)
                logToggle.onValueChanged.AddListener(v => { _showLog = v; RebuildItems(); });

            if (warningToggle != null)
                warningToggle.onValueChanged.AddListener(v => { _showWarning = v; RebuildItems(); });

            if (errorToggle != null)
                errorToggle.onValueChanged.AddListener(v => { _showError = v; RebuildItems(); });

            if (detailText != null)
                _detailTextInstances.Add(detailText);

            // 初始化 Content 設定：移除/停用 LayoutGroup 與 SizeFitter，改為純座標驅動
            if (content != null)
            {
                if (content.TryGetComponent<ContentSizeFitter>(out var csf))
                    csf.enabled = false;
                if (content.TryGetComponent<VerticalLayoutGroup>(out var vlg))
                    vlg.enabled = false;

                content.anchorMin = new Vector2(0, 1);
                content.anchorMax = new Vector2(1, 1);
                content.pivot     = new Vector2(0, 1);
                content.anchoredPosition = Vector2.zero;
            }

            if (listScrollRect != null)
            {
                listScrollRect.onValueChanged.AddListener(_ => UpdateVisibleRange(false));
            }
        }

        void OnEnable()
        {
            AdjustItemPool();

            if (_consoleManager != null)
            {
                _consoleManager.OnLogAdded += HandleLogAdded;
                _consoleManager.OnCleared  += HandleCleared;
            }

            RebuildItems();
        }

        void OnDisable()
        {
            if (_consoleManager != null)
            {
                _consoleManager.OnLogAdded -= HandleLogAdded;
                _consoleManager.OnCleared  -= HandleCleared;
            }
        }

        void OnDestroy()
        {
            for (int i = 0; i < _activeItems.Count; i++)
            {
                if (_activeItems[i] != null)
                    _activeItems[i].OnSelected -= HandleItemSelected;
            }
            _activeItems.Clear();
        }

        void LateUpdate()
        {
            // 視窗尺寸動態改變檢測
            float vpHeight = GetViewportHeight();
            if (Mathf.Abs(vpHeight - _lastViewportHeight) > 1f)
            {
                _lastViewportHeight = vpHeight;
                AdjustItemPool();
                UpdateContentSize();
                UpdateVisibleRange(true);
            }

            if (_dirtyScroll)
            {
                _dirtyScroll = false;
                ScrollToBottom();
            }
        }

        void OnRectTransformDimensionsChange()
        {
            float vpHeight = GetViewportHeight();
            if (Mathf.Abs(vpHeight - _lastViewportHeight) > 1f)
            {
                _lastViewportHeight = vpHeight;
                AdjustItemPool();
                UpdateContentSize();
                UpdateVisibleRange(true);
            }
        }

        // ── 公開方法 ──────────────────────────────────────────────────────────

        /// <summary>複製目前選取的 Log 內容至系統剪貼簿</summary>
        public void CopySelectedLog()
        {
            OnCopyClicked();
        }

        // ── 虛擬列表核心邏輯 ──────────────────────────────────────────────────

        /// <summary>取得 Item 高度（若未手動指定，則自動從 logItemPrefab 解析）</summary>
        float GetItemHeight()
        {
            if (_resolvedItemHeight > 0f) return _resolvedItemHeight;
            _resolvedItemHeight = ResolveItemHeight();
            return _resolvedItemHeight;
        }

        float ResolveItemHeight()
        {
            if (itemHeight > 0f)
                return itemHeight;

            if (logItemPrefab != null)
            {
                // 1. 優先從 Prefab 上的 LayoutElement 取得 preferredHeight 或 minHeight
                if (logItemPrefab.TryGetComponent<LayoutElement>(out var le))
                {
                    if (le.preferredHeight > 0f) return le.preferredHeight;
                    if (le.minHeight > 0f) return le.minHeight;
                }

                // 2. 次之從 Prefab 上的 RectTransform 取得 sizeDelta.y 或 rect.height
                if (logItemPrefab.transform is RectTransform rt)
                {
                    if (rt.sizeDelta.y > 0f) return rt.sizeDelta.y;
                    if (rt.rect.height > 0f) return rt.rect.height;
                }
            }

            return 40f; // 備用預設值
        }

        float GetViewportHeight()
        {
            if (listScrollRect != null)
            {
                if (listScrollRect.viewport != null)
                    return listScrollRect.viewport.rect.height;
                if (listScrollRect.transform is RectTransform rt)
                    return rt.rect.height;
            }
            return (content != null ? (content.parent as RectTransform)?.rect.height : 0f) ?? 0f;
        }

        /// <summary>依據當前視窗高度，動態調整 ItemPool 實體數量</summary>
        void AdjustItemPool()
        {
            if (content == null || logItemPrefab == null) return;

            float vpHeight = GetViewportHeight();
            if (vpHeight <= 0) vpHeight = 800f; // 初始備用高度

            float h = GetItemHeight();
            int visibleCount = Mathf.CeilToInt(vpHeight / h);
            int targetPoolSize = visibleCount + (bufferItems * 2) + 2;

            while (_activeItems.Count < targetPoolSize)
            {
                ConsoleLogItem item = Instantiate(logItemPrefab, content);
                RectTransform rt = item.RectTransform;
                rt.anchorMin = new Vector2(0, 1);
                rt.anchorMax = new Vector2(1, 1);
                rt.pivot     = new Vector2(0, 1);
                rt.sizeDelta = new Vector2(0, h);

                item.OnSelected += HandleItemSelected;
                item.gameObject.SetActive(false);
                _activeItems.Add(item);
            }
        }

        /// <summary>更新 Content 總高度（純資料總長度，不生成實際物件）</summary>
        void UpdateContentSize()
        {
            if (content == null) return;
            float totalHeight = _filteredLogs.Count * GetItemHeight();
            content.sizeDelta = new Vector2(content.sizeDelta.x, totalHeight);
        }

        /// <summary>依據 Content 捲動位置更新目前可見之少數 Item 實體的位置與內容</summary>
        void UpdateVisibleRange(bool force = false)
        {
            if (content == null) return;

            int totalCount = _filteredLogs.Count;
            if (totalCount == 0)
            {
                for (int i = 0; i < _activeItems.Count; i++)
                    _activeItems[i].gameObject.SetActive(false);
                _lastStartIndex = -1;
                _lastTotalCount = 0;
                return;
            }

            float h = GetItemHeight();
            float scrollY = Mathf.Max(0, content.anchoredPosition.y);
            int firstVisibleIndex = Mathf.FloorToInt(scrollY / h);
            int startIndex = Mathf.Max(0, firstVisibleIndex - bufferItems);

            int maxStartIndex = Mathf.Max(0, totalCount - _activeItems.Count);
            if (startIndex > maxStartIndex)
                startIndex = maxStartIndex;

            if (!force && startIndex == _lastStartIndex && totalCount == _lastTotalCount)
                return;

            _lastStartIndex = startIndex;
            _lastTotalCount = totalCount;

            for (int i = 0; i < _activeItems.Count; i++)
            {
                int dataIndex = startIndex + i;
                ConsoleLogItem item = _activeItems[i];

                if (dataIndex < totalCount)
                {
                    item.gameObject.SetActive(true);
                    item.RectTransform.anchoredPosition = new Vector2(0, -dataIndex * h);
                    ConsoleLogEntry entry = _filteredLogs[dataIndex];
                    item.Setup(entry);
                    item.SetSelected(entry == _selectedEntry);
                }
                else
                {
                    item.gameObject.SetActive(false);
                }
            }
        }

        /// <summary>將視窗捲動至最底部</summary>
        void ScrollToBottom()
        {
            if (content == null) return;

            float vpHeight = GetViewportHeight();
            float maxScrollY = Mathf.Max(0, content.sizeDelta.y - vpHeight);
            content.anchoredPosition = new Vector2(content.anchoredPosition.x, maxScrollY);

            if (listScrollRect != null)
                listScrollRect.velocity = Vector2.zero;

            UpdateVisibleRange(true);
        }

        // ── 事件處理 ──────────────────────────────────────────────────────────

        void HandleLogAdded(ConsoleLogEntry entry)
        {
            UpdateCountTexts();

            if (!ShouldShow(entry.Type)) return;

            _filteredLogs.Add(entry);
            UpdateContentSize();

            _dirtyScroll = true;
        }

        void HandleCleared()
        {
            _filteredLogs.Clear();
            ClearDetailPanel();
            UpdateContentSize();
            UpdateCountTexts();
            UpdateVisibleRange(true);
        }

        /// <summary>過濾條件改變時，重新從 ConsoleManager 建立資料集合</summary>
        void RebuildItems()
        {
            _filteredLogs.Clear();
            ClearDetailPanel();

            if (_consoleManager != null)
            {
                var logs = _consoleManager.Logs;
                for (int i = 0; i < logs.Count; i++)
                {
                    if (ShouldShow(logs[i].Type))
                        _filteredLogs.Add(logs[i]);
                }
            }

            UpdateContentSize();
            UpdateCountTexts();
            ScrollToBottom();
        }

        /// <summary>選取某個 LogItem，更新所有可見項目的高亮狀態並顯示完整內容於底部面板</summary>
        void HandleItemSelected(ConsoleLogEntry entry)
        {
            _selectedEntry = entry;

            string fullText = string.IsNullOrEmpty(entry.StackTrace)
                ? entry.Message
                : $"{entry.Message}\n\n{entry.StackTrace}";

            _selectedLogText = fullText;

            // 更新所有當前可見項目的高亮狀態
            for (int i = 0; i < _activeItems.Count; i++)
            {
                if (_activeItems[i].gameObject.activeSelf)
                    _activeItems[i].SetSelected(_activeItems[i].Entry == _selectedEntry);
            }

            // 更新底部詳細面板
            UpdateDetailText(fullText);

            // 詳細面板捲動到最頂端
            if (detailScrollRect != null)
            {
                Canvas.ForceUpdateCanvases();
                detailScrollRect.verticalNormalizedPosition = 1f;
            }
        }

        void UpdateDetailText(string fullText)
        {
            if (detailText == null) return;

            int chunksCount = Mathf.CeilToInt((float)fullText.Length / MAX_TEXT_LENGTH);
            if (chunksCount == 0) chunksCount = 1;

            while (_detailTextInstances.Count < chunksCount)
            {
                Text newText = Instantiate(detailText, detailText.transform.parent);
                _detailTextInstances.Add(newText);
            }

            for (int i = 0; i < _detailTextInstances.Count; i++)
            {
                if (i < chunksCount)
                {
                    _detailTextInstances[i].gameObject.SetActive(true);
                    int start = i * MAX_TEXT_LENGTH;
                    int length = Mathf.Min(MAX_TEXT_LENGTH, fullText.Length - start);
                    _detailTextInstances[i].text = fullText.Substring(start, length);
                }
                else
                {
                    _detailTextInstances[i].gameObject.SetActive(false);
                }
            }
        }

        void ClearDetailPanel()
        {
            _selectedEntry = null;
            _selectedLogText = null;

            for (int i = 0; i < _activeItems.Count; i++)
            {
                if (_activeItems[i].gameObject.activeSelf)
                    _activeItems[i].SetSelected(false);
            }

            if (_detailTextInstances != null && _detailTextInstances.Count > 0)
            {
                for (int i = 0; i < _detailTextInstances.Count; i++)
                {
                    if (i == 0)
                    {
                        _detailTextInstances[i].text = string.Empty;
                        _detailTextInstances[i].gameObject.SetActive(true);
                    }
                    else
                    {
                        _detailTextInstances[i].gameObject.SetActive(false);
                    }
                }
            }
            else if (detailText != null)
            {
                detailText.text = string.Empty;
            }
        }

        void OnCopyClicked()
        {
            if (!string.IsNullOrEmpty(_selectedLogText))
            {
                GUIUtility.systemCopyBuffer = _selectedLogText;
            }
        }

        bool ShouldShow(ConsoleLogType type)
        {
            return type switch
            {
                ConsoleLogType.Log       => _showLog,
                ConsoleLogType.Warning   => _showWarning,
                ConsoleLogType.Error     => _showError,
                ConsoleLogType.Exception => _showError,
                _                        => true,
            };
        }

        void UpdateCountTexts()
        {
            if (_consoleManager == null) return;

            if (logCountText != null)     logCountText.text     = _consoleManager.LogCount.ToString();
            if (warningCountText != null) warningCountText.text = _consoleManager.WarningCount.ToString();
            if (errorCountText != null)   errorCountText.text   = _consoleManager.ErrorCount.ToString();
        }

        void OnClearClicked()
        {
            _consoleManager?.Clear();
        }
    }
}
