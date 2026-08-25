using System;
using System.Collections.Generic;
using UnityEngine;
using Module5.DI;
namespace Module5.Player
{
    public class DefaultGlobalVarTargetResolver : IConditionTargetResolver
    {
        public bool TryResolveValue(M5Condition condition, Dictionary<string, object> eventContext, IVariableManager variableManager, IAssetDataBaseManager assetDataBaseManager, out object sourceValue)
        {
            if (variableManager != null && condition != null && !string.IsNullOrEmpty(condition.TargetId))
            {
                return variableManager.TryGetValue(condition.TargetId, out sourceValue);
            }
            sourceValue = null;
            return false;
        }
    }

    public class DefaultEventContextTargetResolver : IConditionTargetResolver
    {
        public bool TryResolveValue(M5Condition condition, Dictionary<string, object> eventContext, IVariableManager variableManager, IAssetDataBaseManager assetDataBaseManager, out object sourceValue)
        {
            if (eventContext != null && condition != null && !string.IsNullOrEmpty(condition.TargetId))
            {
                string cleanKey = condition.TargetId;
                if (cleanKey.StartsWith("${") && cleanKey.EndsWith("}")) cleanKey = cleanKey.Substring(2, cleanKey.Length - 3);
                return eventContext.TryGetValue(cleanKey, out sourceValue);
            }
            sourceValue = null;
            return false;
        }
    }

    public class DefaultSceneObjectTargetResolver : IConditionTargetResolver
    {
        public bool TryResolveValue(M5Condition condition, Dictionary<string, object> eventContext, IVariableManager variableManager, IAssetDataBaseManager assetDataBaseManager, out object sourceValue)
        {
            sourceValue = null;

            if (condition == null || string.IsNullOrEmpty(condition.TargetId) || string.IsNullOrEmpty(condition.TargetProperty))
            {
                return false;
            }

            if (assetDataBaseManager == null || !Guid.TryParse(condition.TargetId, out var targetGuid) || !assetDataBaseManager.TryGetInstance(targetGuid, out var container) || container.Obj == null)
            {
                return false;
            }

            object rootObj = container.Obj;
            string targetProperty = condition.TargetProperty;

            string targetTypeToken;
            string propertyPath;

            int dotIndex = targetProperty.IndexOf('.');
            if (dotIndex >= 0)
            {
                targetTypeToken = targetProperty.Substring(0, dotIndex);
                propertyPath = targetProperty.Substring(dotIndex + 1);
            }
            else
            {
                targetTypeToken = targetProperty;
                propertyPath = string.Empty;
            }

            object resolvedTarget = null;

            if (rootObj is GameObject go)
            {
                if (targetTypeToken.Equals("GameObject", StringComparison.OrdinalIgnoreCase) || IsComponentTypeOfName(go.GetComponent<Component>(), targetTypeToken))
                {
                    resolvedTarget = go;
                }
                else
                {
                    var components = go.GetComponents<Component>();
                    if (components != null)
                    {
                        foreach (var comp in components)
                        {
                            if (IsComponentTypeOfName(comp, targetTypeToken))
                            {
                                resolvedTarget = comp;
                                break;
                            }
                        }
                    }

                    if (resolvedTarget == null)
                    {
                        resolvedTarget = go;
                        propertyPath = targetProperty;
                    }
                }
            }
            else if (rootObj is Component comp)
            {
                if (targetTypeToken.Equals("GameObject", StringComparison.OrdinalIgnoreCase))
                {
                    resolvedTarget = comp.gameObject;
                }
                else if (IsComponentTypeOfName(comp, targetTypeToken))
                {
                    resolvedTarget = comp;
                }
                else
                {
                    var components = comp.GetComponents<Component>();
                    if (components != null)
                    {
                        foreach (var c in components)
                        {
                            if (IsComponentTypeOfName(c, targetTypeToken))
                            {
                                resolvedTarget = c;
                                break;
                            }
                        }
                    }

                    if (resolvedTarget == null)
                    {
                        resolvedTarget = comp;
                        propertyPath = targetProperty;
                    }
                }
            }
            else
            {
                resolvedTarget = rootObj;
                if (!rootObj.GetType().Name.Equals(targetTypeToken, StringComparison.OrdinalIgnoreCase))
                {
                    propertyPath = targetProperty;
                }
            }

            if (resolvedTarget == null)
            {
                return false;
            }

            if (string.IsNullOrEmpty(propertyPath))
            {
                sourceValue = resolvedTarget;
                return true;
            }

            try
            {
                sourceValue = DeepReflection.GetValue(resolvedTarget, propertyPath);
                return true;
            }
            catch
            {
                sourceValue = null;
                return false;
            }
        }

