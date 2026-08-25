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
    /// 使用 Object Pool 管理 ConsoleLogItem 以避免頻繁 Instantiate/Destroy。
    ///
    /// UGUI 結構（手動建立 Prefab）：
    /// ConsolePanel (此腳本掛載位置)
    /// ├── Toolbar (HorizontalLayoutGroup)
    /// │   ├── ClearButton    (Button)
    /// │   ├── LogToggle      (Toggle) + LogCountText (Text)
    /// │   ├── WarnToggle     (Toggle) + WarnCountText (Text)
    /// │   └── ErrorToggle    (Toggle) + ErrorCountText (Text)
    /// ├── ListScrollView (ScrollRect) — 上半部 Log 列表
    /// │   └── Content (VerticalLayoutGroup + ContentSizeFitter)
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

        [Header("底部詳細面板")]
        [SerializeField] ScrollRect detailScrollRect;
        [SerializeField] Text detailText;

        // 用來顯示長文字的多個 Text 實體
        readonly List<Text> _detailTextInstances = new List<Text>();
        const int MAX_TEXT_LENGTH = 15000;

        [Header("工具列")]
        [SerializeField] Button clearButton;
        [SerializeField] Toggle logToggle;
        [SerializeField] Toggle warningToggle;
        [SerializeField] Toggle errorToggle;
        [SerializeField] Text logCountText;
        [SerializeField] Text warningCountText;
        [SerializeField] Text errorCountText;

        // Object Pool
        readonly Queue<ConsoleLogItem> _pool = new Queue<ConsoleLogItem>();

        // 目前顯示中的 items（依實際 Log 條目順序）
        readonly List<ConsoleLogItem> _activeItems = new List<ConsoleLogItem>();

        bool _showLog     = true;
        bool _showWarning = true;
        bool _showError   = true;

        bool _dirtyScroll;

        ConsoleLogItem _selectedItem;

        // ── Unity 生命週期 ────────────────────────────────────────────────────

        void Awake()
        {
            if (clearButton != null)
                clearButton.onClick.AddListener(OnClearClicked);

            if (logToggle != null)
                logToggle.onValueChanged.AddListener(v => { _showLog = v; RebuildItems(); });

            if (warningToggle != null)
                warningToggle.onValueChanged.AddListener(v => { _showWarning = v; RebuildItems(); });

            if (errorToggle != null)
                errorToggle.onValueChanged.AddListener(v => { _showError = v; RebuildItems(); });

            if (detailText != null)
                _detailTextInstances.Add(detailText);
        }

        void OnEnable()
        {
            if (_consoleManager == null) return;

            _consoleManager.OnLogAdded += HandleLogAdded;
            _consoleManager.OnCleared  += HandleCleared;

            RebuildItems();
        }

        void OnDisable()
        {
            if (_consoleManager == null) return;

            _consoleManager.OnLogAdded -= HandleLogAdded;
            _consoleManager.OnCleared  -= HandleCleared;
        }

        void LateUpdate()
        {
            if (_dirtyScroll)
            {
                _dirtyScroll = false;
                Canvas.ForceUpdateCanvases();
                if (listScrollRect != null)
                    listScrollRect.verticalNormalizedPosition = 0f;
            }
        }

        // ── 公開方法 ──────────────────────────────────────────────────────────



        // ── 私有方法 ──────────────────────────────────────────────────────────

        void HandleLogAdded(ConsoleLogEntry entry)
        {
            if (!ShouldShow(entry.Type)) return;

            ConsoleLogItem item = GetFromPoolWithTracking(entry);
            item.Setup(entry);
            item.transform.SetAsLastSibling();
            _activeItems.Add(item);

            UpdateCountTexts();
            _dirtyScroll = true;
        }

        void HandleCleared()
        {
            ReturnAllToPool();
            ClearDetailPanel();
            UpdateCountTexts();
        }

        /// <summary>過濾條件改變時，重新從 ConsoleManager 建立所有可見 Item</summary>
        void RebuildItems()
        {
            ReturnAllToPool();
            ClearDetailPanel();

            if (_consoleManager == null) return;

            foreach (ConsoleLogEntry entry in _consoleManager.Logs)
            {
                if (!ShouldShow(entry.Type)) continue;

                ConsoleLogItem item = GetFromPoolWithTracking(entry);
                item.Setup(entry);
                item.transform.SetAsLastSibling();
                _activeItems.Add(item);
            }

            UpdateCountTexts();
            _dirtyScroll = true;
        }

        /// <summary>選取某個 LogItem，更新高亮並顯示完整內容於底部面板</summary>
        void OnItemSelected(ConsoleLogEntry entry, ConsoleLogItem clickedItem)
        {
            // 取消前一個選取的高亮
            if (_selectedItem != null)
                _selectedItem.SetSelected(false);

            _selectedItem = clickedItem;
            _selectedItem.SetSelected(true);

            // 更新底部詳細面板
            if (detailText != null)
            {
                string fullText = string.IsNullOrEmpty(entry.StackTrace)
                    ? entry.Message
                    : $"{entry.Message}\n\n{entry.StackTrace}";
                
                int chunksCount = Mathf.CeilToInt((float)fullText.Length / MAX_TEXT_LENGTH);
                if (chunksCount == 0) chunksCount = 1;

                // 確保有足夠的 Text 實體來顯示長文字
                while (_detailTextInstances.Count < chunksCount)
                {
                    Text newText = Instantiate(detailText, detailText.transform.parent);
                    _detailTextInstances.Add(newText);
                }

                // 更新文字內容並控制顯示狀態
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

            // 詳細面板捲動到最頂端
            if (detailScrollRect != null)
            {
                Canvas.ForceUpdateCanvases();
                detailScrollRect.verticalNormalizedPosition = 1f;
            }
        }

        void ClearDetailPanel()
        {
            _selectedItem = null;
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

        // ── Object Pool ───────────────────────────────────────────────────────

        ConsoleLogItem GetFromPool()
        {
            ConsoleLogItem item;

            if (_pool.Count > 0)
            {
                item = _pool.Dequeue();
                item.gameObject.SetActive(true);
            }
            else
            {
                item = Instantiate(logItemPrefab, content);
            }

            // 每次取出時重新綁定選取事件（確保無重複訂閱）
            item.OnSelected -= HandleItemSelected;
            item.OnSelected += HandleItemSelected;

            return item;
        }

        void ReturnAllToPool()
        {
            foreach (ConsoleLogItem item in _activeItems)
            {
                item.OnSelected -= HandleItemSelected;

                if (item == _selectedItem)
                    item.SetSelected(false);

                item.gameObject.SetActive(false);
                _pool.Enqueue(item);
            }
            _activeItems.Clear();
            _entryToItem.Clear();
            _selectedItem = null;
        }

        /// <summary>
        /// 透過 entry 找到對應的 active item 並呼叫 OnItemSelected。
        /// 使用 Dictionary 建立 entry→item 的對應以 O(1) 查找。
        /// </summary>
        readonly Dictionary<ConsoleLogEntry, ConsoleLogItem> _entryToItem
            = new Dictionary<ConsoleLogEntry, ConsoleLogItem>();

        ConsoleLogItem GetFromPoolWithTracking(ConsoleLogEntry entry)
        {
            ConsoleLogItem item = GetFromPool();
            _entryToItem[entry] = item;
            return item;
        }

        void HandleItemSelected(ConsoleLogEntry entry)
        {
            // 找到觸發此事件的 item
            if (_entryToItem.TryGetValue(entry, out ConsoleLogItem clickedItem))
            {
                OnItemSelected(entry, clickedItem);
            }
        }
    }
}
