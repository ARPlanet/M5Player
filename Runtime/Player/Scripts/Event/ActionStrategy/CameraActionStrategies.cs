using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Module5.Player
{
    public class CameraTakePhotoActionStrategy : IActionStrategy
    {
        public Dictionary<string, object> DefaultParameters => new Dictionary<string, object>();

        public IEnumerator Execute(M5Command command)
        {
            Debug.Log($"[Action {command.CommandType}]");
            yield break;
        }
    }

    public class CameraRecordVideoActionStrategy : IActionStrategy
    {
        public Dictionary<string, object> DefaultParameters => new Dictionary<string, object>();

        public IEnumerator Execute(M5Command command)
        {
            Debug.Log($"[Action {command.CommandType}]");
            yield break;
        }
    }

    public class CameraFlipFrontCameraActionStrategy : IActionStrategy
    {
        public Dictionary<string, object> DefaultParameters => new Dictionary<string, object>();

        public IEnumerator Execute(M5Command command)
        {
            Debug.Log($"[Action {command.CommandType}]");
            yield break;
        }
    }

    public class CameraPreviewSaveActionStrategy : IActionStrategy
    {
        public Dictionary<string, object> DefaultParameters => new Dictionary<string, object>();

        public IEnumerator Execute(M5Command command)
        {
            Debug.Log($"[Action {command.CommandType}]");
            yield break;
        }
    }

    public class CameraPreviewUploadActionStrategy : IActionStrategy
    {
        public Dictionary<string, object> DefaultParameters => new Dictionary<string, object>();

        public IEnumerator Execute(M5Command command)
        {
            Debug.Log($"[Action {command.CommandType}]");
            yield break;
        }
    }

    public class CameraPreviewShareActionStrategy : IActionStrategy
    {
        public Dictionary<string, object> DefaultParameters => new Dictionary<string, object>();

        public IEnumerator Execute(M5Command command)
        {
            Debug.Log($"[Action {command.CommandType}]");
            yield break;
        }
    }
}
