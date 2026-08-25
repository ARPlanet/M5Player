using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Module5.Player
{
    public class SetGlobalVariableActionStrategy : IActionStrategy
    {
        public const string ParamKey = "key";
        public const string ParamVal = "val";

        public Dictionary<string, object> DefaultParameters => new Dictionary<string, object>
        {
            { ParamKey, string.Empty },
            { ParamVal, null }
        };

        private IVariableManager _variableManager;
        public SetGlobalVariableActionStrategy(IVariableManager variableManager)
        {
            _variableManager = variableManager;
        }

        public IEnumerator Execute(M5Command command)
        {
            string key = (string)command.Parameters[ParamKey];
            object value = command.Parameters[ParamVal];

            if (!string.IsNullOrEmpty(key))
            {
                Debug.Log($"[Action {command.CommandType}][key={key}, val={value}]");
                _variableManager.SetValue(key, value);
            }
            yield break;
        }
    }
}