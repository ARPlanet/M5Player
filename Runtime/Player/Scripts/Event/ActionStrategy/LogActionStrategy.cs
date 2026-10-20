using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Module5.Player
{
    [Command("Debug", "Log")]
    public class LogActionStrategy : IActionStrategy
    {
        public const string CommandKey = "debug_log";
        string IActionStrategy.CommandKey => CommandKey;
        public const string ParamMessage = "message";

        public IReadOnlyDictionary<string, ParameterDefinition> ParameterDefinitions => new Dictionary<string, ParameterDefinition>
        {
            { ParamMessage, ParameterDefinition.Create<string>(string.Empty) }
        };

        private IVariableManager _variableManager;
        public LogActionStrategy(IVariableManager variableManager)
        {
            _variableManager = variableManager;
        }

        public IEnumerator Execute(M5Command command, ActionExecutionContext context)
        {
            string msg = (string)command.Parameters[ParamMessage];
            Debug.Log($"{context.LogPrefix}[Action {command.CommandType}][message={msg}]");
            yield break;
        }
    }
}