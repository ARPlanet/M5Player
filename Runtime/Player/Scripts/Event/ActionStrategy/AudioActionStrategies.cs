using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Module5.Player
{
    public class AudioPlayActionStrategy : IActionStrategy
    {
        public const string ParamTarget = "target";

        public Dictionary<string, object> DefaultParameters => new Dictionary<string, object>
        {
            { ParamTarget, Guid.Empty.ToString() }
        };

        public IEnumerator Execute(M5Command command)
        {
            var source = (AudioSource)command.Parameters[ParamTarget];
            if (source != null)
            {
                Debug.Log($"[Action {command.CommandType}][target={source.gameObject.name}]");
                source.Play();
            }
            yield break;
        }
    }

    public class AudioPauseActionStrategy : IActionStrategy
    {
        public const string ParamTarget = "target";

        public Dictionary<string, object> DefaultParameters => new Dictionary<string, object>
        {
            { ParamTarget, Guid.Empty.ToString() }
        };

        public IEnumerator Execute(M5Command command)
        {
            var source = (AudioSource)command.Parameters[ParamTarget];
            if (source != null)
            {
                Debug.Log($"[Action {command.CommandType}][target={source.gameObject.name}]");
                source.Pause();
            }
            yield break;
        }
    }

    public class AudioStopActionStrategy : IActionStrategy
    {
        public const string ParamTarget = "target";

        public Dictionary<string, object> DefaultParameters => new Dictionary<string, object>
        {
            { ParamTarget, Guid.Empty.ToString() }
        };

        public IEnumerator Execute(M5Command command)
        {
            var source = (AudioSource)command.Parameters[ParamTarget];
            if (source != null)
            {
                Debug.Log($"[Action {command.CommandType}][target={source.gameObject.name}]");
                source.Stop();
            }
            yield break;
        }
    }
}
