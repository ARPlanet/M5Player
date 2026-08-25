using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Module5.Player
{
    public class LogActionStrategy : IActionStrategy
    {
        public const string ParamMessage = "message";

        public Dictionary<string, object> DefaultParameters => new Dictionary<string, object>
        {
            { ParamMessage, string.Empty }
        };

        private IVariableManager _variableManager;
        public LogActionStrategy(IVariableManager variableManager)
        {
            _variableManager = variableManager;
        }

        public IEnumerator Execute(M5Command command)
        {
            string msg = (string)command.Parameters[ParamMessage];
            Debug.Log($"[Action {command.CommandType}][message={msg}]");
            Debug.Log(msg);
            yield break;
        }
    }
}