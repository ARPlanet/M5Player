using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Module5.Player
{
    public class VideoPlayActionStrategy : IActionStrategy
    {
        public const string ParamTarget = "target";

        public Dictionary<string, object> DefaultParameters => new Dictionary<string, object>
        {
            { ParamTarget, Guid.Empty.ToString() }
        };

        public IEnumerator Execute(M5Command command)
        {
            var videoPlayerBase = (VideoPlayerBase)command.Parameters[ParamTarget];
            if (videoPlayerBase != null)
            {
                Debug.Log($"[Action {command.CommandType}][target={videoPlayerBase.gameObject.name}]");
                videoPlayerBase.PlayVideo();
            }
            yield break;
        }
    }

    public class VideoPauseActionStrategy : IActionStrategy
    {
        public const string ParamTarget = "target";

        public Dictionary<string, object> DefaultParameters => new Dictionary<string, object>
        {
            { ParamTarget, Guid.Empty.ToString() }
        };

        public IEnumerator Execute(M5Command command)
        {
            var videoPlayerBase = (VideoPlayerBase)command.Parameters[ParamTarget];
            if (videoPlayerBase != null)
            {
                Debug.Log($"[Action {command.CommandType}][target={videoPlayerBase.gameObject.name}]");
                videoPlayerBase.PauseVideo();
            }
            yield break;
        }
    }

    public class VideoStopActionStrategy : IActionStrategy
    {
        public const string ParamTarget = "target";

        public Dictionary<string, object> DefaultParameters => new Dictionary<string, object>
        {
            { ParamTarget, Guid.Empty.ToString() }
        };

        public IEnumerator Execute(M5Command command)
        {
            var videoPlayerBase = (VideoPlayerBase)command.Parameters[ParamTarget];
            if (videoPlayerBase != null)
            {
                Debug.Log($"[Action {command.CommandType}][target={videoPlayerBase.gameObject.name}]");
                videoPlayerBase.StopVideo();
            }
            yield break;
        }
    }
}
