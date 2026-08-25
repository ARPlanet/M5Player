using System;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Module5.DI;
namespace Module5.Player
{
    /// <summary>
    /// 專案開啟入口 MonoBehaviour。
    /// 根據 useLocalPath 決定使用本機路徑開啟專案，或透過 Google Drive 下載後開啟。
    /// </summary>
    public class ProjectOpener : MonoBehaviour
    {
        public Camera openProjectCamera;
        [Header("專案來源設定")]
        [Tooltip("true = 使用本機路徑開啟, false = 使用 Google Drive 下載")]
        public bool useLocalPath = true;

        [Tooltip("本機專案路徑（useLocalPath = true 時使用）")]
        public string localProjectPath;

        [Header("Google Drive UI")]
        [Tooltip("輸入 Google Drive 下載連結的 InputField")]
        [FormerlySerializedAs("driveIdInputField")]
        public InputField driveUrlInputField;

        [Tooltip("執行下載的按鈕")]
        public Button downloadButton;

        [Tooltip("進度與狀態顯示文字")]
        public Text statusText;

        [Inject] protected IProjectManager _projectManager;

        private GoogleDriveDownloader downloader;

        private void Awake()
        {
            downloader = new GoogleDriveDownloader();
        }

        private async void Start()
        {
            if (useLocalPath)
            {
                // 本機路徑模式：隱藏 UI，直接載入
                SetUIActive(false);
                await LoadProject(localProjectPath);
            }
            else
            {
                // Google Drive 模式：顯示 UI，等待使用者操作
                SetUIActive(true);
            }
        }

        private void OnEnable()
        {
            if (downloadButton != null)
                downloadButton.onClick.AddListener(OnDownloadClicked);

            if (_projectManager != null && _projectManager != null)
            {
                _projectManager.UnloadEvent += OnProjectUnloaded;
            }
        }

        private void OnDisable()
        {
            if (downloadButton != null)
                downloadButton.onClick.RemoveListener(OnDownloadClicked);

            if (_projectManager != null)
            {
                _projectManager.UnloadEvent -= OnProjectUnloaded;
            }
        }

        private void OnProjectUnloaded()
        {
            openProjectCamera.gameObject.SetActive(true);
            SetUIActive(true);
            SetInteractable(true);
        }

        /// <summary>
        /// 下載按鈕點擊處理。
        /// </summary>
        private async void OnDownloadClicked()
        {
            if (driveUrlInputField == null)
            {
                Debug.LogError("[ProjectOpener] InputField 未指定。");
                UpdateStatus("錯誤: InputField 未指定", Color.red);
                return;
            }

            string input = driveUrlInputField.text?.Trim();
            if (string.IsNullOrEmpty(input))
            {
                Debug.LogWarning("[ProjectOpener] 請輸入 Google Drive 下載連結。");
                UpdateStatus("提示: 請輸入連結", Color.yellow);
                return;
            }

            // 提取 ID
            string driveId = GoogleDriveDownloader.TryExtractFileId(input);
            if (string.IsNullOrEmpty(driveId))
            {
                Debug.LogWarning("[ProjectOpener] 無法從連結中提取檔案 ID。");
                UpdateStatus("錯誤: 無效的連結", Color.red);
                return;
            }

            // 停用按鈕，防止重複點擊
            SetInteractable(false);
            Debug.Log($"[ProjectOpener] 開始處理下載. 原輸入: {input}, 提取 ID: {driveId}");
            UpdateStatus("初始化下載...", Color.white);

            try
            {
                string projectPath = await downloader.DownloadAndExtract(driveId, progress =>
                {
                    string progressMsg = $"下載中: {progress:P0}";
                    Debug.Log($"[ProjectOpener] {progressMsg}");
                    UpdateStatus(progressMsg, Color.white);
                });

                if (string.IsNullOrEmpty(projectPath))
                {
                    Debug.LogError("[ProjectOpener] 下載或解壓失敗。");
                    UpdateStatus("下載或解壓失敗", Color.red);
                    SetInteractable(true);
                    return;
                }

                Debug.Log($"[ProjectOpener] 下載完成，專案路徑: {projectPath}");
                UpdateStatus("下載完成，正在載入專案...", Color.green);
                await LoadProject(projectPath);
            }
            catch (Exception e)
            {
                Debug.LogError($"[ProjectOpener] 下載過程發生錯誤: {e.Message}");
                UpdateStatus($"錯誤: {e.Message}", Color.red);
                SetInteractable(true);
            }
        }

        /// <summary>
        /// 更新狀態文字。
        /// </summary>
        private void UpdateStatus(string message, Color color)
        {
            if (statusText != null)
            {
                statusText.text = message;
                statusText.color = color;
            }
        }

        /// <summary>
        /// 呼叫 ProjectManager 載入專案。
        /// </summary>
        private async System.Threading.Tasks.Task LoadProject(string path)
        {
            if (_projectManager == null)
            {
                Debug.LogError("[ProjectOpener] ProjectManager 未就緒。");
                return;
            }

            bool success = await _projectManager.Load(path);
            if (success)
            {
                openProjectCamera.gameObject.SetActive(false);
                SetUIActive(false);
            }
            else
            {
                Debug.LogError($"[ProjectOpener] 載入專案失敗: {path}");
            }
        }

        /// <summary>
        /// 設定 UI 元件顯示/隱藏。
        /// </summary>
        public void SetUIActive(bool active)
        {
            if (driveUrlInputField != null)
                driveUrlInputField.gameObject.SetActive(active);
            if (downloadButton != null)
                downloadButton.gameObject.SetActive(active);
            if (statusText != null)
                statusText.gameObject.SetActive(active);
        }

        /// <summary>
        /// 設定 UI 元件互動狀態。
        /// </summary>
        private void SetInteractable(bool interactable)
        {
            if (downloadButton != null)
                downloadButton.interactable = interactable;
            if (driveUrlInputField != null)
                driveUrlInputField.interactable = interactable;
        }
    }
}
