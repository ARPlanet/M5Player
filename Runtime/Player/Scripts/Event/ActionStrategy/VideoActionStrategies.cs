using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Module5.Player
{
    [Command("Video", "Play")]
    public class VideoPlayActionStrategy : IActionStrategy
    {
        public const string CommandKey = "video_play";
        string IActionStrategy.CommandKey => CommandKey;
        public const string ParamTarget = "target";

        public IReadOnlyDictionary<string, ParameterDefinition> ParameterDefinitions => new Dictionary<string, ParameterDefinition>
        {
            { ParamTarget, ParameterDefinition.CreateObject<VideoPlayerBase>() }
        };

        public IEnumerator Execute(M5Command command, ActionExecutionContext context)
        {
            var videoPlayerBase = (VideoPlayerBase)command.Parameters[ParamTarget];
            if (videoPlayerBase != null)
            {
                Debug.Log($"{context.LogPrefix}[Action {command.CommandType}][target={videoPlayerBase.gameObject.name}]");
                videoPlayerBase.PlayVideo();
            }
            yield break;
        }
    }

    [Command("Video", "Pause")]
    public class VideoPauseActionStrategy : IActionStrategy
    {
        public const string CommandKey = "video_pause";
        string IActionStrategy.CommandKey => CommandKey;
        public const string ParamTarget = "target";

        public IReadOnlyDictionary<string, ParameterDefinition> ParameterDefinitions => new Dictionary<string, ParameterDefinition>
        {
            { ParamTarget, ParameterDefinition.CreateObject<VideoPlayerBase>() }
        };

        public IEnumerator Execute(M5Command command, ActionExecutionContext context)
        {
            var videoPlayerBase = (VideoPlayerBase)command.Parameters[ParamTarget];
            if (videoPlayerBase != null)
            {
                Debug.Log($"{context.LogPrefix}[Action {command.CommandType}][target={videoPlayerBase.gameObject.name}]");
                videoPlayerBase.PauseVideo();
            }
            yield break;
        }
    }

    [Command("Video", "Stop")]
    public class VideoStopActionStrategy : IActionStrategy
    {
        public const string CommandKey = "video_stop";
        string IActionStrategy.CommandKey => CommandKey;
        public const string ParamTarget = "target";

        public IReadOnlyDictionary<string, ParameterDefinition> ParameterDefinitions => new Dictionary<string, ParameterDefinition>
        {
            { ParamTarget, ParameterDefinition.CreateObject<VideoPlayerBase>() }
        };

        public IEnumerator Execute(M5Command command, ActionExecutionContext context)
        {
            var videoPlayerBase = (VideoPlayerBase)command.Parameters[ParamTarget];
            if (videoPlayerBase != null)
            {
                Debug.Log($"{context.LogPrefix}[Action {command.CommandType}][target={videoPlayerBase.gameObject.name}]");
                videoPlayerBase.StopVideo();
            }
            yield break;
        }
    }
}
