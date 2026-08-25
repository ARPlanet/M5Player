using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Module5.Player
{
    public class InteractionOpenActionStrategy : IActionStrategy
    {
        public const string ParamUri = "uri";

        public Dictionary<string, object> DefaultParameters => new Dictionary<string, object>
        {
            { ParamUri, string.Empty }
        };

        private readonly IProjectManager _projectManager;
        private readonly GoogleDriveDownloader _downloader;

        public InteractionOpenActionStrategy(IProjectManager projectManager)
        {
            _projectManager = projectManager;
            _downloader = new GoogleDriveDownloader();
        }

        public IEnumerator Execute(M5Command command)
        {
            string uri = (string)command.Parameters[ParamUri];
            Debug.Log($"[Action {command.CommandType}][uri={uri}]");
            string driveId = GoogleDriveDownloader.TryExtractFileId(uri);
            if (string.IsNullOrEmpty(driveId))
            {
                Debug.LogWarning("[InteractionOpen] 無法從連結中提取檔案 ID。");
                yield break;
            }

            var downloadTask = _downloader.DownloadAndExtract(driveId);
            yield return new WaitUntil(() => downloadTask.IsCompleted);

            if (downloadTask.IsFaulted || string.IsNullOrEmpty(downloadTask.Result))
            {
                Debug.LogError("[InteractionOpen] 下載或解壓縮失敗。");
                yield break;
            }

            string projectPath = downloadTask.Result;
            var loadTask = _projectManager.Load(projectPath);
            yield return new WaitUntil(() => loadTask.IsCompleted);
        }
    }

    public class InteractionCloseActionStrategy : IActionStrategy
    {
        public Dictionary<string, object> DefaultParameters => new Dictionary<string, object>();

        private readonly IProjectManager _projectManager;

        public InteractionCloseActionStrategy(IProjectManager projectManager)
        {
            _projectManager = projectManager;
        }

        public IEnumerator Execute(M5Command command)
        {
            Debug.Log($"[Action {command.CommandType}]");
            _projectManager.Unload();
            yield break;
        }
    }
}
