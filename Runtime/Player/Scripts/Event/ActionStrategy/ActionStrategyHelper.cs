using System;
using System.Collections.Generic;
using UnityEngine;

namespace Module5.Player
{
    public static class ActionStrategyHelper
    {
        public static bool TryGetComponent<T>(M5Command command, string paramName, out T component) where T : class
        {
            component = null;
            if (command?.Parameters != null && command.Parameters.TryGetValue(paramName, out object targetObj) && targetObj != null)
            {
                if (targetObj is T directVal)
                {
                    component = directVal;
                    return true;
                }
                if (targetObj is GameObject go)
                {
                    if (typeof(T) == typeof(GameObject))
                    {
                        component = go as T;
                        return true;
                    }
                    component = go.GetComponent<T>();
                    return component != null;
                }
                if (targetObj is Component comp)
                {
                    if (typeof(T) == typeof(GameObject))
                    {
                        component = comp.gameObject as T;
                        return true;
                    }
                    component = comp.GetComponent<T>();
                    return component != null;
                }
            }
            return false;
        }

        public static bool TryGetTransform(M5Command command, out Transform transform)
        {
            return TryGetComponent(command, "target", out transform);
        }

        public static bool TryGetGameObject(M5Command command, string paramName, out GameObject go)
        {
            return TryGetComponent(command, paramName, out go);
        }

        public static float GetFloat(M5Command command, string key, float defaultValue)
        {
            if (command?.Parameters != null && command.Parameters.TryGetValue(key, out object obj))
            {
                if (obj is double d) return (float)d;
                if (obj is float f) return f;
                if (obj is string s && float.TryParse(s, out float pf)) return pf;
                if (obj is int i) return (float)i;
            }
            return defaultValue;
        }
    }
}
