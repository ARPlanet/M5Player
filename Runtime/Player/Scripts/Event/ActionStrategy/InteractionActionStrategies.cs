using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Module5.Player
{
    [Command("Interaction", "Open")]
    public class InteractionOpenActionStrategy : IActionStrategy
    {
        public const string CommandKey = "interaction_open";
        string IActionStrategy.CommandKey => CommandKey;
        public const string ParamUri = "uri";

        public IReadOnlyDictionary<string, ParameterDefinition> ParameterDefinitions => new Dictionary<string, ParameterDefinition>
        {
            { ParamUri, ParameterDefinition.Create<string>(string.Empty) }
        };

        private readonly IProjectManager _projectManager;
        private readonly GoogleDriveDownloader _downloader;

        public InteractionOpenActionStrategy(IProjectManager projectManager)
        {
            _projectManager = projectManager;
            _downloader = new GoogleDriveDownloader();
        }

        public IEnumerator Execute(M5Command command, ActionExecutionContext context)
        {
            string uri = (string)command.Parameters[ParamUri];
            Debug.Log($"{context.LogPrefix}[Action {command.CommandType}][uri={uri}]");
            string driveId = GoogleDriveDownloader.TryExtractFileId(uri);
            if (string.IsNullOrEmpty(driveId))
            {
                Debug.LogWarning($"{context.LogPrefix}[InteractionOpen] 無法從連結中提取檔案 ID。");
                yield break;
            }

            var downloadTask = _downloader.DownloadAndExtract(driveId);
            yield return new WaitUntil(() => downloadTask.IsCompleted);

            if (downloadTask.IsFaulted || string.IsNullOrEmpty(downloadTask.Result))
            {
                Debug.LogError($"{context.LogPrefix}[InteractionOpen] 下載或解壓縮失敗。");
                yield break;
            }

            string projectPath = downloadTask.Result;
            var loadTask = _projectManager.Load(projectPath);
            yield return new WaitUntil(() => loadTask.IsCompleted);
        }
    }

    [Command("Interaction", "Close")]
    public class InteractionCloseActionStrategy : IActionStrategy
    {
        public const string CommandKey = "interaction_close";
        string IActionStrategy.CommandKey => CommandKey;
        public IReadOnlyDictionary<string, ParameterDefinition> ParameterDefinitions => new Dictionary<string, ParameterDefinition>();

        private readonly IProjectManager _projectManager;

        public InteractionCloseActionStrategy(IProjectManager projectManager)
        {
            _projectManager = projectManager;
        }

        public IEnumerator Execute(M5Command command, ActionExecutionContext context)
        {
            Debug.Log($"{context.LogPrefix}[Action {command.CommandType}]");
            _projectManager.Unload();
            yield break;
        }
    }
}
