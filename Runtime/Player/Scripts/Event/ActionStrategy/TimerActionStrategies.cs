using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Module5.Player
{
    public class TimerStartActionStrategy : IActionStrategy
    {
        public const string ParamTarget = "target";
        public const string ParamIsForward = "isForward";

        public Dictionary<string, object> DefaultParameters => new Dictionary<string, object>
        {
            { ParamTarget, string.Empty },
            { ParamIsForward, true }
        };

        private readonly ITimerManager _timerManager;

        public TimerStartActionStrategy(ITimerManager timerManager)
        {
            _timerManager = timerManager;
        }

        public IEnumerator Execute(M5Command command)
        {
            string target = (string)command.Parameters[ParamTarget];
            bool isForward = (bool)command.Parameters[ParamIsForward];

            Debug.Log($"[Action {command.CommandType}][target={target}, isForward={isForward}]");
            _timerManager.StartTimer(target, isForward);
            yield break;
        }
    }

    public class TimerStopActionStrategy : IActionStrategy
    {
        public const string ParamTarget = "target";

        public Dictionary<string, object> DefaultParameters => new Dictionary<string, object>
        {
            { ParamTarget, string.Empty }
        };

        private readonly ITimerManager _timerManager;

        public TimerStopActionStrategy(ITimerManager timerManager)
        {
            _timerManager = timerManager;
        }

        public IEnumerator Execute(M5Command command)
        {
            string target = (string)command.Parameters[ParamTarget];
            Debug.Log($"[Action {command.CommandType}][target={target}]");
            _timerManager.StopTimer(target);
            yield break;
        }
    }

    public class TimerAddActionStrategy : IActionStrategy
    {
        public const string ParamTarget = "target";
        public const string ParamTime = "time";

        public Dictionary<string, object> DefaultParameters => new Dictionary<string, object>
        {
            { ParamTarget, string.Empty },
            { ParamTime, DateTime.MinValue }
        };

        private readonly ITimerManager _timerManager;

        public TimerAddActionStrategy(ITimerManager timerManager)
        {
            _timerManager = timerManager;
        }

        public IEnumerator Execute(M5Command command)
        {
            string target = (string)command.Parameters[ParamTarget];
            DateTime time = (DateTime)command.Parameters[ParamTime];
            Debug.Log($"[Action {command.CommandType}][target={target}, time={time}]");
            _timerManager.AddTimer(target, time);
            yield break;
        }
    }

    public class TimerSubActionStrategy : IActionStrategy
    {
        public const string ParamTarget = "target";
        public const string ParamTime = "time";

        public Dictionary<string, object> DefaultParameters => new Dictionary<string, object>
        {
            { ParamTarget, string.Empty },
            { ParamTime, DateTime.MinValue }
        };

        private readonly ITimerManager _timerManager;

        public TimerSubActionStrategy(ITimerManager timerManager)
        {
            _timerManager = timerManager;
        }

        public IEnumerator Execute(M5Command command)
        {
            string target = (string)command.Parameters[ParamTarget];
            DateTime time = (DateTime)command.Parameters[ParamTime];
            Debug.Log($"[Action {command.CommandType}][target={target}, time={time}]");
            _timerManager.SubTimer(target, time);
            yield break;
        }
    }
}
