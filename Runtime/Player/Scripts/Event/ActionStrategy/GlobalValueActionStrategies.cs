using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Module5.Player
{
    [Command("Global Value", "Set")]
    public class GlobalValueSetActionStrategy : IActionStrategy
    {
        public const string CommandKey = "global_value_set";
        string IActionStrategy.CommandKey => CommandKey;
        public const string ParamKey = "key";
        public const string ParamValue = "value";

        public IReadOnlyDictionary<string, ParameterDefinition> ParameterDefinitions => new Dictionary<string, ParameterDefinition>
        {
            { ParamKey, ParameterDefinition.CreateVariableKey() },
            { ParamValue, ParameterDefinition.Create<object>(null) }
        };

        private readonly IVariableManager _variableManager;

        public GlobalValueSetActionStrategy(IVariableManager variableManager)
        {
            _variableManager = variableManager;
        }

        public IEnumerator Execute(M5Command command, ActionExecutionContext context)
        {
            string key = command.Parameters != null && command.Parameters.TryGetValue(ParamKey, out object k) ? k as string : null;
            if (string.IsNullOrEmpty(key)) yield break;

            object newValue = command.Parameters != null && command.Parameters.TryGetValue(ParamValue, out object v) ? v : null;

            string logPrefix = context?.LogPrefix ?? string.Empty;
            Debug.Log($"{logPrefix}[Action {command.CommandType}][key={key}, value={newValue}]");
            if (_variableManager != null && _variableManager.TryGetValue(key, out object oldValue))
            {
                if (oldValue == null || newValue == null || oldValue.GetType() == newValue.GetType())
                {
                    _variableManager.SetValue(key, newValue);
                }
                else if (TryConvertValue(newValue, oldValue.GetType(), out object convertedValue))
                {
                    _variableManager.SetValue(key, convertedValue);
                }
                else
                {
                    Debug.LogWarning($"{logPrefix}[GlobalValueSet] Type mismatch for variable '{key}'. Expected {oldValue.GetType().Name}, got {newValue.GetType().Name}.");
                }
            }
            else if (_variableManager != null)
            {
                _variableManager.SetValue(key, newValue);
            }
            yield break;
        }

        private static bool TryConvertValue(object value, Type targetType, out object result)
        {
            if (value == null)
            {
                result = null;
                return !targetType.IsValueType || Nullable.GetUnderlyingType(targetType) != null;
            }

            if (value is Newtonsoft.Json.Linq.JValue jValue)
            {
                value = jValue.Value;
                if (value == null)
                {
                    result = null;
                    return !targetType.IsValueType || Nullable.GetUnderlyingType(targetType) != null;
                }
            }

            if (targetType.IsAssignableFrom(value.GetType()))
            {
                result = value;
                return true;
            }

            if (targetType == typeof(Guid))
            {
                if (value is string strGuid && Guid.TryParse(strGuid, out Guid parsedGuid))
                {
                    result = parsedGuid;
                    return true;
                }
            }

            if (targetType == typeof(string) && value is Guid guidVal)
            {
                result = guidVal.ToString();
                return true;
            }

            try
            {
                result = Convert.ChangeType(value, targetType, System.Globalization.CultureInfo.InvariantCulture);
                return true;
            }
            catch
            {
                result = null;
                return false;
            }
        }
    }

    [Command("Global Value", "Numeric Add")]
    public class GlobalValueNumericAddActionStrategy : IActionStrategy
    {
        public const string CommandKey = "global_value_numeric_add";
        string IActionStrategy.CommandKey => CommandKey;
        public const string ParamKey = "key";
        public const string ParamValue = "value";

        public IReadOnlyDictionary<string, ParameterDefinition> ParameterDefinitions => new Dictionary<string, ParameterDefinition>
        {
            { ParamKey, ParameterDefinition.CreateVariableKey() },
            { ParamValue, ParameterDefinition.Create<float>(0f) }
        };

        private readonly IVariableManager _variableManager;

        public GlobalValueNumericAddActionStrategy(IVariableManager variableManager)
        {
            _variableManager = variableManager;
        }

        public IEnumerator Execute(M5Command command, ActionExecutionContext context)
        {
            string key = command.Parameters.TryGetValue(ParamKey, out object k) ? k as string : string.Empty;
            if (string.IsNullOrEmpty(key)) yield break;

            object deltaObj = command.Parameters.TryGetValue(ParamValue, out object v) ? v : 0f;
            Debug.Log($"{context.LogPrefix}[Action {command.CommandType}][key={key}, value={deltaObj}]");

            if (_variableManager != null && _variableManager.TryGetValue(key, out object oldValue) && oldValue != null)
            {
                if (GlobalValueNumericHelper.IsNumeric(oldValue))
                {
                    if (GlobalValueNumericHelper.TryComputeNumeric(oldValue, deltaObj, isAdd: true, out object result))
                    {
                        _variableManager.SetValue(key, result);
                    }
                }
                else
                {
                    Debug.LogWarning($"{context.LogPrefix}[GlobalValueNumericAdd] Variable '{key}' is not a numeric type (Current type: {oldValue.GetType().Name}).");
                }
            }
            else if (_variableManager != null)
            {
                if (GlobalValueNumericHelper.TryComputeNumeric(null, deltaObj, isAdd: true, out object result))
                {
                    _variableManager.SetValue(key, result);
                }
            }

            yield break;
        }
    }

    [Command("Global Value", "Numeric Subtract")]
    public class GlobalValueNumericSubtractActionStrategy : IActionStrategy
    {
        public const string CommandKey = "global_value_numeric_subtract";
        string IActionStrategy.CommandKey => CommandKey;
        public const string ParamKey = "key";
        public const string ParamValue = "value";

        public IReadOnlyDictionary<string, ParameterDefinition> ParameterDefinitions => new Dictionary<string, ParameterDefinition>
        {
            { ParamKey, ParameterDefinition.CreateVariableKey() },
            { ParamValue, ParameterDefinition.Create<float>(0f) }
        };

        private readonly IVariableManager _variableManager;

        public GlobalValueNumericSubtractActionStrategy(IVariableManager variableManager)
        {
            _variableManager = variableManager;
        }

        public IEnumerator Execute(M5Command command, ActionExecutionContext context)
        {
            string key = command.Parameters.TryGetValue(ParamKey, out object k) ? k as string : string.Empty;
            if (string.IsNullOrEmpty(key)) yield break;

            object deltaObj = command.Parameters.TryGetValue(ParamValue, out object v) ? v : 0f;
            Debug.Log($"{context.LogPrefix}[Action {command.CommandType}][key={key}, value={deltaObj}]");

            if (_variableManager != null && _variableManager.TryGetValue(key, out object oldValue) && oldValue != null)
            {
                if (GlobalValueNumericHelper.IsNumeric(oldValue))
                {
                    if (GlobalValueNumericHelper.TryComputeNumeric(oldValue, deltaObj, isAdd: false, out object result))
                    {
                        _variableManager.SetValue(key, result);
                    }
                }
                else
                {
                    Debug.LogWarning($"{context.LogPrefix}[GlobalValueNumericSubtract] Variable '{key}' is not a numeric type (Current type: {oldValue.GetType().Name}).");
                }
            }
            else if (_variableManager != null)
            {
                if (GlobalValueNumericHelper.TryComputeNumeric(null, deltaObj, isAdd: false, out object result))
                {
                    _variableManager.SetValue(key, result);
                }
            }

            yield break;
        }
    }

    internal static class GlobalValueNumericHelper
    {
        public static bool IsNumeric(object value)
        {
            return value is float or int or double or long or sbyte or byte or short or ushort or uint or ulong or decimal;
        }

        public static bool TryComputeNumeric(object currentVal, object deltaObj, bool isAdd, out object result)
        {
            if (deltaObj == null)
            {
                result = currentVal;
                return false;
            }

            try
            {
                double delta = Convert.ToDouble(deltaObj);

                if (currentVal is float f)
                {
                    result = isAdd ? f + (float)delta : f - (float)delta;
                    return true;
                }
                if (currentVal is int i)
                {
                    result = isAdd ? i + (int)delta : i - (int)delta;
                    return true;
                }
                if (currentVal is double d)
                {
                    result = isAdd ? d + delta : d - delta;
                    return true;
                }
                if (currentVal is long l)
                {
                    result = isAdd ? l + (long)delta : l - (long)delta;
                    return true;
                }
                if (currentVal is decimal dec)
                {
                    result = isAdd ? dec + Convert.ToDecimal(deltaObj) : dec - Convert.ToDecimal(deltaObj);
                    return true;
                }
                if (currentVal is short s)
                {
                    result = isAdd ? (short)(s + (short)delta) : (short)(s - (short)delta);
                    return true;
                }
                if (currentVal is byte b)
                {
                    result = isAdd ? (byte)(b + (byte)delta) : (byte)(b - (byte)delta);
                    return true;
                }

                if (currentVal == null)
                {
                    if (deltaObj is int intVal)
                    {
                        result = isAdd ? intVal : -intVal;
                    }
                    else
                    {
                        result = isAdd ? (float)delta : -(float)delta;
                    }
                    return true;
                }
            }
            catch
            {
                result = null;
                return false;
            }

            result = null;
            return false;
        }
    }
}

