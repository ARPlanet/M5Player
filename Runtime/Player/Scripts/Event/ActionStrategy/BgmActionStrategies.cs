using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Module5.Player
{
    [Command("BGM", "Play")]
    public class BgmPlayActionStrategy : IActionStrategy
    {
        public const string CommandKey = "bgm_play";
        string IActionStrategy.CommandKey => CommandKey;

        public const string ParamClip = "clip";
        public const string ParamLoop = "loop";
        public const string ParamFadeDuration = "fade";

        public IReadOnlyDictionary<string, ParameterDefinition> ParameterDefinitions => new Dictionary<string, ParameterDefinition>
        {
            { ParamClip, ParameterDefinition.CreateObject<AudioClip>() },
            { ParamLoop, ParameterDefinition.Create<bool>(true) },
            { ParamFadeDuration, ParameterDefinition.Create<float>(0f) }
        };

        private readonly IBgmManager _bgmManager;
        private readonly IAssetLoaderManager _assetLoaderManager;

        public BgmPlayActionStrategy(IBgmManager bgmManager, IAssetLoaderManager assetLoaderManager = null)
        {
            _bgmManager = bgmManager;
            _assetLoaderManager = assetLoaderManager;
        }

        public IEnumerator Execute(M5Command command, ActionExecutionContext context)
        {
            if (_bgmManager == null) yield break;

            AudioClip clip = null;
            if (command.Parameters.TryGetValue(ParamClip, out object clipObj))
            {
                if (clipObj is AudioClip c)
                {
                    clip = c;
                }
                else if (clipObj is string clipGuidStr && Guid.TryParse(clipGuidStr, out Guid guid) && _assetLoaderManager != null)
                {
                    var task = _assetLoaderManager.LoadInstance(guid);
                    while (!task.IsCompleted) yield return null;
                    if (task.Result != null && task.Result.Obj is AudioClip loadedClip)
                    {
                        clip = loadedClip;
                    }
                }
            }

            bool loop = true;
            if (command.Parameters.TryGetValue(ParamLoop, out object loopObj) && loopObj is bool l)
            {
                loop = l;
            }

            float fade = 0f;
            if (command.Parameters.TryGetValue(ParamFadeDuration, out object fadeObj) && fadeObj is float f)
            {
                fade = f;
            }

            Debug.Log($"{context.LogPrefix}[Action {command.CommandType}][clip={(clip != null ? clip.name : "(current)")}, loop={loop}, fade={fade}]");
            _bgmManager.Play(clip, loop, fade);
            yield break;
        }
    }

    [Command("BGM", "Pause")]
    public class BgmPauseActionStrategy : IActionStrategy
    {
        public const string CommandKey = "bgm_pause";
        string IActionStrategy.CommandKey => CommandKey;

        public IReadOnlyDictionary<string, ParameterDefinition> ParameterDefinitions => new Dictionary<string, ParameterDefinition>();

        private readonly IBgmManager _bgmManager;

        public BgmPauseActionStrategy(IBgmManager bgmManager)
        {
            _bgmManager = bgmManager;
        }

        public IEnumerator Execute(M5Command command, ActionExecutionContext context)
        {
            if (_bgmManager != null)
            {
                Debug.Log($"{context.LogPrefix}[Action {command.CommandType}]");
                _bgmManager.Pause();
            }
            yield break;
        }
    }

    [Command("BGM", "Stop")]
    public class BgmStopActionStrategy : IActionStrategy
    {
        public const string CommandKey = "bgm_stop";
        string IActionStrategy.CommandKey => CommandKey;

        public const string ParamFadeDuration = "fade";

        public IReadOnlyDictionary<string, ParameterDefinition> ParameterDefinitions => new Dictionary<string, ParameterDefinition>
        {
            { ParamFadeDuration, ParameterDefinition.Create<float>(0f) }
        };

        private readonly IBgmManager _bgmManager;

        public BgmStopActionStrategy(IBgmManager bgmManager)
        {
            _bgmManager = bgmManager;
        }

        public IEnumerator Execute(M5Command command, ActionExecutionContext context)
        {
            if (_bgmManager != null)
            {
                float fade = 0f;
                if (command.Parameters.TryGetValue(ParamFadeDuration, out object fadeObj) && fadeObj is float f)
                {
                    fade = f;
                }
                Debug.Log($"{context.LogPrefix}[Action {command.CommandType}][fade={fade}]");
                _bgmManager.Stop(fade);
            }
            yield break;
        }
    }

    [Command("BGM", "Set Volume")]
    public class BgmSetVolumeActionStrategy : IActionStrategy
    {
        public const string CommandKey = "bgm_set_volume";
        string IActionStrategy.CommandKey => CommandKey;

        public const string ParamVolume = "volume";
        public const string ParamFadeDuration = "fade";

        public IReadOnlyDictionary<string, ParameterDefinition> ParameterDefinitions => new Dictionary<string, ParameterDefinition>
        {
            { ParamVolume, ParameterDefinition.Create<float>(1f) },
            { ParamFadeDuration, ParameterDefinition.Create<float>(0f) }
        };

        private readonly IBgmManager _bgmManager;

        public BgmSetVolumeActionStrategy(IBgmManager bgmManager)
        {
            _bgmManager = bgmManager;
        }

        public IEnumerator Execute(M5Command command, ActionExecutionContext context)
        {
            if (_bgmManager != null)
            {
                float volume = 1f;
                if (command.Parameters.TryGetValue(ParamVolume, out object volObj) && volObj is float v)
                {
                    volume = v;
                }
                float fade = 0f;
                if (command.Parameters.TryGetValue(ParamFadeDuration, out object fadeObj) && fadeObj is float f)
                {
                    fade = f;
                }
                Debug.Log($"{context.LogPrefix}[Action {command.CommandType}][volume={volume}, fade={fade}]");
                _bgmManager.SetVolume(volume, fade);
            }
            yield break;
        }
    }
}
