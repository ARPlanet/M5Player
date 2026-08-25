using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Module5.Player
{
    public class GlobalValueSetActionStrategy : IActionStrategy
    {
        public const string ParamKey = "key";
        public const string ParamValue = "value";

        public Dictionary<string, object> DefaultParameters => new Dictionary<string, object>
        {
            { ParamKey, string.Empty },
            { ParamValue, null }
        };

        private readonly IVariableManager _variableManager;

        public GlobalValueSetActionStrategy(IVariableManager variableManager)
        {
            _variableManager = variableManager;
        }

        public IEnumerator Execute(M5Command command)
        {
            string key = (string)command.Parameters[ParamKey];
            object newValue = command.Parameters[ParamValue];

            Debug.Log($"[Action {command.CommandType}][key={key}, value={newValue}]");
            if (_variableManager.TryGetValue(key, out object oldValue))
            {
                if (oldValue == null || newValue == null || oldValue.GetType() == newValue.GetType())
                {
                    _variableManager.SetValue(key, newValue);
                }
                else
                {
                    Debug.LogWarning($"[GlobalValueSet] Type mismatch for variable '{key}'. Expected {oldValue.GetType().Name}, got {newValue.GetType().Name}.");
                }
            }
            else
            {
                _variableManager.SetValue(key, newValue);
            }
            yield break;
        }
    }
}
