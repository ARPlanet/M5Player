using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Module5.Player
{
    [Command("Audio", "Play")]
    public class AudioPlayActionStrategy : IActionStrategy
    {
        public const string CommandKey = "audio_play";
        string IActionStrategy.CommandKey => CommandKey;
        public const string ParamTarget = "target";

        public IReadOnlyDictionary<string, ParameterDefinition> ParameterDefinitions => new Dictionary<string, ParameterDefinition>
        {
            { ParamTarget, ParameterDefinition.CreateObject<AudioPlayer>() }
        };

        public IEnumerator Execute(M5Command command, ActionExecutionContext context)
        {
            var player = (AudioPlayer)command.Parameters[ParamTarget];
            if (player != null)
            {
                Debug.Log($"{context.LogPrefix}[Action {command.CommandType}][target={player.gameObject.name}]");
                player.PlayAudio();
            }
            yield break;
        }
    }

    [Command("Audio", "Pause")]
    public class AudioPauseActionStrategy : IActionStrategy
    {
        public const string CommandKey = "audio_pause";
        string IActionStrategy.CommandKey => CommandKey;
        public const string ParamTarget = "target";

        public IReadOnlyDictionary<string, ParameterDefinition> ParameterDefinitions => new Dictionary<string, ParameterDefinition>
        {
            { ParamTarget, ParameterDefinition.CreateObject<AudioPlayer>() }
        };

        public IEnumerator Execute(M5Command command, ActionExecutionContext context)
        {
            var player = (AudioPlayer)command.Parameters[ParamTarget];
            if (player != null)
            {
                Debug.Log($"{context.LogPrefix}[Action {command.CommandType}][target={player.gameObject.name}]");
                player.PauseAudio();
            }
            yield break;
        }
    }

    [Command("Audio", "Stop")]
    public class AudioStopActionStrategy : IActionStrategy
    {
        public const string CommandKey = "audio_stop";
        string IActionStrategy.CommandKey => CommandKey;
        public const string ParamTarget = "target";

        public IReadOnlyDictionary<string, ParameterDefinition> ParameterDefinitions => new Dictionary<string, ParameterDefinition>
        {
            { ParamTarget, ParameterDefinition.CreateObject<AudioPlayer>() }
        };

        public IEnumerator Execute(M5Command command, ActionExecutionContext context)
        {
            var player = (AudioPlayer)command.Parameters[ParamTarget];
            if (player != null)
            {
                Debug.Log($"{context.LogPrefix}[Action {command.CommandType}][target={player.gameObject.name}]");
                player.StopAudio();
            }
            yield break;
        }
    }
}
