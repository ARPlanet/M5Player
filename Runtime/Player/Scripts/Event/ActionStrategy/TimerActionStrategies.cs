using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Module5.Player
{
    [Command("Timer", "Start")]
    public class TimerStartActionStrategy : IActionStrategy
    {
        public const string CommandKey = "timer_start";
        string IActionStrategy.CommandKey => CommandKey;
        public const string ParamTarget = "target";
        public const string ParamIsForward = "isForward";

        public IReadOnlyDictionary<string, ParameterDefinition> ParameterDefinitions => new Dictionary<string, ParameterDefinition>
        {
            { ParamTarget, ParameterDefinition.Create<string>(string.Empty) },
            { ParamIsForward, ParameterDefinition.Create<bool>(true) }
        };

        private readonly ITimerManager _timerManager;

        public TimerStartActionStrategy(ITimerManager timerManager)
        {
            _timerManager = timerManager;
        }

        public IEnumerator Execute(M5Command command, ActionExecutionContext context)
        {
            string target = (string)command.Parameters[ParamTarget];
            bool isForward = (bool)command.Parameters[ParamIsForward];

            Debug.Log($"{context.LogPrefix}[Action {command.CommandType}][target={target}, isForward={isForward}]");
            _timerManager.StartTimer(target, isForward);
            yield break;
        }
    }

    [Command("Timer", "Stop")]
    public class TimerStopActionStrategy : IActionStrategy
    {
        public const string CommandKey = "timer_stop";
        string IActionStrategy.CommandKey => CommandKey;
        public const string ParamTarget = "target";

        public IReadOnlyDictionary<string, ParameterDefinition> ParameterDefinitions => new Dictionary<string, ParameterDefinition>
        {
            { ParamTarget, ParameterDefinition.Create<string>(string.Empty) }
        };

        private readonly ITimerManager _timerManager;

        public TimerStopActionStrategy(ITimerManager timerManager)
        {
            _timerManager = timerManager;
        }

        public IEnumerator Execute(M5Command command, ActionExecutionContext context)
        {
            string target = (string)command.Parameters[ParamTarget];
            Debug.Log($"{context.LogPrefix}[Action {command.CommandType}][target={target}]");
            _timerManager.StopTimer(target);
            yield break;
        }
    }

    [Command("Timer", "Add")]
    public class TimerAddActionStrategy : IActionStrategy
    {
        public const string CommandKey = "timer_add";
        string IActionStrategy.CommandKey => CommandKey;
        public const string ParamTarget = "target";
        public const string ParamTime = "time";

        public IReadOnlyDictionary<string, ParameterDefinition> ParameterDefinitions => new Dictionary<string, ParameterDefinition>
        {
            { ParamTarget, ParameterDefinition.Create<string>(string.Empty) },
            { ParamTime, ParameterDefinition.Create<DateTime>(DateTime.MinValue) }
        };

        private readonly ITimerManager _timerManager;

        public TimerAddActionStrategy(ITimerManager timerManager)
        {
            _timerManager = timerManager;
        }

        public IEnumerator Execute(M5Command command, ActionExecutionContext context)
        {
            string target = (string)command.Parameters[ParamTarget];
            DateTime time = (DateTime)command.Parameters[ParamTime];
            Debug.Log($"{context.LogPrefix}[Action {command.CommandType}][target={target}, time={time}]");
            _timerManager.AddTimer(target, time);
            yield break;
        }
    }

    [Command("Timer", "Sub")]
    public class TimerSubActionStrategy : IActionStrategy
    {
        public const string CommandKey = "timer_sub";
        string IActionStrategy.CommandKey => CommandKey;
        public const string ParamTarget = "target";
        public const string ParamTime = "time";

        public IReadOnlyDictionary<string, ParameterDefinition> ParameterDefinitions => new Dictionary<string, ParameterDefinition>
        {
            { ParamTarget, ParameterDefinition.Create<string>(string.Empty) },
            { ParamTime, ParameterDefinition.Create<DateTime>(DateTime.MinValue) }
        };

        private readonly ITimerManager _timerManager;

        public TimerSubActionStrategy(ITimerManager timerManager)
        {
            _timerManager = timerManager;
        }

        public IEnumerator Execute(M5Command command, ActionExecutionContext context)
        {
            string target = (string)command.Parameters[ParamTarget];
            DateTime time = (DateTime)command.Parameters[ParamTime];
            Debug.Log($"{context.LogPrefix}[Action {command.CommandType}][target={target}, time={time}]");
            _timerManager.SubTimer(target, time);
            yield break;
        }
    }
}
