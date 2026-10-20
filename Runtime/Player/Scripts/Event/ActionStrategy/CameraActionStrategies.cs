using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Module5.Player
{
    [Command("Camera", "Take Photo")]
    public class CameraTakePhotoActionStrategy : IActionStrategy
    {
        public const string CommandKey = "camera_take_photo";
        string IActionStrategy.CommandKey => CommandKey;
        public IReadOnlyDictionary<string, ParameterDefinition> ParameterDefinitions => new Dictionary<string, ParameterDefinition>();

        public IEnumerator Execute(M5Command command, ActionExecutionContext context)
        {
            Debug.Log($"{context.LogPrefix}[Action {command.CommandType}]");
            yield break;
        }
    }

    [Command("Camera", "Record Video")]
    public class CameraRecordVideoActionStrategy : IActionStrategy
    {
        public const string CommandKey = "camera_record_video";
        string IActionStrategy.CommandKey => CommandKey;
        public IReadOnlyDictionary<string, ParameterDefinition> ParameterDefinitions => new Dictionary<string, ParameterDefinition>();

        public IEnumerator Execute(M5Command command, ActionExecutionContext context)
        {
            Debug.Log($"{context.LogPrefix}[Action {command.CommandType}]");
            yield break;
        }
    }

    [Command("Camera", "Flip Front Camera")]
    public class CameraFlipFrontCameraActionStrategy : IActionStrategy
    {
        public const string CommandKey = "camera_flip_front_camera";
        string IActionStrategy.CommandKey => CommandKey;
        public IReadOnlyDictionary<string, ParameterDefinition> ParameterDefinitions => new Dictionary<string, ParameterDefinition>();

        public IEnumerator Execute(M5Command command, ActionExecutionContext context)
        {
            Debug.Log($"{context.LogPrefix}[Action {command.CommandType}]");
            yield break;
        }
    }

    [Command("Camera", "Preview Save")]
    public class CameraPreviewSaveActionStrategy : IActionStrategy
    {
        public const string CommandKey = "camera_preview_save";
        string IActionStrategy.CommandKey => CommandKey;
        public IReadOnlyDictionary<string, ParameterDefinition> ParameterDefinitions => new Dictionary<string, ParameterDefinition>();

        public IEnumerator Execute(M5Command command, ActionExecutionContext context)
        {
            Debug.Log($"{context.LogPrefix}[Action {command.CommandType}]");
            yield break;
        }
    }

    [Command("Camera", "Preview Upload")]
    public class CameraPreviewUploadActionStrategy : IActionStrategy
    {
        public const string CommandKey = "camera_preview_upload";
        string IActionStrategy.CommandKey => CommandKey;
        public IReadOnlyDictionary<string, ParameterDefinition> ParameterDefinitions => new Dictionary<string, ParameterDefinition>();

        public IEnumerator Execute(M5Command command, ActionExecutionContext context)
        {
            Debug.Log($"{context.LogPrefix}[Action {command.CommandType}]");
            yield break;
        }
    }

    [Command("Camera", "Preview Share")]
    public class CameraPreviewShareActionStrategy : IActionStrategy
    {
        public const string CommandKey = "camera_preview_share";
        string IActionStrategy.CommandKey => CommandKey;
        public IReadOnlyDictionary<string, ParameterDefinition> ParameterDefinitions => new Dictionary<string, ParameterDefinition>();

        public IEnumerator Execute(M5Command command, ActionExecutionContext context)
        {
            Debug.Log($"{context.LogPrefix}[Action {command.CommandType}]");
            yield break;
        }
    }
}