        private static bool IsComponentTypeOfName(Component comp, string typeName)
        {
            if (comp == null || string.IsNullOrEmpty(typeName)) return false;
            Type t = comp.GetType();
            while (t != null && t != typeof(Component) && t != typeof(MonoBehaviour) && t != typeof(UnityEngine.Object))
            {
                if (t.Name.Equals(typeName, StringComparison.OrdinalIgnoreCase)) return true;
                t = t.BaseType;
            }
            return false;
        }
    }

    public static class DefaultEventRegistration
    {
        public static void RegisterDefaultConditions(IConditionRegistry registry)
        {
            if (registry == null) return;

            // Register Resolvers
            registry.RegisterTargetResolver(ConditionTargetType.GlobalVar, new DefaultGlobalVarTargetResolver());
            registry.RegisterTargetResolver(ConditionTargetType.EventContext, new DefaultEventContextTargetResolver());
            registry.RegisterTargetResolver(ConditionTargetType.SceneObject, new DefaultSceneObjectTargetResolver());

            // Register Operators
            registry.RegisterOperator(new BoolEqualsOperatorEvaluator());

            registry.RegisterOperator(new ObjectEqualsOperatorEvaluator());
            registry.RegisterOperator(new ObjectNotEqualsOperatorEvaluator());

            registry.RegisterOperator(new StringEqualsOperatorEvaluator());
            registry.RegisterOperator(new StringNotEqualsOperatorEvaluator());

            registry.RegisterOperator(new NumericEqualsOperatorEvaluator());
            registry.RegisterOperator(new NumericNotEqualsOperatorEvaluator());
            registry.RegisterOperator(new NumericGreaterThanOperatorEvaluator());
            registry.RegisterOperator(new NumericGreaterThanEqualsOperatorEvaluator());
            registry.RegisterOperator(new NumericLessThanOperatorEvaluator());
            registry.RegisterOperator(new NumericLessThanEqualsOperatorEvaluator());

            registry.RegisterOperator(new DateEqualsOperatorEvaluator());
            registry.RegisterOperator(new DateNotEqualsOperatorEvaluator());
            registry.RegisterOperator(new DateGreaterThanOperatorEvaluator());
            registry.RegisterOperator(new DateGreaterThanEqualsOperatorEvaluator());
            registry.RegisterOperator(new DateLessThanOperatorEvaluator());
            registry.RegisterOperator(new DateLessThanEqualsOperatorEvaluator());

            registry.RegisterOperator(new DateEqualsNowOperatorEvaluator());
            registry.RegisterOperator(new DateNotEqualsNowOperatorEvaluator());
            registry.RegisterOperator(new DateGreaterThanNowOperatorEvaluator());
            registry.RegisterOperator(new DateLessThanNowOperatorEvaluator());
        }

        public static void RegisterDefaultCommands(ICommandRegistry registry, IObjectResolver container)
        {
            if (registry == null) return;
            foreach (var descriptor in DefaultCommandDescriptors.All(container))
            {
                registry.RegisterCommand(descriptor);
            }
        }
    }
}
